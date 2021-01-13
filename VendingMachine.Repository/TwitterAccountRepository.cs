using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachine.Model;

namespace VendingMachine.Repository
{
	public class TwitterAccountRepository
	{
		VendingMachineContext ctx = new VendingMachineContext();

		public TwitterAccount GetTwitterAccount(int id)
		{
			var ret = ctx.TwitterAccounts.Find(id);
			return ret;
		}

		public TwitterAccount GetTwitterAccount(string screenName)
		{
			var ret =
				ctx.TwitterAccounts.FirstOrDefault(
					x => string.Equals(x.ScreenName, screenName, StringComparison.CurrentCultureIgnoreCase));

			return ret;
		}
	}
}
