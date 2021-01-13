using System;

namespace VendingMachine.Model
{
	public class TwitterMention
	{
		public TwitterUser TwitterUser { get; set; }
		public string Text { get; set; }
		public string MentionId { get; set; }
		public DateTime TweetCreated { get; set; }
	}
}
