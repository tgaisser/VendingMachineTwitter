namespace VendingMachineTwitter
{
	static class AppConfiguration
	{
		private static string fileName = string.Empty;
		public static string FileName 
		{ 
			get
			{
				if (!string.IsNullOrEmpty(fileName)) return fileName;

				var logFileName = System.Configuration.ConfigurationManager.AppSettings["LogFileLocationAndName"];
				if (logFileName != null)
				{
					return fileName = logFileName.Trim();
				}

				return fileName = string.Empty;
			}
		}

		private static string pmStatusUpdateToAccounts = string.Empty;
		public static string PmStatusUpdateToAccounts
		{
			get 
			{
				if (!string.IsNullOrEmpty(pmStatusUpdateToAccounts)) return pmStatusUpdateToAccounts;

				var screenNames = System.Configuration.ConfigurationManager.AppSettings["PMStatusUpdateToAccounts"];
				if (screenNames != null)
				{
					return pmStatusUpdateToAccounts = screenNames.Trim();
				}

				return pmStatusUpdateToAccounts = string.Empty;
			}
		}
		
	}
}
