using System.Globalization;
using com.bluewatertech.common.logging;
using LinqToTwitter;
using LitJson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using VendingMachine.Model;
using VendingMachine.Repository;

namespace VendingMachineTwitter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string productName = "Vending Machine Twitter Monitor";
		private bool isMonitoring = false;

		private static BackgroundWorker backgroundWorker = new BackgroundWorker();
		private static TwitterInterface twitterInterface;
		private static MachineTwitterAccount machineTwitterAccount;
		private string dropboxScreenName = string.Empty;
		private string accountScreenName = string.Empty;
		private int accountDbId = 0;

		private UserStream userStream;
		private TwitterContext twitterContext;
		private StreamContent streamContent;

		// There appears to be an issue with Twitter resetting the connection after 8 hours of inactivity
		private DateTime timeOfInitialConnection;
		private int hourOfLastStatusUpdate;
		private string logFileName;

		private Stack<TwitterMention> mentions;
		private bool isResponding = false;


		public MainWindow()
		{

			logFileName = AppConfiguration.FileName;

			InitializeComponent();

			Logger.Instance.IsLogDebug = true;
			Logger.Instance.LogMessage += Instance_LogMessage;

		}

		private void Instance_LogMessage(object sender, Logger.LogMessageEventArgs e)
		{
			Dispatcher.Invoke(new Action(delegate
			{
				_lvLogView.AddLogMessage(e);
			}));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			MachineTwitterAccountRepository repository = new MachineTwitterAccountRepository();
			List<MachineTwitterAccount> twitterAccounts = repository.GetActiveMachineTwitterAccounts().OrderBy(x => x.ScreenName).ToList();

			accountScreenNameComboBox.ItemsSource = twitterAccounts;
			accountScreenNameComboBox.DisplayMemberPath = "ScreenNameWithEventName";
			accountScreenNameComboBox.SelectedValuePath = "Id";
		}

		private void startMonitor_Click(object sender, RoutedEventArgs e)
		{

			if (!isMonitoring)
			{
				dropboxScreenName = accountScreenNameComboBox.Text.Trim();
				if(!string.IsNullOrEmpty(dropboxScreenName.Trim()))
					accountDbId = (int)accountScreenNameComboBox.SelectedValue;

				//dropboxScreenName = dropboxScreenName.Replace("@", "");
				//dropboxScreenName = dropboxScreenName.Replace(" ", "");
				//accountScreenNameTextBox.Text = dropboxScreenName;

				if (! string.IsNullOrEmpty(AppConfiguration.FileName.Trim()))
				{
					string errorLogFileName = string.Format(AppConfiguration.FileName, dropboxScreenName, DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString());
					Logger.Instance.SetLogFile(errorLogFileName);
					Logger.Instance.LogInfo("Error Log file: {0}.", errorLogFileName);
				}
				else
				{
					Logger.Instance.LogInfo("No error log created due to no information within app.config file.");
				}

				//if (string.IsNullOrEmpty(dropboxScreenName))
				if (accountDbId == 0)
				{
					MessageBox.Show("Twitter account is required.");
					StopMonitor();
					return;
				}
				StartMonitor();
				//Start Monitor Here
			}
			else
			{
				StopMonitor();
				//Stop and Cleanup
			}
		}

		private void StopMonitor()
		{
			accountScreenNameComboBox.IsEnabled = true;
			accountScreenName = string.Empty;

			isMonitoring = false;
			
			if(twitterInterface != null)
				twitterInterface.RunMonitor = false;

			if(userStream != null)
				userStream.CloseStream();

			if(streamContent != null)
				streamContent.CloseStream();
			
			
			streamContent = null;
			userStream = null;
			
			Logger.Instance.LogInfo("Stopping Monitor");
			startMonitor.Content = "Start Monitor";

			this.Title = productName;
			
			//mentions = null;
		}

		private void StartMonitor()
		{
			isMonitoring = true;
			mentions = new Stack<TwitterMention>();

			Logger.Instance.LogInfo("Starting Monitor for {0}", dropboxScreenName);
			startMonitor.Content = "Stop Monitor";
			accountScreenNameComboBox.IsEnabled = false;

			System.Windows.Controls.Page myPage = new System.Windows.Controls.Page();

			this.Title = dropboxScreenName;

			InitializeDatabase();

			if (!LoginTwitter())
			{
				StopMonitor();
				return;
			}

			try
			{
				CheckHashTag();
			}
			catch(Exception e)
			{
				Logger.Instance.LogError("Exception in MainWindow.StartMonitor", e);
				StopMonitor();
				return;
			}

			Logger.Instance.LogInfo("Logging into Twitter @{0}", machineTwitterAccount.ScreenName);
			CreateTwitterInterfaceAndContext();
			timeOfInitialConnection = DateTime.Now;
			CreateUserStream();

			// Sends status update hourly
			ThreadPool.QueueUserWorkItem(delegate
			{
				while (isMonitoring)
				{
					CheckStatusUpdate();
					Thread.Sleep(10000);
				}
			});

			// Respond to tweet mentions put in stack
			ThreadPool.QueueUserWorkItem(delegate
			{
				while (isMonitoring)
				{
					RespondToMentions();
					Thread.Sleep(1000);
				}
			});

		}

		private void RespondToMentions()
		{
			if (isResponding)
				return;

			if (mentions.Count == 0)
				return;

			int mentionsProcessed = 0;
			int newCodesAssigned = 0;

			isResponding = true;

			Logger.Instance.LogInfo("Processing mentions");

			while (mentions.Count > 0)
			{
				if (ProcessTweet(mentions.Pop()))
					newCodesAssigned++;

				mentionsProcessed++;
			}

			Logger.Instance.LogInfo("Process mentions finished. Mentions Processed: {0}{1}"
				, mentionsProcessed.ToString(CultureInfo.InvariantCulture)
				, (newCodesAssigned > 0) ? " New Codes assigned: " + newCodesAssigned.ToString(CultureInfo.InvariantCulture) : ""
				);

			isResponding = false;
		}

		private void CreateTwitterInterfaceAndContext()
		{
			twitterInterface = new TwitterInterface(machineTwitterAccount) {RunMonitor = true};

			twitterContext = twitterInterface.ThisTwitterContext;
			twitterContext.AuthorizedClient.UseCompression = false;
			twitterContext.Timeout = 10000;
		}

		private void CreateUserStream()
		{
			userStream =
				twitterContext.UserStream.Where(x => x.Type == UserStreamType.User)
					.StreamingCallback(strm =>
					{
						streamContent = strm;

						// If we're not connected, reconnect
						if (strm.Status != TwitterErrorStatus.Success)
						{
							Logger.Instance.LogError("Status Exception in MainWindow.CreateUserStream", strm.Error);
							userStream = null;
							Logger.Instance.LogTrace("Reinitializing Twitter connection.");
							twitterInterface = null;
							CreateTwitterInterfaceAndContext();
							CreateUserStream();
							return;
						}



						string strmContent = strm.Content;

						if (!string.IsNullOrEmpty(strmContent.Trim()))
						{
							if (isMonitoring)
							{
								LinqToTwitter.Status status = null;
								try
								{
									JsonData json = JsonMapper.ToObject(strmContent);

									if(TwitterTypes.GetType(json) == TwitterTypes.StatusType.Mention)
										status = new Status(json);

								}
								catch (Exception e)
								{
									Logger.Instance.LogError("Exception in MainWindow.CreateUserStream: Json", e);
								}

								TwitterMention mention;
								if(status != null) {
									if (status.User != null)
									{
										bool notParentAccount = false;

										try
										{
											if (status.User.Identifier != null)
												notParentAccount = (!String.Equals(dropboxScreenName, status.User.Identifier.ScreenName, StringComparison.CurrentCultureIgnoreCase));

											if (notParentAccount)
											{
												mention = twitterInterface.GetMentionWithHashTag(status);

												if (mention != null)
												{
													//ProcessTweet(mention);
													mentions.Push(mention);
												}
											}
										}
										catch (Exception e)
										{
											mention = null;
											status = null;
											Logger.Instance.LogError("Exception in MainWindow.CreateUserStream", e);
											Logger.Instance.LogError("JSON FROM STREAM :: {0}", strmContent);
											userStream = null;
											Logger.Instance.LogTrace("Reinitializing Twitter connection.");
											twitterInterface = null;
											CreateTwitterInterfaceAndContext();
											CreateUserStream();
											return;
										}
									}
								}

								mention = null;
								status = null;
							}
						}
					})	// end stream Lambda
				.SingleOrDefault();

			SendStatusUpdate(string.Format("Monitoring account initiated. {0}", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()));
		}

		private static void CheckHashTag()
		{
			if (machineTwitterAccount.HashTag == null)
			{
				throw new Exception("Database value HashTag is null for this account.");
			}
			if (string.IsNullOrEmpty(machineTwitterAccount.HashTag))
			{
				throw new Exception("Database value HashTag is empty for this account.");
			}
		}

		private bool LoginTwitter()
		{
			bool loginSuccess = true;

			Logger.Instance.LogText("Retrieving login information for {0}", dropboxScreenName);

			//Get login information for this twitter account
			try
			{
				GetMachineTwitterAccount();
			}
			catch (Exception e)
			{
				loginSuccess = false;
				if (e.Message.StartsWith("No Twitter account information stored for ScreenName"))
				{
					Logger.Instance.LogError("No Twitter account information stored for ScreenName {0}. It is either not active or doesn't exist in the database under that ScreenName.", dropboxScreenName);
				}
				else
				{
					Logger.Instance.LogError(string.Format("There was an error while looking up {0} account in the database.", dropboxScreenName), e);
				}
			}
			return loginSuccess;
		}

		private static void InitializeDatabase()
		{
			Logger.Instance.LogTrace("Checking Database");
			CheckForDatabase checkForDatabase = new CheckForDatabase();
			if (checkForDatabase.CreateIfNotExists())
			{
				Logger.Instance.LogTrace("# Database Created");
			}
			checkForDatabase = null;
		}

		private void GetMachineTwitterAccount()
		{
			MachineTwitterAccountRepository twitterAccountRepo = new MachineTwitterAccountRepository();
			try
			{
				machineTwitterAccount = twitterAccountRepo.GetActiveMachineTwitterAccount(accountDbId);
				accountScreenName = machineTwitterAccount.ScreenName;
				if (machineTwitterAccount == null)
				{
					throw new Exception(string.Format("No Twitter account information stored for ScreenName {0}. It is either not active or doesn't exist in the database with that ScreenName.", dropboxScreenName));
				}
			}
			finally
			{
				twitterAccountRepo = null;
			}
		}

		void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			TwitterMention mention = e.UserState as TwitterMention;

			ProcessTweet(mention);
		}

		//void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		//{
		//	BackgroundWorker instance = sender as BackgroundWorker;
		//	twitterInterface.UserStreamWithTimeout(instance);
		//}


		private bool ProcessTweet(TwitterMention Mention)
		{
			// if Mention is from logged in account, then skip it.
			if (String.Equals(accountScreenName, Mention.TwitterUser.ScreenName, StringComparison.CurrentCultureIgnoreCase))
				return false;

			// Add user info to TwitterUsers if doesn't already exist
			bool isExistingUser = true;
			bool giveNewCode = true;
			TwitterUser fullUserInfo = GetUserInfo(Mention.TwitterUser, ref isExistingUser);
			//dropboxScreenName

			Mention.TwitterUser = fullUserInfo;
			CodeRepository codeRepo = new CodeRepository();


			// if new user, no need to check for previous awarded code
			if (isExistingUser)
			{
				// check Codes to see if twitter user has already received a hash tag code for that calendar day
				// Codes checked should be based on EventId of the MachineTwitterAccount eventId
				Code usedCode = codeRepo.GetCodeUsedTodayByTweetUserId(fullUserInfo.TwitterId, machineTwitterAccount.EventId, machineTwitterAccount.Id);

				// Can user get new code?
				giveNewCode = (usedCode == null);
			}

			if (giveNewCode)
			{
				// return non used code and mark code as used in Codes
				string newCode = codeRepo.SecureAndReturnNewCodeForUser(Mention, machineTwitterAccount.EventId, machineTwitterAccount.Id);
				if (string.IsNullOrEmpty(newCode))
				{
					SendStatusUpdate("No valid codes in the database to assign.");
					Logger.Instance.LogWarning("NO VALID CODES IN DATABASE TO ASSIGN");
				}

				// Reply with new code via Twitter
				string codeReplyMessage = GetRandomTweetReply();

				bool successfulReply = twitterInterface.ReplyWithNewCode(codeReplyMessage, newCode, Mention.MentionId, Mention.TwitterUser.ScreenName);
				//if (!twitterInterface.ReplyWithNewCode(codeReplyMessage, newCode, Mention.MentionId, Mention.TwitterUser.ScreenName))
				
				if (!successfulReply)
				{
					// Try again
					Logger.Instance.LogTrace("Attempting reply again.");
					CreateTwitterInterfaceAndContext();
					successfulReply = twitterInterface.ReplyWithNewCode(codeReplyMessage + "     ", newCode, Mention.MentionId, Mention.TwitterUser.ScreenName); 
					// Extra characters are so Twitter doesn't recognize it as a repeat status.
				}
				
				if (!successfulReply)
				{
					Logger.Instance.LogError("An error occured. A code was assigned but not able to send via twitter");
				}
			}

			codeRepo = null;
			fullUserInfo = null;
			return giveNewCode;
		}

		private static string GetRandomTweetReply()
		{
			string codeReplyMessage = "{0}";

			List<ReplyTweet> replyTweets = machineTwitterAccount.Event.ReplyTweets.Where(x => x.IsActive).ToList();

			int count = replyTweets.Count();

			if (count > 0)
			{
				int index = new Random().Next(count);
				codeReplyMessage = replyTweets.Skip(index).FirstOrDefault().Tweet;
			}
			else
			{
				//Grab Default Message
				if (machineTwitterAccount.CodeReplyMessage != null)
				{
					if (machineTwitterAccount.CodeReplyMessage.Trim().Length > 0)
						codeReplyMessage = machineTwitterAccount.CodeReplyMessage;
				}
			}
			return codeReplyMessage;
		}

		private TwitterUser GetUserInfo(TwitterUser twitterUser, ref bool IsExistingUser)
		{
			IsExistingUser = false;
			TwitterUserRepository repo = new TwitterUserRepository();
			TwitterUser checkUser = repo.GetTwitterUserByTwitterId(twitterUser.TwitterId);
			IsExistingUser = (checkUser != null);

			TwitterUser returnUser = IsExistingUser ? checkUser : repo.CreateNewTwitterUser(twitterUser);

			repo = null;
			checkUser = null;

			return returnUser;
		}

		private void Window_Loaded_1(object sender, RoutedEventArgs e)
		{

		}

		private bool SendStatusUpdate(string statusMessage)
		{
			const bool returnValue = true;
			hourOfLastStatusUpdate = DateTime.Now.Hour;
			twitterInterface.SendPersonalMessage(statusMessage, AppConfiguration.PmStatusUpdateToAccounts, ":STATUS UPDATE");
			return returnValue;
		}

		private void CheckStatusUpdate()
		{
			if (hourOfLastStatusUpdate != DateTime.Now.Hour)
			{
				SendStatusUpdate(string.Format("Twitter Monitor checking in and running properly. {0}", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()));
			}
		}

	}

}
