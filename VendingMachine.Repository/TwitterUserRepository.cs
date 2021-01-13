using System;
using System.Linq;
using VendingMachine.Model;

namespace VendingMachine.Repository
{
	public class TwitterUserRepository
	{
		VendingMachineContext ctx = new VendingMachineContext();
		public TwitterUser GetTwitterUser(int id)
		{
			var ret = ctx.TwitterUsers.Find(id);
			return ret;
		}

		public TwitterUser GetTwitterUserByScreenName(string screenName)
		{
			var ret = ctx.TwitterUsers.FirstOrDefault(x => String.Equals(x.ScreenName, screenName, StringComparison.CurrentCultureIgnoreCase));

			return ret;
		}
		public TwitterUser GetActiveTwitterUserByScreenName(string screenName)
		{
			var ret = ctx.TwitterUsers.FirstOrDefault(x => String.Equals(x.ScreenName, screenName, StringComparison.CurrentCultureIgnoreCase) && x.IsActive);

			return ret;
		}

		public TwitterUser GetTwitterUserByTwitterId(string twitterId)
		{
			var ret = ctx.TwitterUsers.FirstOrDefault(x => String.Equals(x.TwitterId, twitterId, StringComparison.CurrentCultureIgnoreCase));

			return ret;
		}

		public TwitterUser CreateNewTwitterUser(TwitterUser twitterUser)
		{
			twitterUser.IsActive = true;
			twitterUser.CreateDate = DateTime.Now;
			TwitterUser newUser = ctx.TwitterUsers.Add(twitterUser);
			ctx.SaveChanges();
			return newUser;
		}
	}
}
