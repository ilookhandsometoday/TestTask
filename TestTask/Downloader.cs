using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Drawing;
using System.IO;
using System.Web;

namespace TestTask
{
    public class Downloader //each image will have it's own dedicated downloader
    {
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
        private string path;
        private long? expectedSize;
        private long? currentSize;
        private bool finishedDownloading;

        public Downloader()
        {
            this.path = "";
            this.expectedSize = 0;
            this.currentSize = 0;
            this.finishedDownloading = false;

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

        public bool FinishedDownloading
        {
            get { return this.finishedDownloading; }
            set { this.finishedDownloading = value; }
        }

        public long? CurrentSize
        {
            get { return this.currentSize; }
            set { this.currentSize = value; }
        }

        public async Task<long?> GetContentLength(string url, HttpClient httpClient)
        {
            HttpRequestMessage head = new HttpRequestMessage(HttpMethod.Head, url);
            HttpResponseMessage headResponse = await httpClient.SendAsync(head);
            HttpContent content = headResponse.Content;
            long? contentLength = content.Headers.ContentLength;
            return contentLength;
        }

        public async Task<string> GetExtension(string url, HttpClient httpClient)
        {
            HttpRequestMessage head = new HttpRequestMessage(HttpMethod.Head, url);
            HttpResponseMessage headResponse = await httpClient.SendAsync(head);
            HttpContent content = headResponse.Content;
            string extension = Downloader.extensions[content.Headers.ContentType.MediaType];
            return extension;
        }

        public async Task<string> DownloadImage(string url, string fileName, HttpClient httpClient)
        {
            this.ExpectedSize = await this.GetContentLength(url, httpClient);
            HttpRequestMessage get = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage getResponse = await httpClient.SendAsync(get, HttpCompletionOption.ResponseHeadersRead);
            getResponse.EnsureSuccessStatusCode();
            this.Path = fileName + await this.GetExtension(url, httpClient);
            using (Stream contentStream = await getResponse.Content.ReadAsStreamAsync(), fileStream = new FileStream(this.Path, FileMode.Create, FileAccess.Write, FileShare.None, 512, true))
            {
                long totalRead = 0L;
                bool isMoreToRead = true;

                do
                {
                    byte[] buffer = new byte[512]; /*had an issue where with a larger buffer size(4096)
                the resulting image would have a lesser than expected size. Decreasing the buffer size seemed to fix the issue. 
                My guess is that reading from a stream took less time than getting the necessary bytes from the url.
                It's hard to tell what is the optimal size of the buffer, so I decided on the size of 512 bytes. 
                Some bugs may still occur if the file is very small.
                Doesn't seem to have a strong negative effect on performance judging by the completion time of unit tests
                , although that's very debatable.*/
                    int read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;
                        this.CurrentSize = totalRead;
                    }
                }
                while (isMoreToRead);
            }

            return this.Path;
        }
    }
}
