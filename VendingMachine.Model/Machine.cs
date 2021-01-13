using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine.Model
{
	public class Machine
	{
		public int Id { get; set; }

		[Required]
		public string MachineName { get; set; }
		public string MachineDescription { get; set; }

		public DateTime? CreateDate { get; set; }
		public bool IsActive { get; set; }

		public List<MachineTwitterAccount> MachineTwitterAccounts { get; set; }

	}
}
