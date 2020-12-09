using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LocalizationTool
{
    /// <summary>
    /// Interaction logic for ExcelWorkWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {

        public BackgroundWorker worker = null;

        public ProgressWindow()
        {
            InitializeComponent();
        }

        public void InitProgressWindow(BackgroundWorker worker)
        {
            this.worker = worker;
            this.worker.ProgressChanged += worker_ProgressChanged;
            this.worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (Visibility != Visibility.Visible) ShowDialog();

            ItemsCountText.Text = e.UserState.ToString();
            ExcelProcessBar.Value = e.ProgressPercentage;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.worker.CancelAsync();
        }
    }
}
