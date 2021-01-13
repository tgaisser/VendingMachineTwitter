using System;
using System.IO;

namespace VendingMachine.Repository
{
	public class Misc
	{
		VendingMachineContext context;

		public bool ExecuteSqlFile(string fileName)
		{
			context = new VendingMachineContext();

			bool executed = true;
			string cleanFileName = fileName;

			if (! cleanFileName.ToUpper().EndsWith("SQL"))
			{
				cleanFileName += ".sql";
			}

			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			FileInfo file = new FileInfo(baseDirectory + cleanFileName);
			if (file.Exists)
			{
				string sqlScript = file.OpenText().ReadToEnd();
				context.Database.ExecuteSqlCommand(sqlScript);
			}
			else
			{
				executed = false;
			}

// ReSharper disable once RedundantAssignment
			file = null;
			context = null;
			return executed;
		}
		
	}
}
