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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Downloader LeftDownloader;
        private Downloader CenterDownloader;
        private Downloader RightDownloader;

        public MainWindow()
        {
            InitializeComponent();
            this.LeftDownloader = new Downloader();
            this.CenterDownloader = new Downloader();
            this.RightDownloader = new Downloader();
        }

        public enum Position
        {
            Left,
            Center,
            Right
        }

        public void PreventNoURLStart(Position position) 
        {
            Button buttonWildCard = new Button();
            TextBox textBoxWildCard = new TextBox();
            switch (position) 
            {
                case Position.Left:
                    textBoxWildCard = textBoxURLLeft;
                    buttonWildCard = buttonStartLeft;
                    break;
                case Position.Center:
                    textBoxWildCard = textBoxURLCenter;
                    buttonWildCard = buttonStartCenter;
                    break;
                case Position.Right:
                    textBoxWildCard = textBoxURLRight;
                    buttonWildCard = buttonStartRight;
                    break;
            }

            if (textBoxWildCard.Text.Length != 0)
            {
                buttonWildCard.IsEnabled = true;
            }
            else
            {
                buttonWildCard.IsEnabled = false;
            }
        }

        public void PreventNoURLDownloadAll()
        {
            if (textBoxURLLeft.Text.Length != 0 && textBoxURLCenter.Text.Length != 0 && textBoxURLRight.Text.Length != 0)
            {
                buttonDownloadAll.IsEnabled = true;
            }
            else 
            {
                buttonDownloadAll.IsEnabled = false;
            }
        }

        public async Task StartDownload(Position position, string url, Downloader downloader) 
        {
            Image imageWildCard = new Image();
            Button stopButtonWildCard = new Button();
            Button startButtonWildCard = new Button();
            string fileName = "";
            switch (position)
            {
                case Position.Left:
                    imageWildCard = imageLeft;
                    stopButtonWildCard = buttonStopLeft;
                    startButtonWildCard = buttonStartLeft;
                    fileName = "Left";
                    break;
                case Position.Center:
                    imageWildCard = imageCenter;
                    stopButtonWildCard = buttonStopCenter;
                    startButtonWildCard = buttonStartCenter;
                    fileName = "Center";
                    break;
                case Position.Right:
                    imageWildCard = imageRight;
                    stopButtonWildCard = buttonStopRight;
                    startButtonWildCard = buttonStartRight;
                    fileName = "Right";
                    break;
            }
            stopButtonWildCard.IsEnabled = true;
            startButtonWildCard.IsEnabled = false;
            imageWildCard.Source = await downloader.DownloadImage(url, fileName);
        }

        private void textBoxURLLeft_TextChanged(object sender, TextChangedEventArgs e)
        {
            PreventNoURLStart(Position.Left);
            PreventNoURLDownloadAll();
        }

        private void textBoxURLCenter_TextChanged(object sender, TextChangedEventArgs e)
        {
            PreventNoURLStart(Position.Center);
            PreventNoURLDownloadAll();
        }

        private void textBoxURLRight_TextChanged(object sender, TextChangedEventArgs e)
        {
            PreventNoURLStart(Position.Right);
            PreventNoURLDownloadAll();
        }

        private async void buttonStartLeft_Click(object sender, RoutedEventArgs e)
        {
            await StartDownload(Position.Left, textBoxURLLeft.Text, this.LeftDownloader);
        }

        private async void buttonStartCenter_Click(object sender, RoutedEventArgs e)
        {
            await StartDownload(Position.Center, textBoxURLCenter.Text, this.CenterDownloader);
        }

        private async void buttonStartRight_Click(object sender, RoutedEventArgs e)
        {
            await StartDownload(Position.Right, textBoxURLRight.Text, this.RightDownloader);
        }

        private async void buttonDownloadAll_Click(object sender, RoutedEventArgs e)
        {
            StartDownload(Position.Left, textBoxURLLeft.Text, this.LeftDownloader);
            StartDownload(Position.Center, textBoxURLCenter.Text, this.CenterDownloader);
            StartDownload(Position.Right, textBoxURLRight.Text, this.RightDownloader);
        }
    }
}
