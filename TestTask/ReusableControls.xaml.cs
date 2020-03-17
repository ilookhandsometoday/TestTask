using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace TestTask
{
    /// <summary>
    /// Логика взаимодействия для ReusableControls.xaml
    /// </summary>
    public partial class ReusableControls : UserControl
    {
        internal Downloader Downloader;
        private MainWindow ParentWindow;
        private BackgroundWorker BackgroundWorker;
        public ReusableControls()
        {
            InitializeComponent();
            this.Downloader = new Downloader();
            this.Loaded += OnLoaded;
            this.BackgroundWorker = new BackgroundWorker();
            this.BackgroundWorker.DoWork += BackgroundWorker_DoWork;
            this.BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) 
        {
            string url = null;
            string fileName = null;
            this.Dispatcher.Invoke(() => url = this.textBoxURL.Text);
            this.Dispatcher.Invoke(() => fileName = this.Name);
            Task<BitmapImage> downloadImageTask = this.Downloader.DownloadImage(url, fileName);
            while (this.Downloader.State == Downloader.States.AwaitingDownload) ;
            this.Dispatcher.Invoke(() => this.buttonStop.IsEnabled = true);
            BitmapImage imageForDisplay = downloadImageTask.Result;
            this.Dispatcher.Invoke(() => this.image.Source = imageForDisplay);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) 
        {
            this.Dispatcher.Invoke(() => this.buttonStop.IsEnabled = false);
            this.Dispatcher.Invoke(() => this.buttonStart.IsEnabled = true);
            this.PreventNoURLStart();
            this.ParentWindow.PreventNoURLDownloadAll();
            this.ParentWindow.PreventDownloadAllWhileDownloading();
        }

        private void OnLoaded(object sender, RoutedEventArgs e) 
        {
            this.ParentWindow = Window.GetWindow(this) as MainWindow;
            this.Loaded -= OnLoaded;
        }

        private void PreventNoURLStart() 
        {
            this.buttonStart.IsEnabled = this.textBoxURL.Text.Length != 0;
        }

        private void textBoxURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            PreventNoURLStart();
            this.ParentWindow.PreventNoURLDownloadAll();
        }

        internal void StartDownload()
        {

            this.image.Source = null;
            this.ParentWindow.buttonDownloadAll.IsEnabled = false;
            this.buttonStart.IsEnabled = false;
            try
            {
                this.BackgroundWorker.RunWorkerAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                this.image.Source = null;
            }
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            this.Downloader.StopDownload();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            StartDownload();
        }
    }
}
