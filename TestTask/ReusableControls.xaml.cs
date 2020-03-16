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

namespace TestTask
{
    /// <summary>
    /// Логика взаимодействия для ReusableControls.xaml
    /// </summary>
    public partial class ReusableControls : UserControl
    {
        internal Downloader Downloader;
        private MainWindow ParentWindow;
        public ReusableControls()
        {
            InitializeComponent();
            this.Downloader = new Downloader();
            this.ParentWindow = (MainWindow)Window.GetWindow(this);
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

        private async Task StartDownload(string fileName)
        {

            this.image.Source = null;
            this.ParentWindow.buttonDownloadAll.IsEnabled = false;
            this.buttonStop.IsEnabled = true;
            this.buttonStart.IsEnabled = false;
            try
            {
                this.image.Source = await this.Downloader.DownloadImage(this.textBoxURL.Text, fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                this.image.Source = null;
            }

            this.buttonStop.IsEnabled = false;
            this.buttonStart.IsEnabled = true;
            PreventNoURLStart();
            this.ParentWindow.PreventDownloadAllWhileDownloading();
            this.ParentWindow.PreventNoURLDownloadAll();
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            this.Downloader.StopDownload();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            this.StartDownload(this.Name);
        }
    }
}
