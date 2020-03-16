using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace TestTask
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker BackgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            this.BackgroundWorker = new BackgroundWorker();
            this.BackgroundWorker.WorkerReportsProgress = true;
            this.BackgroundWorker.DoWork += BackgroundWorker_DoWork;
            this.BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            MessageBox.Show("Внимание! После нажатия кнопки \"Старт\"  или \"Загрузить всё\" проходит несколько секунд " +
                "прежде чем загрузка действительно начнется. В это время кнопки \"Стоп\" будут неотзывчивыми");
            this.BackgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) 
        {
            int percentageValue = 0;
            while (true)
            {
                if (LeftReusable.Downloader.ExpectedSize != 0
                || CenterReusable.Downloader.ExpectedSize != 0
                || RightReusable.Downloader.ExpectedSize != 0)
                {
                    percentageValue = Convert.ToInt32
                    ((LeftReusable.Downloader.CurrentSize
                    + CenterReusable.Downloader.CurrentSize
                    + RightReusable.Downloader.CurrentSize) 
                    * 100L
                    / (LeftReusable.Downloader.ExpectedSize
                    + CenterReusable.Downloader.ExpectedSize
                    + RightReusable.Downloader.ExpectedSize));
                    this.BackgroundWorker.ReportProgress(percentageValue);
                    Thread.Sleep(700);
                }
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) 
        {
            progressBar.Value = e.ProgressPercentage;
        }

        internal void PreventNoURLDownloadAll()
        {
            buttonDownloadAll.IsEnabled = LeftReusable.textBoxURL.Text.Length != 0 
                && CenterReusable.textBoxURL.Text.Length != 0 
                && RightReusable.textBoxURL.Text.Length != 0;
        }

        internal void PreventDownloadAllWhileDownloading() 
        {
           this.buttonDownloadAll.IsEnabled = LeftReusable.Downloader.State == Downloader.States.AwaitingDownload &&
           CenterReusable.Downloader.State == Downloader.States.AwaitingDownload &&
           RightReusable.Downloader.State == Downloader.States.AwaitingDownload;
        }

        private void buttonDownloadAll_Click(object sender, RoutedEventArgs e)
        {
            LeftReusable.StartDownload();
            CenterReusable.StartDownload();
            RightReusable.StartDownload();
        }
        //not awaiting async methods is SO wrong, but I don't think I can do anything about it
    }
}
