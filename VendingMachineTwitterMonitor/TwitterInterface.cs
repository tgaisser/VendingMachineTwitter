using LinqToTwitter;
using LitJson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using VendingMachine.Model;

namespace VendingMachineTwitterMonitor
{
	public class TwitterInterface
	{
		private SingleUserAuthorizer singleUserAuth;
		private TwitterContext twitterContext;
		private StreamContent streamContent = null;
	
		private string _consumerKey;
		private string _consumerSecret;
		private string _accessToken;
		private string _accessTokenSecret;
		private MachineTwitterAccount _machineTwitterAccount;

		private string hashCode = "";

		private bool singleUserAuthorizationChecked = false;
		private bool _isAuthorized = false;

		private string _twitterScreenName = string.Empty;

		public TwitterInterface(MachineTwitterAccount MachineTwitterAcct, DateTime ExecutableStartTime)
		{
			if (MachineTwitterAcct == null )
			{
				throw new Exception("MachineTwitterAccount is null.");
			}
			if (! MachineTwitterAcct.HasValidSecurityKeys())
			{
				throw new Exception("All security keys and tokens are required.");
			}

			singleUserAuth = CreateOAuth(MachineTwitterAcct);
			twitterContext = CreateTwitterContext(singleUserAuth);
			this.ExecutableStartDateTime = ExecutableStartTime;
		}

		public DateTime ExecutableStartDateTime { get; set; }

		public string HashCode
		{
			get 
			{ 
				if (string.IsNullOrEmpty(hashCode.Trim())) 
				{
					if (_machineTwitterAccount.HashTag != null)
					{
						hashCode = _machineTwitterAccount.HashTag.Trim();
					}

					if (string.IsNullOrEmpty(hashCode.Trim()))
					{
						throw new MissingMemberException("Hash Code can not be empty.");
					}
				}
				return hashCode;
			}
			set
			{
				hashCode = value;
			}
		}

		public bool IsAccountLoginValid(bool ReCheck = false)
		{
			if (singleUserAuth == null)
			{
				throw new Exception("OAuth is null. Cannot validate login.");
			}
			if (twitterContext == null)
			{
				throw new Exception("Twitter Context is null. Cannot validate login.");
			}

			// Let's not keep checking if we already know the answer.
			if (singleUserAuthorizationChecked && !ReCheck)
			{
				return _isAuthorized;
			}

			singleUserAuthorizationChecked = true;
			_isAuthorized = false;
			TwitterQueryException twitterException = null;

			try
			{
				var account = twitterContext.Account.FirstOrDefault(t => t.Type == AccountType.VerifyCredentials);

				PopulateAccountVariables(account);

				_isAuthorized = true;
			}
			catch (TwitterQueryException tqe)
			{
				twitterException = tqe;
				_isAuthorized = false;
			}

			return _isAuthorized;
		}

		public TwitterContext TwitterContext { get { return twitterContext; } set { twitterContext = value; } }

		public IList<TwitterMention> GetMentions() 
		{
			var myMentions =
				from mention in twitterContext.Status
				where mention.Type == StatusType.Mentions
				select mention;

			IList<TwitterMention> mentions = new List<TwitterMention>();

			myMentions.ToList().ForEach(
					mention =>
					mentions.Add(new TwitterMention()
					{
						Text = mention.Text,
						MentionId = mention.StatusID,
						TweetCreated = mention.CreatedAt,
						TwitterUser = new TwitterUser()
						{
							TwitterId = mention.User.Identifier.UserID,
							Name = mention.User.Name,
							ScreenName = mention.User.Identifier.ScreenName,
							UserProtected = mention.User.Protected
						}
					})
			);

			return mentions;
		}
		public IList<TwitterMention> GetMentions(string HashTag)
		{
			var myMentions =
				from mention in twitterContext.Status
				where mention.Type == StatusType.Mentions
					&& mention.Text.ToUpper().Contains("#" + HashTag.ToUpper())
				select mention;

			IList<TwitterMention> mentions = new List<TwitterMention>();

			myMentions.ToList().ForEach(
					mention =>
					mentions.Add(new TwitterMention()
					{
						Text = mention.Text,
						MentionId = mention.StatusID,
						TweetCreated = mention.CreatedAt,
						TwitterUser = new TwitterUser()
						{
							TwitterId = mention.User.Identifier.UserID,
							Name = mention.User.Name,
							ScreenName = mention.User.Identifier.ScreenName,
							UserProtected = mention.User.Protected
						}
					})
			);

			return mentions;
		}

		public void UserStreamWithTimeout(BackgroundWorker backgroundWorker)
		{
			Thread.CurrentThread.IsBackground = true;
			twitterContext.AuthorizedClient.UseCompression = false;
			twitterContext.Timeout = 10000;

			int showStatusCount = 0;

			UserStream ustrm;
			LinqToTwitter.Status status;
			TwitterMention mention;

			Console.WriteLine("## Begin Stream Monitoring ##");

				ustrm =
					twitterContext.UserStream.Where(x => x.Type == UserStreamType.User)
					.StreamingCallback(strm =>
						{
							streamContent = strm;

							if (strm.Status != TwitterErrorStatus.Success)
							{
								Console.WriteLine();
								Console.WriteLine(strm.Error.ToString());
								return;
							}

							string strmContent = strm.Content;

							if (!string.IsNullOrEmpty(strmContent.Trim()))
							{
								try
								{
									var json = JsonMapper.ToObject(strmContent);
									status = new Status(json);
									if (status.Entities.HashTagEntities != null)
									{
										if (status.Entities.HashTagEntities.Any(ht => String.Equals(ht.Tag, HashCode, StringComparison.CurrentCultureIgnoreCase)))
										{
											mention = BuildMention(status);
											// to blast ui, loop through `1000
											backgroundWorker.ReportProgress(0, mention);
										}
									}

								}
								catch (Exception e)
								{
									string except = e.Message;
									Console.WriteLine();
									Console.WriteLine(string.Format("### ERROR ### {0} - {1}", DateTime.Now.ToString(), except));
								}
								finally
								{
									mention = null;
									status = null;
								}

							}
						})
					.SingleOrDefault();

				//while (streamContent == null)
				//{
				//	Console.WriteLine();
				//	Console.WriteLine("# Initializing Stream");
				//	Thread.Sleep(30000);
				//}
				
				//Thread.Sleep(30000);

				showStatusCount++;
				if (showStatusCount % 5 == 0)
				{
					Console.WriteLine();
					Console.WriteLine(string.Format("# {0} -> Uptime: {1}", DateTime.Now, DateTime.Now.Subtract(ExecutableStartDateTime)));
				}

		}


		public bool ReplyWithNewCode(string CodeReplyMessage, string NewCode, string ReplyToStatusId, string ReplyToUserScreenName)
		{
			Status reply;
			try
			{
				reply = twitterContext.UpdateStatus(string.Format("@{1} " + CodeReplyMessage, NewCode, ReplyToUserScreenName), ReplyToStatusId);
			}
			//catch (TwitterQueryException tqe)
			catch
			{
				return false;
			}
			return (reply != null);
		}






		public TwitterMention BuildMention(Status TwitterStatus)
		{
			TwitterMention mention = new TwitterMention();

			mention.MentionId = TwitterStatus.StatusID;
			mention.Text = TwitterStatus.Text;
			mention.TweetCreated = TwitterStatus.CreatedAt;

			TwitterUser user = new TwitterUser();
			user.ScreenName = TwitterStatus.User.Identifier.ScreenName;
			user.Name = TwitterStatus.User.Name;
			user.TwitterId = TwitterStatus.User.Identifier.UserID;
			user.UserProtected = TwitterStatus.User.Protected;

			mention.TwitterUser = user;

			return mention;
		}







		private void PopulateAccountVariables(Account Account)
		{
			_isAuthorized = true;
			_twitterScreenName = Account.User.Identifier.ScreenName;
		}

		private SingleUserAuthorizer CreateOAuth(MachineTwitterAccount MachineTwitterAccount)
		{
			if (MachineTwitterAccount == null )
			{
				throw new Exception("MachineTwitterAccount is null.");
			}
			if (! MachineTwitterAccount.HasValidSecurityKeys())
			{
				throw new Exception("All OAuth keys and tokens are required.");
			}

			_consumerKey = MachineTwitterAccount.ConsumerKey.Trim();
			_consumerSecret = MachineTwitterAccount.ConsumerSecret.Trim();
			_accessToken = MachineTwitterAccount.AccessToken.Trim();
			_accessTokenSecret = MachineTwitterAccount.AccessTokenSecret.Trim();
			_machineTwitterAccount = MachineTwitterAccount;

			// Need OAuth
			var auth = new SingleUserAuthorizer
			{
				Credentials = new SingleUserInMemoryCredentials
				{
					ConsumerKey = _consumerKey,
					ConsumerSecret = _consumerSecret,
					TwitterAccessToken = _accessToken,
					TwitterAccessTokenSecret = _accessTokenSecret
				}
			};

			return auth;
		}

		private TwitterContext CreateTwitterContext(SingleUserAuthorizer SingleUserAuthorizer)
		{
			TwitterContext twitterCtx = new TwitterContext(SingleUserAuthorizer);
			return twitterCtx;
		}

	}
}
