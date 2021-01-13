using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Globalization;

namespace VendingMachineReportExport
{
	class Program
	{
		private static string templatePath;
		private static string csvPath;
		private static string outputPath;
		private static string prevDirectory;

// ReSharper disable RedundantDefaultFieldInitializer
		private static Excel.Application excelApp = null;
		private static Excel.Worksheet xlSheet = null;
		private static Excel.Workbook xlBook = null;
// ReSharper restore RedundantDefaultFieldInitializer

		private static int startingWorksheetRow = 2;
		private static readonly object oSaveChanges = true;
		private static object oFilename;
		private static readonly object oRouteWorkbook = Missing.Value;


// ReSharper disable once UnusedParameter.Local
		static void Main(string[] args)
		{
			/*
		 *1) check for directory (dropbox)
		 *2) check for existing csv files (dropbox)
		 *3) load file(s)
		 *4) process file into desired excel format
		 *5) save excel file to desired directory
		 *6) delete processed csv file
		 * 
		 */
			Console.WriteLine("Export Process started.");
			InitializeFileInfo();

			try
			{
				// Loop through directories and sub directories for csv files
				LoopDir(csvPath);

			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.Read();
				//throw e;
			}
			finally
			{
				if (xlBook != null)
					xlBook.Close(oSaveChanges, oFilename, oRouteWorkbook);

				Thread.Sleep(1000);
				if (excelApp != null)
				{
					excelApp.Quit();
					Marshal.ReleaseComObject(excelApp);
				}
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Thread.Sleep(1000);
			}
			Console.WriteLine("Export Process completed.");
		}


		private static void ProcessCsvFiles(string directory)
		{
			foreach (string f in Directory.GetFiles(directory, "*.csv"))
			{
				// Process CSV File
				if (!Directory.Exists(outputPath))
					Directory.CreateDirectory(outputPath);

				CreateSpreadsheet(f, outputPath + string.Format(AppConfiguration.OutputFileName, DateTime.Now.Year.ToString(CultureInfo.InvariantCulture) + DateTime.Now.Month.ToString(CultureInfo.InvariantCulture) + DateTime.Now.Day.ToString(CultureInfo.InvariantCulture)) );

				// Delete CSV File
				//File.Delete(f);
			}
		}

		private static void LoopDir(string directory, bool deleteDirectory = false)
		{
			if (prevDirectory != directory)
				ProcessCsvFiles(directory);

			// Loop through directories and sub directories for csv files
			foreach (string d in Directory.GetDirectories(directory))
			{
				prevDirectory = d;
				ProcessCsvFiles(d);

				//LoopDir(d, true);
				LoopDir(d);

				// Delete Directory
				if(deleteDirectory)
				{
					if(!Directory.GetDirectories(d).Any() && !Directory.GetFiles(d).Any())
						Directory.Delete(d);
				}
			}
		}

		private static DataTable CsvFileToDatatable(string path, bool isFirstRowHeader)
		{
			string header = "No";
// ReSharper disable once RedundantAssignment
			DataTable dtCsv = null;

			//try
			//{
				string pathOnly = Path.GetDirectoryName(path);
				string fileName = Path.GetFileName(path);

				string sql = @"SELECT * FROM [" + fileName + "]";

				if (isFirstRowHeader)
				{
					header = "Yes";
				}

				using (OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly + ";Extended Properties=\"Text;HDR=" + header + "\""))
				{
					using (OleDbCommand cmd = new OleDbCommand(sql, connection))
					{
						using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
						{
							dtCsv = new DataTable {Locale = CultureInfo.CurrentCulture};
							adapter.Fill(dtCsv);
						}
					}

				}
			//}
			//catch (Exception ex)
			//{
			//	throw ex;
			//}
			return dtCsv;
		}

		private static void InitializeFileInfo()
		{
			Console.WriteLine("Initializing file/directory info.");

			//string templatePath = @"..\..\ExcelTemplate\FTPLogReport.xltx";
			//string CSVPath = @"..\..\Input\08Oct2013_FTPLogReport.csv";
			//string outputPath = @"..\..\Output\08Oct2013_FTPLogReport.xlsx";

			templatePath = AppConfiguration.TemplateFileName;
			csvPath = AppConfiguration.CsvDirectory;
			outputPath = AppConfiguration.OutputDirectory;

			templatePath = Path.GetFullPath(templatePath);
			csvPath = Path.GetFullPath(csvPath);
			outputPath = Path.GetFullPath(outputPath);
		}

		private static void CreateSpreadsheet(string csvFile, string outputFile)
		{
			Console.Write("Processing CSVFile: {0} Output to {1}... ", csvFile, outputFile);

			try
			{
				if (excelApp == null)
					excelApp = new Excel.Application();

				if (xlBook == null)
				{
					xlBook = excelApp.Workbooks.Open(
						templatePath,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value,
						Missing.Value);
				}

				DataTable dtCSV = CsvFileToDatatable(csvFile, true);

				foreach (DataRow dr in dtCSV.Rows)
				{
					if(! (string.IsNullOrEmpty(dr[0].ToString().Trim())
						|| string.IsNullOrEmpty(dr[1].ToString().Trim())
						)) {

						excelApp.Cells[startingWorksheetRow, 1] = dr[0].ToString();
						excelApp.Cells[startingWorksheetRow, 2] = dr[1].ToString();
						excelApp.Cells[startingWorksheetRow, 3] = dr[2].ToString();
						excelApp.Cells[startingWorksheetRow, 4] = dr[3].ToString();
						excelApp.Cells[startingWorksheetRow, 5] = dr[4].ToString();
						startingWorksheetRow++;
					}
				}

				if (xlSheet == null)
					if (xlBook.Worksheets != null) 
						xlSheet = (Excel.Worksheet)xlBook.Worksheets.Item[3];

				oFilename = outputFile;
				object oFileFormat = Excel.XlFileFormat.xlWorkbookDefault;
				object oPassword = Missing.Value;
				object oWriteResPassword = Missing.Value;
				object oReadOnlyRecommended = false;
				object oCreateBackup = false;

				const Excel.XlSaveAsAccessMode accessMode = Excel.XlSaveAsAccessMode.xlNoChange;
				object oConflictResolution = false;
				object oAddToMru = true;
				object oTextCodepage = Missing.Value;
				object oTextVisualLayout = Missing.Value;

				excelApp.DisplayAlerts = false;

				xlBook.RefreshAll();

				xlBook.SaveAs(oFilename, oFileFormat, oPassword, oWriteResPassword, oReadOnlyRecommended, oCreateBackup, accessMode, oConflictResolution, oAddToMru, oTextCodepage, oTextVisualLayout);
			}
			catch (Exception ex)
			{
				if(xlBook != null)
					xlBook.Close(false);
				//throw ex;
				Console.WriteLine("ERROR:");
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine("FINISHED");
		}
	}
}