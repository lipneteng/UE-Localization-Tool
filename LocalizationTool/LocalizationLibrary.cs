using CsvHelper;
using Microsoft.Win32;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace LocalizationTool
{
    public static class FileDialogFactory<T> where T : FileDialog, new()
    {
        public static T CreateFileDialog(string title, string extension)
        {
            return new T() { Title = title, Filter = string.Format("Data Files(*.{0}) | *.{0}", extension), DefaultExt = extension, AddExtension = true };
        }

        public static T CreateFileDialog(string title)
        {
            return new T() { Title = title };
        }
    }

    public static class FileHelper
    {
        public static BackgroundWorker Worker = new BackgroundWorker();

        public static bool OpenFile(Localization localization)
        {
            localization.ClearLocalization();
            bool IsValidFile = false;

            OpenFileDialog fileDialog = FileDialogFactory<OpenFileDialog>.CreateFileDialog("Select file for opening");

            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                string ext = Path.GetExtension(fileName);

                IsValidFile = true;

                switch (ext)
                {
                    case ".po":
                        ImportPoFile(localization, fileName);
                        break;

                    case ".json":
                        ImportJson(localization, fileName);
                        break;

                    case ".csv":
                        ImportCsv(localization, fileName);
                        break;

                    case ".xlsx":
                        ImportExcelFile(localization, fileName);
                        break;

                    default: IsValidFile = false;
                        break;
                }
            }

            return IsValidFile;
        } //deprecated

        public delegate void FileImported();
        public static event FileImported OnFileImported;

        private static void CreateWorker(Localization localization, string fileName, DoWorkEventHandler doWorkEventHandler)
        {
            List<object> paramsList = new List<object>();

            paramsList.Add(localization);
            paramsList.Add(fileName);

            Worker = new BackgroundWorker();
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += doWorkEventHandler;
            Worker.RunWorkerAsync(paramsList);
        }

        public static void ImportPoFileAsync(Localization localization, string fileName)
        {
            CreateWorker(localization, fileName, ImportPoFileWork);
        }

        private static void ImportPoFileWork(object sender, DoWorkEventArgs e)
        {
            List<object> paramsList = e.Argument as List<object>;
            ImportPoFile(paramsList[0] as Localization, paramsList[1] as string);
        }

        public static void ImportPoFile(Localization localization, string fileName)
        {
            string[] strings = File.ReadAllLines(fileName);
            PoFile poFile = new PoFile(strings);

            localization.PoFile = poFile;
            int fieldsCount = poFile.LocalizationFields.Count;

            for (int i = 0; i < fieldsCount; i++)
            {
                if (Worker != null && Worker.CancellationPending == true) break;
                else
                {
                    localization.AddFields(poFile.LocalizationFields[i]);
                    if (Worker != null) Worker.ReportProgress(Convert.ToInt32(((double)i / fieldsCount) * 100), i);
                }
            }
        }

        public static void ImportJson(Localization localization, string fileName)
        {
            string jsonString = File.ReadAllText(fileName);

            Localization outLocalization = new Localization();
            outLocalization = JsonConvert.DeserializeObject<Localization>(jsonString);

            localization.AddFields(outLocalization.LocalizationFields);

            OnFileImported();
        }

        public static void ImportCsv(Localization localization, string fileName)
        {
            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var fields = csv.GetRecords<LocalizationField>();

                foreach (var field in fields)
                {
                    localization.AddFields(field);
                }

                OnFileImported();
            }
        }
        public static void ImportExcelFileAsync(Localization localization, string fileName)
        {
            CreateWorker(localization, fileName, ImportExcelFileWork);
        }

        private static void ImportExcelFileWork(object sender, DoWorkEventArgs e)
        {
            List<object> paramsList = e.Argument as List<object>;
            ImportExcelFile(paramsList[0] as Localization, paramsList[1] as string);
        }

        public static void ImportExcelFile(Localization localization, string fileName)
        {
            FileInfo existingFile = new FileInfo(fileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.End.Row;     

                for (int i = 2; i <= rowCount; i++)
                {
                    if (Worker != null && Worker.CancellationPending == true) break;
                    else
                    {
                        var keyCell = worksheet.Cells[i, 1].Value;
                        if (keyCell == null) continue;
                        string Key = keyCell.ToString();

                        var sourceCell = worksheet.Cells[i, 2].Value;
                        string sourceString = worksheet.Cells[i, 2].Value != null ? sourceCell.ToString() : "";

                        var translationCell = worksheet.Cells[i, 3].Value;
                        string translationString = worksheet.Cells[i, 3].Value != null ? translationCell.ToString() : "";

                        string nameSpace = worksheet.Cells[i, 4].Value.ToString();
                        string sourceLocation = worksheet.Cells[i, 5].Value.ToString();

                        if (Worker != null) Worker.ReportProgress(Convert.ToInt32(((double)i / rowCount) * 100), i);
                        localization.AddFields(new LocalizationField(Key, sourceString, translationString, nameSpace, sourceLocation));
                    }
                }
            }
        }

        public static void ExportToPoFile(Localization localization, string fileName)
        {
            PoFile targetPoFile = null;

            if (File.Exists(fileName))
            {
                string[] strings = File.ReadAllLines(fileName);
                targetPoFile = new PoFile(strings);
            }

            else
            {
                targetPoFile = localization.PoFile; 
            }

            if (targetPoFile != null)
            {
                targetPoFile.ReplaceTranslationByLocalization(localization);
                File.WriteAllLines(fileName, targetPoFile.File);
            }
        }

        public static void ExportToJsonFile(Localization localization, string fileName)
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(localization, Formatting.Indented));
        }

        public static void ExportToCsvFile(Localization localization, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(localization.LocalizationFields);
            }
        }

        public static void ExportToExcelFile(Localization localization, string fileName)
        {
            FileInfo existingFile = new FileInfo(fileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                var workSheet = package.Workbook.Worksheets.Add("Localization");

                workSheet.Cells[1, 1].Value = "Key";
                workSheet.Cells[1, 2].Value = "SourceString";
                workSheet.Cells[1, 3].Value = "TranslationString";
                workSheet.Cells[1, 4].Value = "NameSpace";
                workSheet.Cells[1, 5].Value = "SourceLocation";

                int recordIndex = 2;

                foreach (LocalizationField field in localization.LocalizationFields)
                {
                    workSheet.Cells[recordIndex, 1].Value = field.Key;
                    workSheet.Cells[recordIndex, 2].Value = field.SourceString;
                    workSheet.Cells[recordIndex, 3].Value = field.TranslationString;
                    workSheet.Cells[recordIndex, 4].Value = field.NameSpace;
                    workSheet.Cells[recordIndex, 5].Value = field.SourceLocation;

                    recordIndex++;
                }

                package.SaveAs(existingFile);
            }
        }
    }
}
