using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using VendingMachine.Model;
using VendingMachine.Repository;

namespace VendingMachineTwitterMonitor
{
	class Program
	{

		private static BackgroundWorker backgroundWorker = new BackgroundWorker();
		private static TwitterInterface twitterInterface;
		private static MachineTwitterAccount machineTwitterAccount;
		private static string accountScreenName = "";
		private static DateTime startExeDate;

		static void Main(string[] args)
		{
			startExeDate = DateTime.Now;

			Console.WriteLine(" ----------------------------------------");
			Console.WriteLine("| Vending Machine Social Network Monitor |");
			Console.WriteLine(" ----------------------------------------");


			// Use CommandLine Parameter as accountScreenName to check database and log in
			if (args.Length == 0)
			{
				Console.WriteLine("# What is the screen name of the account to monitor?");
				accountScreenName = Console.ReadLine();
				accountScreenName = accountScreenName.Trim();
				if (string.IsNullOrEmpty(accountScreenName))
				{
					Console.WriteLine("# Account screen name is required.");
					ShowHelp();
					return;
				}
			}
			else
			{
				// Break up command parameters and handle accordingly
				if (args.Length > 1 && args[0].ToUpper() != "SQL")
				{
					Console.WriteLine(string.Format("# using only the first command line parameter : {0}", args[0]));
				}

				string argument = args[0].Trim().ToUpper();

				if (argument.StartsWith("/") || argument.StartsWith("-"))
				{
					string restOfArgument = argument.Substring(1);
					if (restOfArgument == "HELP" || restOfArgument == "?")
					{
						ShowHelp();
						return;
					}
					else if (restOfArgument == "sql")
					{
						if (args.Length < 2)
						{
							Console.WriteLine("# a SQL Script file name is required. See -help for more info.");
							return;
						}
						Console.WriteLine(string.Format("# Executing SQL Script {0}", args[1]));

						Misc miscRepo = new Misc();

						Console.WriteLine(miscRepo.ExecuteSqlFile(args[1].Trim())
							? "# Script executed."
							: "# COULD NOT FIND THE SQL SCRIPT FILE. Cancelling.");

						miscRepo = null;
						return;
					}
					else
					{
						Console.WriteLine(string.Format("# unknown command line parameter [{0}]", args[0]));
						ShowHelp();
						return;
					}
				}
				else if (argument.StartsWith("?"))
				{
					ShowHelp();
					return;
				}
				else if (argument.StartsWith("@"))
				{
					// must be account name
					accountScreenName = args[0].Trim();
					if (accountScreenName.Length == 1)
					{
						Console.WriteLine("# Account screen name is required.");
						return;
					}
				}
				else
				{
					// use as account name
					accountScreenName = args[0].Trim();
				}

				accountScreenName = args[0];
			}

			accountScreenName = accountScreenName.Replace("@", "");

			if (string.IsNullOrEmpty(accountScreenName.Trim()))
			{
				Console.WriteLine("# The Account Screen Name can not be empty.");
				ShowHelp();
				return;
			}

			Console.WriteLine("# Checking For Database");
			CheckForDatabase checkForDatabase = new CheckForDatabase();
			if (checkForDatabase.CreateIfNotExists())
			{
				Console.WriteLine("# Database Created");
			}
			checkForDatabase = null;


			// Start of real application.

			backgroundWorker.DoWork += backgroundWorker_DoWork;
			backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
			backgroundWorker.WorkerReportsProgress = true;


			Console.WriteLine(string.Format("# {0} : Retrieving login information for @{1}", DateTime.Now, accountScreenName));

			//Get login information for this twitter account
			try
			{
				GetMachineTwitterAccount(accountScreenName);
			}
			catch (Exception e)
			{
				Console.WriteLine("### ERROR ###");
				if (e.Message.StartsWith("No Twitter account information stored for ScreenName"))
				{
					Console.WriteLine(string.Format("# No Twitter account information stored for ScreenName @{0}.", accountScreenName));
					Console.WriteLine("# It is either not active or doesn't exist in the database with that ScreenName.");
				}
				else
				{
					Console.WriteLine(string.Format("# There was an error while looking up @{0} account in the database.", accountScreenName));
				}
				return;
			}

			if (machineTwitterAccount.HashTag == null)
			{
				throw new Exception("HashTag is null for this account.");
			}
			if (machineTwitterAccount.HashTag == null)
			{
				throw new Exception("HashTag is empty for this account.");
			}


			Console.WriteLine(string.Format("# {0} : Logging into Twitter @{1}", DateTime.Now, machineTwitterAccount.ScreenName));
	
			// Instantiate twitter communications
			Console.WriteLine(string.Format("# {0} : Monitoring for Mentions.",DateTime.Now));

			twitterInterface = new TwitterInterface(machineTwitterAccount, startExeDate);

			backgroundWorker.RunWorkerAsync();

			while (true)
			{
				Thread.Sleep(500);
			}

			twitterInterface = null;

		}

		static void ShowHelp() 
		{
			string exeName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
			string bird = "                       \\/\n                    __.---;_\n                  .'  './'0)',\\\n                  |o)  |     | ';\n                  :'--; \\.__/'   ;\n                   ;.' (         |\n              __.-'   _.)        |\n        ---==\"=----'''           |\n                 ;^;         .  ^+^^;\n               ;^  :         :       ^;\n                \\  {          :_     /\n                 ^'-;          :'--'^\n                    \",,____,,-'\n                 __   _______   ______\n       ============(((=======(((============\n";
			Console.WriteLine(bird);
			Console.WriteLine();


			Console.WriteLine("---ENTERING A SCREEN NAME TO LOG IN AS---");
			Console.WriteLine(string.Format("     1) {0} <screenname>", exeName));
			Console.WriteLine(string.Format("     2) {0} @<screenname>", exeName));
			Console.WriteLine(string.Format("     3) {0}", exeName));
			Console.WriteLine("          -- If screen name is blank, you will be prompted");
			Console.WriteLine("             to enter it.");

			Console.WriteLine();
			Console.WriteLine("---DISPLAYING HELP---");
			Console.WriteLine(string.Format("     {0} ?|/?|-?|/help|-help", exeName));

			Console.WriteLine();
			Console.WriteLine("---RUNNING SPECIFIC SQL SCRIPT---");
			Console.WriteLine(string.Format("     {0} /sql|-sql <script file name>.sql", exeName));
			Console.WriteLine("          NOTE: The file must be located in the program file directory.");
			
			Console.WriteLine();
			Console.WriteLine("Continue?");
			Console.ReadKey();


		}

		static void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			TwitterMention mention = e.UserState as TwitterMention;

			ProcessTweet(mention);
		}

		static void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker instance = sender as BackgroundWorker;
			twitterInterface.UserStreamWithTimeout(instance);
		}

		private static void GetMachineTwitterAccount(int MachineTwitterAccountId)
		{
			MachineTwitterAccountRepository twitterAccountRepo = new MachineTwitterAccountRepository();
			try 
			{
				machineTwitterAccount = twitterAccountRepo.GetActiveMachineTwitterAccount(MachineTwitterAccountId);
				if (machineTwitterAccount == null)
				{
					throw new Exception(string.Format("Machine Twitter account for id {0} not found. It is either not active or doesn't exist in the database.", MachineTwitterAccountId));
				}
			}
			finally 
			{
				twitterAccountRepo = null;
			}
		}

		//private static void GetMachineTwitterAccount(string MachineTwitterAccountScreenName)
		//{
		//	MachineTwitterAccountRepository twitterAccountRepo = new MachineTwitterAccountRepository();
		//	try 
		//	{
		//		machineTwitterAccount = twitterAccountRepo.GetActiveMachineTwitterAccount(MachineTwitterAccountScreenName);
		//		if (machineTwitterAccount == null)
		//		{
		//			throw new Exception(string.Format("No Twitter account information stored for ScreenName {0}. It is either not active or doesn't exist in the database with that ScreenName.", MachineTwitterAccountScreenName));
		//		}
		//	}
		//	finally
		//	{
		//		twitterAccountRepo = null;
		//	}
		//}

		private static void ProcessTweet(TwitterMention Mention)
		{
			Console.WriteLine();
			Console.WriteLine(string.Format("# {0} : Processing mentions", DateTime.Now));

			// Add user info to TwitterUsers if doesn't already exist
			bool isExistingUser = true;
			bool giveNewCode = true;
			TwitterUser fullUserInfo = GetUserInfo(Mention.TwitterUser, ref isExistingUser);
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
					Console.WriteLine(string.Format("# !!!!! {0} : NO VALID CODES IN DATABASE TO ASSIGN !!!!!", DateTime.Now));
				}

				// Reply with new code via Twitter
				twitterInterface.ReplyWithNewCode(machineTwitterAccount.CodeReplyMessage, newCode, Mention.MentionId, Mention.TwitterUser.ScreenName);
			}

			Console.WriteLine(string.Format("# {0} : Finished Processing mentions.{1}", DateTime.Now, giveNewCode ? " New Code assigned": ""));

			codeRepo = null;
			fullUserInfo = null;

		}

		private static TwitterUser GetUserInfo(TwitterUser TwitterUser, ref bool IsExistingUser)
		{
			IsExistingUser = false;
			TwitterUserRepository repo = new TwitterUserRepository();
			TwitterUser checkUser = repo.GetTwitterUserByTwitterId(TwitterUser.TwitterId);
			IsExistingUser = (checkUser != null);

			TwitterUser returnUser;
			if (IsExistingUser)
			{
				returnUser = checkUser;
			}
			else
			{
				returnUser = repo.CreateNewTwitterUser(TwitterUser);
			}

			repo = null;
			checkUser = null;

			return returnUser;
		}

	}
}
