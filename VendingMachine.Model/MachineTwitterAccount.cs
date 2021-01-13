using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachine.Model
{
	public class MachineTwitterAccount
	{
		public int Id { get; set; }

		[Required]
		public string HashTag { get; set; }
		public string CodeReplyMessage { get; set; }
		public DateTime? CreateDate { get; set; }
		public bool IsActive { get; set; }

		// Navigational
		public virtual ICollection<Code> Codes { get; set; }

		[Association("Event", "EventId", "Id")]
		public int EventId { get; set; }
		public virtual Event Event { get; set; }

		[Association("TwitterAccount", "TwitterAccountId", "Id")]
		public int TwitterAccountId { get; set; }
		public virtual TwitterAccount TwitterAccount { get; set; }

		[Association("Machine", "MachineId", "Id")]
		public int MachineId { get; set; }
		public virtual Machine Machine { get; set; }





		// NON MAPPED
		public bool HasValidSecurityKeys()
		{
			if (TwitterAccount == null)
				return false;

			try
			{
				return !(string.IsNullOrEmpty(ConsumerKey.Trim()) ||
					string.IsNullOrEmpty(ConsumerSecret.Trim()) ||
					string.IsNullOrEmpty(AccessToken.Trim()) ||
					string.IsNullOrEmpty(AccessTokenSecret.Trim()));
			}
			catch
			{
				return false;
			}
		}

		[NotMapped]
		public string ScreenNameWithEventName {
			get
			{
				if (TwitterAccount == null)
					return null;

				string retValue = "@" + ScreenName;
				if (Event != null)
				{
					retValue += " - " + Event.Name;
				}
				return retValue;

			}
		}

		[NotMapped]
		public string ScreenName
		{
			get
			{
				return TwitterAccount == null ? null : TwitterAccount.ScreenName;
			}
			set
			{
				TwitterAccount.ScreenName = value;
			}
		}
		[NotMapped]
		public string Password
		{
			get
			{
				return TwitterAccount == null ? null : TwitterAccount.Password;
			}
			set
			{
				TwitterAccount.Password = value;
			}
		}

		[NotMapped]
		public string ConsumerKey
		{
			get
			{
				return TwitterAccount == null ? null : TwitterAccount.ConsumerKey;
			}
			set
			{
				TwitterAccount.ConsumerKey = value;
			}
		}
		[NotMapped]
		public string ConsumerSecret
		{
			get
			{
				return TwitterAccount == null ? null : TwitterAccount.ConsumerSecret;
			}
			set
			{
				TwitterAccount.ConsumerSecret = value;
			}
		}

		[NotMapped]
		public string AccessToken
		{
			get
			{
				return TwitterAccount == null ? null : TwitterAccount.AccessToken;
			}
			set
			{
				TwitterAccount.AccessToken = value;
			}
		}
		[NotMapped]
		public string AccessTokenSecret
		{
			get
			{
				return TwitterAccount == null ? null : TwitterAccount.AccessTokenSecret;
			}
			set
			{
				TwitterAccount.AccessTokenSecret = value;
			}
		}

		[NotMapped]
		public string MachineName
		{
			get
			{
				return Machine == null ? null : Machine.MachineName;
			}
			set
			{
				Machine.MachineName = value;
			}
		}

		[NotMapped]
		public string MachineDescription
		{
			get
			{
				return Machine == null ? null : Machine.MachineDescription;
			}
			set
			{
				Machine.MachineDescription = value;
			}
		}
		


	}
}
