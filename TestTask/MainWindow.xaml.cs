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
        private Downloader LeftDownloader;
        private Downloader CenterDownloader;
        private Downloader RightDownloader;
        private BackgroundWorker BackgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            this.LeftDownloader = new Downloader();
            this.CenterDownloader = new Downloader();
            this.RightDownloader = new Downloader();
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
                if (LeftDownloader.ExpectedSize != 0
                || CenterDownloader.ExpectedSize != 0
                || RightDownloader.ExpectedSize != 0)
                {
                    percentageValue = Convert.ToInt32
                    ((LeftDownloader.CurrentSize
                    + CenterDownloader.CurrentSize
                    + RightDownloader.CurrentSize) 
                    * 100L
                    / (LeftDownloader.ExpectedSize
                    + CenterDownloader.ExpectedSize
                    + RightDownloader.ExpectedSize));
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
           RightDownloader.State == Downloader.States.AwaitingDownload;
        }

        private void buttonDownloadAll_Click(object sender, RoutedEventArgs e)
        {
            StartDownload(imageLeft, buttonStartLeft, buttonStopLeft, textBoxURLLeft, "Left", this.LeftDownloader);
            StartDownload(imageCenter, buttonStartCenter, buttonStopCenter, textBoxURLCenter, "Center", this.CenterDownloader);
            StartDownload(imageRight, buttonStartRight, buttonStopRight, textBoxURLRight, "Right", this.RightDownloader);
        }
        //not awaiting async methods is SO wrong, but I don't think I can do anything about it
    }
}
