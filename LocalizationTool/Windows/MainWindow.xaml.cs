using Microsoft.Win32;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LocalizationTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public static Dictionary<LocalizationFieldStatus, Color> ColorMap = new Dictionary<LocalizationFieldStatus, Color>
            {
                {LocalizationFieldStatus.Idle, Color.FromRgb(245,245,245)},
                {LocalizationFieldStatus.New, Color.FromRgb(144,238,144)},
                {LocalizationFieldStatus.Changed, Color.FromRgb(173,255,47)},
                {LocalizationFieldStatus.NoTranslation, Color.FromRgb(219,112,147)}
            };

        public Localization LocalizationData = new Localization();

        public MainWindow()
        {
            InitializeComponent();

            LocalizationGrid.LoadingRow += LocalizationGridAddingNewItem;
            LocalizationData.OnLocalizationUpdated += UpdateLocalizationGrid;

            ImportLocalization.OnFileImported += UpdateLocalizationGrid;
        }

        private void LocalizationGridAddingNewItem(object sender, DataGridRowEventArgs e)
        {
            LocalizationField field = e.Row.Item as LocalizationField;

            if (field != null)
            {
                LocalizationFieldStatus status = LocalizationData.LocalizationFields.GetFieldStatusByKey(field.Key);
                Color color = ColorMap[status];
                e.Row.Background = new SolidColorBrush(color);
            }
        }

        public void UpdateLocalizationGrid()
        {
            LocalizationGrid.ItemsSource = null;
            LocalizationGrid.ItemsSource = LocalizationData.LocalizationFields;

            for (int i = 0; i < LocalizationGrid.Columns.Count; i++)
            {
                if (i != 2) LocalizationGrid.Columns[i].IsReadOnly = true; // allow edit only translation string
            }
            
            var languageList = LocalizationData.PoFilesMap.Keys;

            if (languageList.Count > 0)
            {
                PoFilesBox.IsEnabled = true;
                PoFilesBox.ItemsSource = null;
                PoFilesBox.ItemsSource = languageList;
                PoFilesBox.SelectedItem = LocalizationData.CurrentPoFileLanguage;
            }
            else PoFilesBox.IsEnabled = false;

        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LocalizationData.ClearLocalization();
            bool isValidFile = true;

            OpenFileDialog fileDialog = FileDialogFactory<OpenFileDialog>.CreateFileDialog("Select file for opening");

            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                string ext = Path.GetExtension(fileName);

                switch (ext)
                {
                    case ".po":
                        {
                            ImportLocalization.ImportPoFileAsync(LocalizationData, fileName);
                            CreateProgressWindow();
                        }
                        break;

                    case ".json":
                        ImportLocalization.ImportJson(LocalizationData, fileName);
                        break;

                    case ".csv":
                        ImportLocalization.ImportCsv(LocalizationData, fileName);
                        break;

                    case ".xlsx":
                        {
                            ImportLocalization.ImportExcelFileAsync(LocalizationData, fileName);
                            CreateProgressWindow();
                        }
                        break;

                    default:
                        isValidFile = false;
                        break;
                }
            }
            else isValidFile = false;

            if (!isValidFile) MessageBox.Show("No valid file or extension is not supported!");
        }

        private void ClearLocMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LocalizationData.ClearLocalization();
        }

        private void CreateProgressWindow()
        {
            ImportLocalization.ImportHandler.RunWorkerCompleted += WorkerCompleted;

            ProgressWindow progressWindow = new ProgressWindow();
            progressWindow.Owner = this;
            progressWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            progressWindow.InitProgressWindow(ImportLocalization.ImportHandler);
        }

        void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateLocalizationGrid();
        }

        private void ImportPoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = FileDialogFactory<OpenFileDialog>.CreateFileDialog("Select .po file for import", "po");

            if (fileDialog.ShowDialog() == true)
            {
                ImportLocalization.ImportPoFileAsync(LocalizationData, fileDialog.FileName);
                CreateProgressWindow();
            }
        }

        private void ImportJsonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = FileDialogFactory<OpenFileDialog>.CreateFileDialog("Select .json file for import", "json");

            if (fileDialog.ShowDialog() == true)
            {
                ImportLocalization.ImportJson(LocalizationData, fileDialog.FileName);
            }
        }

        private void ImportCsvMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = FileDialogFactory<OpenFileDialog>.CreateFileDialog("Select .csv file for import", "csv");

            if (fileDialog.ShowDialog() == true)
            {
                ImportLocalization.ImportCsv(LocalizationData, fileDialog.FileName);
            }
        }

        private void ImportExcelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = FileDialogFactory<OpenFileDialog>.CreateFileDialog("Select excel file for import", "xlsx");

            if (fileDialog.ShowDialog() == true)
            {
                ImportLocalization.ImportExcelFileAsync(LocalizationData, fileDialog.FileName);
                CreateProgressWindow();
            }
        }

        private void ExportLocToPoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = FileDialogFactory<SaveFileDialog>.CreateFileDialog("Select .po file for export", "po");

            if (fileDialog.ShowDialog() == true)
            {
                ExportLocalization.ExportToPoFile(LocalizationData, fileDialog.FileName);
            }
        }

        private void ExportLocToJsonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = FileDialogFactory<SaveFileDialog>.CreateFileDialog("Select .json file for export", "json");

            if (fileDialog.ShowDialog() == true)
            {
                ExportLocalization.ExportToJsonFile(LocalizationData, fileDialog.FileName);
            }
        }

        private void ExportLocToCsvMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = FileDialogFactory<SaveFileDialog>.CreateFileDialog("Select .csv file for export", "csv");

            if (fileDialog.ShowDialog() == true)
            {
                ExportLocalization.ExportToCsvFile(LocalizationData, fileDialog.FileName);
            }
        }

        private void ExportLocToExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = FileDialogFactory<SaveFileDialog>.CreateFileDialog("Select .xlsx file for export", "xlsx");

            if (fileDialog.ShowDialog() == true)
            {
                ExportLocalization.ExportToExcelFile(LocalizationData, fileDialog.FileName);
            }
        }

        private void FastMergePoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImportPoMenuItem_Click(sender, e);
            ImportPoMenuItem_Click(sender, e);

            ExportLocToPoMenuItem_Click(sender, e);
        }

        private void ShrinkNewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LocalizationData.ShrinkLocalization(LocalizationFieldStatus.New);
        }

        private void ShrinkChangedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LocalizationData.ShrinkLocalization(LocalizationFieldStatus.Changed);
        }

        private void ShrinkNoTranslationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LocalizationData.ShrinkLocalization(LocalizationFieldStatus.NoTranslation);
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PoFilesBox_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            string selectedLanguage = comboBox.SelectedItem.ToString();
            if (selectedLanguage != LocalizationData.CurrentPoFileLanguage)
            {
                LocalizationData.SetLocalizationByPoFile((sender as ComboBox).SelectedItem.ToString());
            }
        }
    }
}

