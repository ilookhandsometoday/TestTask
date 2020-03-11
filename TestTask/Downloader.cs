using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestTask
{
    public class Downloader: INotifyPropertyChanged //each image will have it's own dedicated downloader
    {
        private static HttpClient httpClient = new HttpClient();
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
            Downloading,
            Paused
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
            HttpResponseMessage headResponse = await Downloader.httpClient.SendAsync(head);
            HttpContent content = headResponse.Content;
            long? contentLength = content.Headers.ContentLength;
            return contentLength;
        }

        public async Task<string> GetExtension(string url)
        {
            HttpRequestMessage head = new HttpRequestMessage(HttpMethod.Head, url);
            HttpResponseMessage headResponse = await Downloader.httpClient.SendAsync(head);
            HttpContent content = headResponse.Content;
            string extension = Downloader.extensions[content.Headers.ContentType.MediaType];
            return extension;
        }

        public async Task<string> DownloadImage(string url, string fileName)
        {
            this.ExpectedSize = await this.GetContentLength(url);
            HttpRequestMessage get = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage getResponse = await Downloader.httpClient.SendAsync(get, HttpCompletionOption.ResponseHeadersRead);
            getResponse.EnsureSuccessStatusCode();
            this.Path = fileName + await this.GetExtension(url);
            using (Stream contentStream = await getResponse.Content.ReadAsStreamAsync(), fileStream = new FileStream(this.Path, FileMode.Create, FileAccess.Write, FileShare.None, 512, true))
            {
                long totalRead = 0L;
                bool isMoreToRead = true;
                int read = 0;
                byte[] buffer = new byte[512]; /*had an issue where with a larger buffer size(4096)
                the resulting image would have a lesser than expected size. Decreasing the buffer size seemed to fix the issue. 
                My guess is that reading from a stream took less time than getting the necessary bytes from the url.
                It's hard to tell what is the optimal size of the buffer, so I decided on the size of 512 bytes. 
                Some bugs may still occur if the file is very small.
                Doesn't seem to have a strong negative effect on performance judging by the completion time of unit tests
                , although that's very debatable.*/
                this.State = Downloader.States.Downloading;
                do
                {
                    read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (read != 0 && this.State != Downloader.States.Paused)
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;
                        this.CurrentSize = totalRead;
                    }
                    else if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else ;
                }
                while ((isMoreToRead && this.State == Downloader.States.Downloading) || this.State == Downloader.States.Paused);
            }

            this.State = Downloader.States.AwaitingDownload;

            return this.Path;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "") 
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
