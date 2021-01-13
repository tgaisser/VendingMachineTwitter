using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Model
{
	public class Event
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public DateTime? CreateDate { get; set; }
		public bool IsActive { get; set; }

		// Navigational
		public virtual ICollection<MachineTwitterAccount> MachineTwitterAccounts { get; set; }
		public virtual ICollection<ReplyTweet> ReplyTweets { get; set; }

	}
}
