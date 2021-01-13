using System;
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Model
{
	public class Code
	{
		public int Id { get; set; }

		[Required]
		public string CodeValue { get; set; }
		public string Description { get; set; }
		public string TweetId { get; set; }
		public string TweetMessage { get; set; }
		public string TweetUserId { get; set; }
		public DateTime? DateTweetCreated { get; set; }
		public DateTime? DateAssigned { get; set; }
		public bool IsActive { get; set; }
		public DateTime? CreateDate { get; set; }

		public int DenormalizedEventId { get; set; }

		// Navigational
		[Association("MachineTwitterAccount", "MachineTwitterAccountId", "Id")]
		public int MachineTwitterAccountId { get; set; }
		public virtual MachineTwitterAccount MachineTwitterAccount { get; set; }
	}
}
