using System.Configuration;

namespace VendingMachineReportExport
{
	static class AppConfiguration
	{
		private static string templateFileName = string.Empty;
		public static string TemplateFileName 
		{ 
			get
			{
				if (!string.IsNullOrEmpty(templateFileName)) return templateFileName;

				var readValue = ConfigurationManager.AppSettings["TemplateFileName"];

				if (readValue != null)
				{
					return templateFileName = readValue.Trim();
				}

				templateFileName = string.Empty;
				return templateFileName;
			}
		}

		private static string csvDirectory = string.Empty;
		public static string CsvDirectory
		{
			get
			{
				if (!string.IsNullOrEmpty(csvDirectory)) return csvDirectory;

				var readValue = ConfigurationManager.AppSettings["CsvDirectory"];
				if (readValue != null)
				{
					return csvDirectory = readValue.Trim();
				}
				
				csvDirectory = string.Empty;
				return csvDirectory;
			}
		}

		private static string outputDirectory = string.Empty;
		public static string OutputDirectory
		{
			get
			{
				if (!string.IsNullOrEmpty(outputDirectory)) return outputDirectory;

				var readValue = ConfigurationManager.AppSettings["OutputDirectory"];
				if (readValue != null)
				{
					return outputDirectory = readValue.Trim();
				}

				outputDirectory = string.Empty;
				return outputDirectory;
			}
		}

		private static string outputFileName = string.Empty;
		public static string OutputFileName
		{
			get
			{
				if (!string.IsNullOrEmpty(outputFileName)) return outputFileName;

				var readValue = ConfigurationManager.AppSettings["OutputFileName"];
				if (readValue != null)
				{
					return outputFileName = readValue.Trim();
				}

				outputFileName = string.Empty;
				return outputFileName;
			}
		}
	}
}
