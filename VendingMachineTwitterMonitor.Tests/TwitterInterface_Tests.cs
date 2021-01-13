using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToTwitter;
using VendingMachine.Model;
using System.Collections.Generic;

namespace VendingMachineTwitterMonitor.Tests
{
	[TestClass]
	public class TwitterInterface_Tests
	{
		#region AccountTest Info
		private const int _ID = 1;
		private const string _MACHINENAME = "TonyTestMachineA";
		private const string _MACHINEDESCRIPTION = "Testing Machine A for Tony ";
		private const string _SCREENNAME = "BluewaterTonyG";
		private const string _PASSWORD = "";
		private const string _CONSUMERKEY = "11111";
		private const string _CONSUMERSECRET = "11111";
		private const string _ACCESSTOKEN = "11111-111111";
		private const string _ACCESSTOKENSECRET = "11111";
		private const string _HASHTAG = "ThisRocksTesting";
		private const bool _ISACTIVE = true;
		private const int _EVENTID = 1;
		// might Need to create a Event object and Codes object collection
		#endregion

		#region Build Required Objects For Testing
		private MachineTwitterAccount MachineTwitterAccountObject()
		{
			Event eventObject = EventObject();
			IList<Code> codes = CodeCollection();

			MachineTwitterAccount machineTwitterAccount = new MachineTwitterAccount()
				{
					Id = _ID,
					MachineName = _MACHINENAME,
					MachineDescription = _MACHINEDESCRIPTION,
					ScreenName = _SCREENNAME,
					Password = _PASSWORD,
					ConsumerKey = _CONSUMERKEY,
					ConsumerSecret = _CONSUMERSECRET,
					AccessToken = _ACCESSTOKEN,
					AccessTokenSecret = _ACCESSTOKENSECRET,
					HashTag = _HASHTAG,
					CreateDate = DateTime.Now,
					IsActive = _ISACTIVE,
					Codes = codes,
					EventId = _EVENTID,
					Event = eventObject
				};

			return machineTwitterAccount;
		}

		private Event EventObject()
		{
			Event eventObj = new Event()
				{
					Id = 1,
					Name = "Testing State Fair",
					Description = "Testing event",
					IsActive = true,
					CreateDate = DateTime.Now.AddDays(-10)
				};

			return eventObj;
		}

		private IList<Code> CodeCollection()
		{
			IList<Code> codes = new List<Code>();
			codes.Add(new Code()
			{
				Id = 1,
				CodeValue = "A12345",
				Description = "",
				IsActive = true,
				CreateDate = DateTime.Now,
				DenormalizedEventId = _EVENTID,
				MachineTwitterAccountId =1
			});
			codes.Add(new Code()
			{
				Id = 2,
				CodeValue = "A78901",
				Description = "",
				IsActive = true,
				DenormalizedEventId = _EVENTID,
				CreateDate = DateTime.Now,
				MachineTwitterAccountId = 1
			});
			codes.Add(new Code()
			{
				Id = 3,
				CodeValue = "A12345",
				Description = "",
				IsActive = false,
				DenormalizedEventId = 2,
				CreateDate = DateTime.Now,
				MachineTwitterAccountId = 1
			});
			codes.Add(new Code()
			{
				Id = 4,
				CodeValue = "A75757",
				Description = "",
				TweetId = "9876543210",
				TweetMessage = "This is awesome, dude!",
				TweetUserId = "9988776655",
				DateTweetCreated = DateTime.Now.AddMinutes(-12),
				DateAssigned = DateTime.Now.AddMinutes(-1),
				IsActive = true,
				CreateDate = DateTime.Now,
				DenormalizedEventId = _EVENTID,
				MachineTwitterAccountId = 1
			});



			return codes;
		}
		#endregion

		#region TwitterInterface Object Initialization Tests
		[TestMethod]
		public void TwitterInterface_with_null_AccessTokenSecret_should_throw_error()
		{
			Exception expectedException = null;
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			machineTwitterAccount.AccessTokenSecret = null;
			try
			{
				TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			}
			catch (Exception ex)
			{
				expectedException = ex;
			}
			Assert.IsNotNull(expectedException);
		}

		[TestMethod]
		public void TwitterInterface_with_null_AccessToken_should_throw_error()
		{
			Exception expectedException = null;
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			machineTwitterAccount.AccessToken = null;
			try
			{
				TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			}
			catch (Exception ex)
			{
				expectedException = ex;
			}
			Assert.IsNotNull(expectedException);
		}

		[TestMethod]
		public void TwitterInterface_with_null_ConsumerKey_should_throw_error()
		{
			Exception expectedException = null;
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			machineTwitterAccount.ConsumerKey = null;
			try
			{
				TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			}
			catch (Exception ex)
			{
				expectedException = ex;
			}
			Assert.IsNotNull(expectedException);
		}

		[TestMethod]
		public void TwitterInterface_with_null_ConsumerSecret_should_throw_error()
		{
			Exception expectedException = null;
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			machineTwitterAccount.ConsumerSecret = null;
			try
			{
				TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			}
			catch (Exception ex)
			{
				expectedException = ex;
			}
			Assert.IsNotNull(expectedException);
		}

		[TestMethod]
		public void TwitterInterface_with_emptystring_AccessTokenSecret_should_throw_error()
		{
			Exception expectedException = null;
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			machineTwitterAccount.AccessTokenSecret = "";
			try
			{
				TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			}
			catch (Exception ex)
			{
				expectedException = ex;
			}
			Assert.IsNotNull(expectedException);
		}

		[TestMethod]
		public void TwitterInterface_with_emptystring_AccessToken_should_throw_error()
		{
			Exception expectedException = null;
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			machineTwitterAccount.AccessToken = "";
			try
			{
				TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			}
			catch (Exception ex)
			{
				expectedException = ex;
			}
			Assert.IsNotNull(expectedException);
		}

		[TestMethod]
		public void TwitterInterface_with_emptystring_ConsumerKey_should_throw_error()
		{
			Exception expectedException = null;
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			machineTwitterAccount.ConsumerKey = "";
			try
			{
				TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			}
			catch (Exception ex)
			{
				expectedException = ex;
			}
			Assert.IsNotNull(expectedException);
		}

		[TestMethod]
		public void TwitterInterface_with_emptystring_ConsumerSecret_should_throw_error()
		{
			Exception expectedException = null;
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			machineTwitterAccount.ConsumerSecret = "";
			try
			{
				TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			}
			catch (Exception ex)
			{
				expectedException = ex;
			}
			Assert.IsNotNull(expectedException);
		}

		[TestMethod]
		public void TwitterInterface_with_all_login_information_instantiates_properly()
		{
			Exception expectedException = null;
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			try
			{
				TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			}
			catch (Exception ex)
			{
				expectedException = ex;
			}
			Assert.IsNull(expectedException);
		}

		#endregion



		[TestMethod]
		public void Twitter_account_is_unauthorized()
		{
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			machineTwitterAccount.AccessTokenSecret = "aaaaa";
			machineTwitterAccount.AccessToken = "bbbbb";
			machineTwitterAccount.ConsumerSecret = "ccccc";
			machineTwitterAccount.ConsumerKey = "ddddd";

			TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			Assert.AreEqual(false, twitterInterface.IsAccountLoginValid());
		}

		[TestMethod]
		public void Twitter_account_is_authorized()
		{
			MachineTwitterAccount machineTwitterAccount = MachineTwitterAccountObject();
			TwitterInterface twitterInterface = new TwitterInterface(machineTwitterAccount, DateTime.Now);
			Assert.AreEqual(true, twitterInterface.IsAccountLoginValid());
		}

	}
}
