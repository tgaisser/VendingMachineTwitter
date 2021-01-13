using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Model;

namespace VendingMachine.Repository
{
	public class MachineTwitterAccountRepository
	{
// ReSharper disable once FieldCanBeMadeReadOnly.Local
		VendingMachineContext vendingMachineContext = new VendingMachineContext();

		public MachineTwitterAccount GetMachineTwitterAccount(int id)
		{
			var ret = vendingMachineContext.MachineTwitterAccounts.Find(id);
			return ret;				
		}

		//public MachineTwitterAccount GetMachineTwitterAccount(string TwitterAccountName)
		//{
		//	var ret = vendingMachineContext.MachineTwitterAccounts
		//		.Where(x => x.ScreenName.ToUpper() == TwitterAccountName.ToUpper()).FirstOrDefault();
		//	return ret;
		//}

		public MachineTwitterAccount GetActiveMachineTwitterAccount(int id)
		{
			MachineTwitterAccount machineTwitterAccount = GetMachineTwitterAccount(id);
			if (machineTwitterAccount == null)
			{
				return null;
			}

			if (machineTwitterAccount.IsActive)
			{
				return machineTwitterAccount;
			}
			return null;
		}
		//public MachineTwitterAccount GetActiveMachineTwitterAccount(string TwitterAccountName)
		//{

		//	MachineTwitterAccount machineTwitterAccount = vendingMachineContext.MachineTwitterAccounts
		//		.Where(x => x.ScreenName.ToUpper() == TwitterAccountName.ToUpper() && x.IsActive).FirstOrDefault();

		//	return machineTwitterAccount;
		//}

		public List<MachineTwitterAccount> GetMachineTwitterAccounts()
		{
			return vendingMachineContext.MachineTwitterAccounts.ToList();
		}

		public List<MachineTwitterAccount> GetMachineTwitterAccounts(string machineName)
		{
			return vendingMachineContext.MachineTwitterAccounts.Where(x => String.Equals(x.MachineName, machineName, StringComparison.CurrentCultureIgnoreCase)).ToList();
		}

		public List<MachineTwitterAccount> GetMachineTwitterAccounts(int eventId)
		{
			return vendingMachineContext.MachineTwitterAccounts.Where(x => x.EventId == eventId).ToList();
		}

		public List<MachineTwitterAccount> GetActiveMachineTwitterAccounts()
		{
			return vendingMachineContext.MachineTwitterAccounts.Where(x => x.IsActive).ToList();
		}
	}
}
