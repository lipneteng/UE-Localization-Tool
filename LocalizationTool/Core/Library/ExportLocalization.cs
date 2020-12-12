using CsvHelper;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Globalization;
using System.IO;

namespace LocalizationTool
{
    public static class ExportLocalization
    {
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
                if (localization.PoFilesMap.Count > 0)
                {
                    targetPoFile = localization.GetCurrentPoFile();
                }
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
