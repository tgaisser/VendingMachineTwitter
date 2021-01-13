using System.Data.Entity;
using VendingMachine.Model;

namespace VendingMachine.Repository
{
	public class VendingMachineContext : DbContext
	{
		public VendingMachineContext() : base("SocialVendingMachine") { }

		public DbSet<Code> Codes { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<Machine> Machines { get; set; }
		public DbSet<MachineTwitterAccount> MachineTwitterAccounts { get; set; }
		public DbSet<TwitterAccount> TwitterAccounts { get; set; }	
		public DbSet<TwitterUser> TwitterUsers { get; set; }
		public DbSet<ReplyTweet> ReplyTweets { get; set; }
	}
}
