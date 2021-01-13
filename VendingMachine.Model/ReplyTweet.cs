using System;
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Model
{
	public class ReplyTweet
	{
		public int Id { get; set; }

		[Required]
		public string Tweet { get; set; }
		public DateTime? CreateDate { get; set; }
		public bool IsActive { get; set; }

		// Navigational
		[Association("Event", "EventId", "Id")]
		public int EventId { get; set; }
		public virtual Event Event { get; set; }

	}
}
