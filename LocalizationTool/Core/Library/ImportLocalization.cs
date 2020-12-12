using CsvHelper;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace LocalizationTool
{
    public static class ImportLocalization
    {
        public delegate void FileImported();
        public static event FileImported OnFileImported;

        public static BackgroundWorker ImportHandler = null;

        public static void ImportFileAsync(Localization localization, string fileName, DoWorkEventHandler importMethod)
        {
            List<object> paramsList = new List<object>();

            paramsList.Add(localization);
            paramsList.Add(fileName);

            ImportHandler = new BackgroundWorker();
            ImportHandler.WorkerReportsProgress = true;
            ImportHandler.WorkerSupportsCancellation = true;
            ImportHandler.DoWork += importMethod;
            ImportHandler.RunWorkerAsync(paramsList);
        }

        public static void ImportPoFileAsync(Localization localization, string fileName)
        {
            ImportFileAsync(localization, fileName, ImportPoFile);
        }

        private static void ImportPoFile(object sender, DoWorkEventArgs e)
        {
            List<object> paramsList = e.Argument as List<object>;

            Localization localization = paramsList[0] as Localization;
            string fileName = paramsList[1] as string;

            string[] strings = File.ReadAllLines(fileName);
            PoFile poFile = new PoFile(strings);
            string poFileLanguage = poFile.PoFileLanguage;

            localization.AddPoFile(poFile);

            if (!localization.IsNewPoFileLangugage(poFileLanguage))
            {
                int fieldsCount = poFile.LocalizationFields.Count;

                for (int i = 0; i < fieldsCount; i++)
                {
                    if (ImportHandler != null && ImportHandler.CancellationPending == true) break;
                    else
                    {
                        localization.AddFields(poFile.LocalizationFields[i]);
                        if (ImportHandler != null) ImportHandler.ReportProgress(Convert.ToInt32(((double)i / fieldsCount) * 100), i);
                    }
                }
            }
        }

        public static void ImportJson(Localization localization, string fileName)
        {
            string jsonString = File.ReadAllText(fileName);

            Localization outLocalization = new Localization();
            outLocalization = JsonConvert.DeserializeObject<Localization>(jsonString);

            localization.AddFields(outLocalization.LocalizationFields);

            PoFile poFile = localization.GetCurrentPoFile();

            string field = poFile.LocalizationFields[1].TranslationString;

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
            ImportFileAsync(localization, fileName, ImportExcelFile);
        }

        private static void ImportExcelFile(object sender, DoWorkEventArgs e)
        {
            List<object> paramsList = e.Argument as List<object>;
            Localization localization = paramsList[0] as Localization;
            string fileName = paramsList[1] as string;


            FileInfo existingFile = new FileInfo(fileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.End.Row;

                for (int i = 2; i <= rowCount; i++)
                {
                    if (ImportHandler != null && ImportHandler.CancellationPending == true) break;
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

                        if (ImportHandler != null) ImportHandler.ReportProgress(Convert.ToInt32(((double)i / rowCount) * 100), i);
                        localization.AddFields(new LocalizationField(Key, sourceString, translationString, nameSpace, sourceLocation));
                    }
                }
            }
        }
    }
}
