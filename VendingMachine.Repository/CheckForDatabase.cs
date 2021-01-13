namespace VendingMachine.Repository
{
	public class CheckForDatabase
	{
		public bool CreateIfNotExists()
		{
			VendingMachineContext ctx = new VendingMachineContext();
			bool ret = ctx.Database.CreateIfNotExists();
			if (ret)
			{
				Migrations.Configuration seed = new Migrations.Configuration();
				seed.SeedDatabase(ctx);
			}
// ReSharper disable once RedundantAssignment
			ctx = null;
			return ret;
		}
	}
}
