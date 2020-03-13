using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Media.Imaging;
using System.IO;

namespace TestTask
{
    public class Downloader
    {
        private HttpClient httpClient;
        private static Dictionary<string, string> extensions = new Dictionary<string, string>
            {
                {"image/jpeg", ".jpg" },
                {"image/bmp", ".bmp" },
                {"image/gif", ".gif"},
                {"image/vnd.microsoft.icon", ".ico"},
                {"image/png", ".png" },
                {"image/svg+xml", ".svg"},
                {"image/tiff", ".tiff"},
                {"image/webp", ".webp"}
            };

        public enum States 
        {
            AwaitingDownload,
            Downloading
        }
        private string path;
        private long? expectedSize;
        private long? currentSize;
        private States state;

        public Downloader()
        {
            this.path = "";
            this.expectedSize = 0;
            this.currentSize = 0;
            this.state = Downloader.States.AwaitingDownload;
            this.httpClient = new HttpClient();
        }

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public long? ExpectedSize
        {
            get { return this.expectedSize; }
            set { this.expectedSize = value; }
        }

        public long? CurrentSize
        {
            get { return this.currentSize; }
            set { this.currentSize = value; }
        }

        public States State 
        {
            get { return this.state; }
            set { this.state = value; } 
        }

        public async Task<long?> GetContentLength(string url)
        {
            HttpRequestMessage head = new HttpRequestMessage(HttpMethod.Head, url);
            HttpResponseMessage headResponse = await this.httpClient.SendAsync(head);
            HttpContent content = headResponse.Content;
            long? contentLength = content.Headers.ContentLength;
            return contentLength;
        }

        public async Task<string> GetExtension(string url)
        {
            HttpRequestMessage head = new HttpRequestMessage(HttpMethod.Head, url);
            HttpResponseMessage headResponse = await this.httpClient.SendAsync(head);
            HttpContent content = headResponse.Content;
            string extension = Downloader.extensions[content.Headers.ContentType.MediaType];
            return extension;
        }

        public async Task<BitmapImage> DownloadImage(string url, string fileName)//it takes a few seconds before the download ACTUALLY starts
        {
            this.ExpectedSize = await this.GetContentLength(url);
            HttpRequestMessage get = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage getResponse = await this.httpClient.SendAsync(get, HttpCompletionOption.ResponseHeadersRead);
            getResponse.EnsureSuccessStatusCode();
            this.Path = fileName + await this.GetExtension(url);
            int bufferSize = 1024;
            using (Stream contentStream = await getResponse.Content.ReadAsStreamAsync())
            using (FileStream fileStream = new FileStream(this.Path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
            {
                long totalRead = 0L;
                bool isMoreToRead = true;
                int read = 0;
                byte[] buffer = new byte[bufferSize]; /*had an issue where with a larger buffer size(4096)
                the resulting image would have a lesser than expected size. Decreasing the buffer size seemed to fix the issue. 
                My guess is that reading from a stream took less time than getting the necessary bytes from the url.
                It's hard to tell what is the optimal size of the buffer, so I decided on the size of 1024 bytes. 
                Something bad still might happen if the file is too small*/
                this.State = Downloader.States.Downloading;
                do
                {
                    read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (read != 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;
                        this.CurrentSize = totalRead;
                    }
                    else
                    {
                        isMoreToRead = false;
                    }
                }
                while (isMoreToRead && this.State == Downloader.States.Downloading);
            }

            if (this.State == Downloader.States.Downloading)
            {
                
                BitmapImage resultingImage = new BitmapImage();
                this.Path = Directory.GetCurrentDirectory() + @"\" + this.Path;
                resultingImage.BeginInit();
                resultingImage.CacheOption = BitmapCacheOption.OnLoad;
                using (FileStream stream = File.OpenRead(this.Path))
                {
                    resultingImage.StreamSource = stream;
                    resultingImage.EndInit();
                }

                resultingImage.Freeze();
                this.State = Downloader.States.AwaitingDownload;
                return resultingImage;
            }

            return null;
        }

        public void StopDownload() 
        {
            this.State = Downloader.States.AwaitingDownload;
        }
    }
}
