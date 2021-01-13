using System;
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Model
{
	public class TwitterUser
	{
		public int Id { get; set; }

		[Required]
		public string TwitterId { get; set; }
		
		[Required]
		public string ScreenName { get; set; }
		public string Name { get; set; }
		public bool UserProtected { get; set; }
		public bool IsActive { get; set; }
		public DateTime? CreateDate { get; set; }
	}
}
