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

        private void PreventNoURLStart(TextBox textBox, Button startButton)
        {
            startButton.IsEnabled = textBox.Text.Length != 0;
        }

        private void PreventNoURLDownloadAll()
        {
            buttonDownloadAll.IsEnabled = textBoxURLLeft.Text.Length != 0 && textBoxURLCenter.Text.Length != 0 && textBoxURLRight.Text.Length != 0;
        }

        private async Task StartDownload(Image image, Button startButton, Button stopButton, TextBox textBox, string fileName,Downloader downloader)
        {

            image.Source = null;
            buttonDownloadAll.IsEnabled = false;
            stopButton.IsEnabled = true;
            startButton.IsEnabled = false;
            try
            {
                image.Source = await downloader.DownloadImage(textBox.Text, fileName);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                image.Source = null;
            }

            stopButton.IsEnabled = false;
            startButton.IsEnabled = true;
            buttonDownloadAll.IsEnabled = LeftDownloader.State == Downloader.States.AwaitingDownload &&
            CenterDownloader.State == Downloader.States.AwaitingDownload &&
            RightDownloader.State == Downloader.States.AwaitingDownload;
            PreventNoURLStart(textBox, startButton );
            PreventNoURLDownloadAll();
        }

        private void textBoxURLLeft_TextChanged(object sender, TextChangedEventArgs e)
        {
            PreventNoURLStart(textBoxURLLeft, buttonStartLeft);
            PreventNoURLDownloadAll();
        }

        private void textBoxURLCenter_TextChanged(object sender, TextChangedEventArgs e)
        {
            PreventNoURLStart(textBoxURLCenter, buttonStartCenter);
            PreventNoURLDownloadAll();
        }

        private void textBoxURLRight_TextChanged(object sender, TextChangedEventArgs e)
        {
            PreventNoURLStart(textBoxURLRight, buttonStartRight);
            PreventNoURLDownloadAll();
        }

        private void buttonStartLeft_Click(object sender, RoutedEventArgs e)
        {
            StartDownload(imageLeft, buttonStartLeft, buttonStopLeft, textBoxURLLeft, "Left",this.LeftDownloader);
        }

        private void buttonStartCenter_Click(object sender, RoutedEventArgs e)
        {
            StartDownload(imageCenter, buttonStartCenter, buttonStopCenter, textBoxURLCenter, "Center", this.CenterDownloader);
        }

        private void buttonStartRight_Click(object sender, RoutedEventArgs e)
        {
            StartDownload(imageRight, buttonStartRight, buttonStopRight, textBoxURLRight, "Right", this.RightDownloader);
        }

        private void buttonDownloadAll_Click(object sender, RoutedEventArgs e)
        {
            StartDownload(imageLeft, buttonStartLeft, buttonStopLeft, textBoxURLLeft, "Left", this.LeftDownloader);
            StartDownload(imageCenter, buttonStartCenter, buttonStopCenter, textBoxURLCenter, "Center", this.CenterDownloader);
            StartDownload(imageRight, buttonStartRight, buttonStopRight, textBoxURLRight, "Right", this.RightDownloader);
        }
        //not awaiting async methods is SO wrong, but I don't think I can do anything about it

        private void buttonStopLeft_Click(object sender, RoutedEventArgs e)
        {
            LeftDownloader.StopDownload();
        }

        private void buttonStopCenter_Click(object sender, RoutedEventArgs e)
        {
            CenterDownloader.StopDownload();
        }

        private void buttonStopRight_Click(object sender, RoutedEventArgs e)
        {
            RightDownloader.StopDownload();
        }
    }
}
