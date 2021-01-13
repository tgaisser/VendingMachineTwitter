using com.bluewatertech.common.logging;
using LinqToTwitter;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Model;

namespace VendingMachineTwitter
{
	public class TwitterInterface
	{
		private readonly SingleUserAuthorizer singleUserAuth;
		private TwitterContext twitterContext;
/*
		private StreamContent streamContent = null;
*/
	
		private string consumerKey;
		private string consumerSecret;
		private string accessToken;
		private string accessTokenSecret;
		private MachineTwitterAccount machineTwitter;

		private string hashCode = "";

		private bool singleUserAuthorizationChecked;
		private bool isAuthorized;

		private string twitterScreenName = string.Empty;

		public TwitterInterface(MachineTwitterAccount machineTwitterAcct)
		{
			if (machineTwitterAcct == null )
			{
				throw new Exception("MachineTwitterAccount is null.");
			}
			if (! machineTwitterAcct.HasValidSecurityKeys())
			{
				throw new Exception("All security keys and tokens are required.");
			}

			singleUserAuth = CreateOAuth(machineTwitterAcct);
			twitterContext = CreateTwitterContext(singleUserAuth);
		}

		public TwitterContext ThisTwitterContext
		{
			get
			{ 
				return twitterContext;
			}
		}

		public bool RunMonitor { get; set; }

		public string HashCode
		{
			get 
			{ 
				if (string.IsNullOrEmpty(hashCode.Trim())) 
				{
					if (machineTwitter.HashTag != null)
					{
						hashCode = machineTwitter.HashTag.Trim();
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

		public bool IsAccountLoginValid(bool reCheck = false)
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
			if (singleUserAuthorizationChecked && !reCheck)
			{
				return isAuthorized;
			}

			singleUserAuthorizationChecked = true;
			isAuthorized = false;
			//TwitterQueryException twitterException = null;

			try
			{
				var account = twitterContext.Account.FirstOrDefault(t => t.Type == AccountType.VerifyCredentials);

				PopulateAccountVariables(account);

				isAuthorized = true;
			}
			catch (Exception)
			{
				//twitterException = tqe;
				isAuthorized = false;
			}

			return isAuthorized;
		}

		public TwitterContext TwitterContext { get { return twitterContext; } set { twitterContext = value; } }

		public string TwitterScreenName
		{
			get { return twitterScreenName; }
			set { twitterScreenName = value; }
		}

		public IList<TwitterMention> GetMentions() 
		{
			var myMentions =
				from mention in twitterContext.Status
				where mention.Type == StatusType.Mentions
				select mention;

			IList<TwitterMention> mentions = new List<TwitterMention>();

			myMentions.ToList().ForEach(
					mention =>
					mentions.Add(new TwitterMention
					{
						Text = mention.Text,
						MentionId = mention.StatusID,
						TweetCreated = mention.CreatedAt,
						TwitterUser = new TwitterUser
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
		public IList<TwitterMention> GetMentions(string hashTag)
		{
			var myMentions =
				twitterContext.Status.Where(mention => mention.Type == StatusType.Mentions
				                                       && mention.Text.ToUpper().Contains("#" + hashTag.ToUpper()));

			IList<TwitterMention> mentions = new List<TwitterMention>();

			myMentions.ToList().ForEach(
					mention =>
					mentions.Add(new TwitterMention
					{
						Text = mention.Text,
						MentionId = mention.StatusID,
						TweetCreated = mention.CreatedAt,
						TwitterUser = new TwitterUser
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

		public bool ReplyWithNewCode(string codeReplyMessage, string newCode, string replyToStatusId, string replyToUserScreenName)
		{
			Status reply;
			try
			{
				reply = twitterContext.UpdateStatus(string.Format("@{1} " + codeReplyMessage, newCode, replyToUserScreenName), replyToStatusId);
			}
			catch (TwitterQueryException tqe)
			{
				switch (tqe.ErrorCode)
				{
					case 187:
						Logger.Instance.LogWarning("Duplicate tweet to {0}", replyToUserScreenName);
						return true;
					default:
						Logger.Instance.LogError("Twitter Query Exception within TwitterInterface.ReplyWithNewCode", tqe);
						return false;
				}
			}
			return (reply != null);
		}

		public bool SendPersonalMessage(string messageBody, string userName, string additionalLogMessage = "")
		{
			string[] userNames = userName.Split(',');

			bool returnValue = true;


			foreach (string eachUserName in userNames)
			{
				try
				{
					DirectMessage directMessage = twitterContext.NewDirectMessage(eachUserName, messageBody);
					if (directMessage == null)
					{
						returnValue = false;
						Logger.Instance.LogInfo("PM was NOT sent to {0}. {1}", eachUserName, additionalLogMessage);
					}
					else
					{
						Logger.Instance.LogSuccess("PM sent to {0}. {1}", eachUserName, additionalLogMessage);
					}
				}
				catch(TwitterQueryException tqe)
				{
					// These are errors directly from twitter
					switch ( tqe.ErrorCode )
					{
						case 34:
							// Receiving User Screen Name doesn't exist in Twitter
							Logger.Instance.LogWarning("Unable to PM {0}. ScreenName doesn't exist. {1}", eachUserName, additionalLogMessage);
							break;

						case 150:
							// Receiving User isn't following this account
							Logger.Instance.LogWarning("Unable to PM  {0}. {1}. {2}", eachUserName, tqe.Message, additionalLogMessage);
							break;

						default:
							Logger.Instance.LogError(string.Format("QueryException in TwitterInterface.SendPersonalMessage: PM to {0} ", eachUserName), tqe);
							break;
					}
					returnValue = false;
				}
				catch (Exception e)
				{
					Logger.Instance.LogError(string.Format("PM Error in TwitterInterface.SendPersonalMessage: PM to {0} ", eachUserName), e);
					returnValue = false;
				}
			}

			return (returnValue);
		}

		public bool SendPersonalMessage(string messageBody, string[] userNames, string additionalLogMessage = "")
		{
			if (userNames.Length > 0)
			{
				foreach (string userName in userNames)
				{
					SendPersonalMessage(messageBody, userName, additionalLogMessage);
				}
			}
			return true;
		}



		public TwitterMention BuildMention(Status twitterStatus)
		{
			TwitterMention mention = new TwitterMention
			{
				MentionId = twitterStatus.StatusID,
				Text = twitterStatus.Text,
				TweetCreated = twitterStatus.CreatedAt
			};

			TwitterUser user = new TwitterUser
			{
				ScreenName = twitterStatus.User.Identifier.ScreenName,
				Name = twitterStatus.User.Name,
				TwitterId = twitterStatus.User.Identifier.UserID,
				UserProtected = twitterStatus.User.Protected
			};

			mention.TwitterUser = user;

			return mention;
		}



		public TwitterMention GetMentionWithHashTag(Status status)
		{
			if (status.Entities.HashTagEntities != null)
			{
				//				if (status.Entities.HashTagEntities.Where(ht => ht.Tag.ToUpper() == HashCode.ToUpper()).Count() > 0)
				//				if (status.Entities.HashTagEntities.Where(ht => ht.Tag.ToUpper() == HashCode.ToUpper()).Any())

				if (status.Entities.HashTagEntities.Any(ht => ht.Tag.ToUpper() == HashCode.ToUpper()))
				{
					return BuildMention(status);
				}
			}

			return null;
		}



		private void PopulateAccountVariables(Account account)
		{
			isAuthorized = true;
			TwitterScreenName = account.User.Identifier.ScreenName;
		}

		private SingleUserAuthorizer CreateOAuth(MachineTwitterAccount machineTwitterAccount)
		{
			if (machineTwitterAccount == null )
			{
				throw new Exception("MachineTwitterAccount is null.");
			}
			if (! machineTwitterAccount.HasValidSecurityKeys())
			{
				throw new Exception("All OAuth keys and tokens are required.");
			}

			consumerKey = machineTwitterAccount.ConsumerKey.Trim();
			consumerSecret = machineTwitterAccount.ConsumerSecret.Trim();
			accessToken = machineTwitterAccount.AccessToken.Trim();
			accessTokenSecret = machineTwitterAccount.AccessTokenSecret.Trim();
			machineTwitter = machineTwitterAccount;

			// Need OAuth
			var auth = new SingleUserAuthorizer
			{
				Credentials = new SingleUserInMemoryCredentials
				{
					ConsumerKey = consumerKey,
					ConsumerSecret = consumerSecret,
					TwitterAccessToken = accessToken,
					TwitterAccessTokenSecret = accessTokenSecret
				}
			};

			return auth;
		}

		private TwitterContext CreateTwitterContext(SingleUserAuthorizer singleUserAuthorizer)
		{
			TwitterContext twitterCtx = new TwitterContext(singleUserAuthorizer);
			return twitterCtx;
		}

	}

	public static class TwitterTypes
	{
		public enum StatusType
		{
			Unknown,
			Mention,
			Follow,
			Tweet,
			Reply
		}
		public static StatusType GetType(JsonData jsonData)
		{
			if (jsonData == null)
				return StatusType.Unknown;

			JsonData returnedValue;
			if (jsonData.TryGetValue("event", out returnedValue))
			{
				string eventType = returnedValue.ToString().ToUpper();

				switch (eventType)
				{
					case "FOLLOW":
						return StatusType.Follow;
				}
			}

// ReSharper disable once RedundantAssignment
			returnedValue = null;
			if (!jsonData.TryGetValue("text", out returnedValue))
			{
				return StatusType.Unknown;
			}

			return StatusType.Mention;
		}
	}
}
