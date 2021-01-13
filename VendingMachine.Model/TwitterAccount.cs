using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Model
{
	public class TwitterAccount
	{
		public int Id { get; set; }

		[Required]
		public string ScreenName { get; set; }
		public string Password { get; set; }

		[Required]
		public string ConsumerKey { get; set; }
		[Required]
		public string ConsumerSecret { get; set; }

		[Required]
		public string AccessToken { get; set; }
		[Required]
		public string AccessTokenSecret { get; set; }

		public DateTime? CreateDate { get; set; }
		public bool IsActive { get; set; }

		// Navigational
		public virtual ICollection<MachineTwitterAccount> MachineTwitterAccounts  { get; set; }

	}
}
