namespace VendingMachine.Repository.Migrations
{
	using System;
	using System.Data.Entity.Migrations;
	using System.IO;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<VendingMachine.Repository.VendingMachineContext>
	{
		private const string seedEventFileName = "SeedEvent.sql";
		private const string seedMachineAccountFileName = "SeedMachine.sql";
		private const string seedCodesFileName = "SeedCodes.sql";

		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(VendingMachine.Repository.VendingMachineContext Context)
		{
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data.
			//Seed_Events(ref Context);
			//Seed_MachineTwitterAccounts(ref Context);
			//Seed_Codes(ref Context);
			//Context.SaveChanges();



			return;


			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			FileInfo file = new FileInfo(baseDirectory + seedEventFileName);
			if (file.Exists)
			{
				string sqlScript = file.OpenText().ReadToEnd();
				Context.Database.ExecuteSqlCommand(sqlScript);
			}

			file = new FileInfo(baseDirectory + seedMachineAccountFileName);
			if (file.Exists)
			{
				string sqlScript = file.OpenText().ReadToEnd();
				Context.Database.ExecuteSqlCommand(sqlScript);
			}

			string[] filePaths = Directory.GetFiles(baseDirectory, "SeedCodes*.sql");
			if (filePaths != null)
			{
				VendingMachineContext vendingMachineContext = new VendingMachineContext();
				foreach (string fileName in filePaths)
				{
					file = new FileInfo(fileName);
					string sqlScript = file.OpenText().ReadToEnd();
					vendingMachineContext.Database.ExecuteSqlCommand(sqlScript);
				}
			}

			filePaths = Directory.GetFiles(baseDirectory, "SeedOther*.sql");
			if (filePaths != null)
			{
				VendingMachineContext vendingMachineContext = new VendingMachineContext();
				foreach (string fileName in filePaths)
				{
					file = new FileInfo(fileName);
					string sqlScript = file.OpenText().ReadToEnd();
					vendingMachineContext.Database.ExecuteSqlCommand(sqlScript);
				}
			}
		}

		public void SeedDatabase(VendingMachineContext Context)
		{
			Seed(Context);
			Context.SaveChanges();
		}


	}
}
