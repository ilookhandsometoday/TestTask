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
    public class Downloader
    {
        private HttpClient httpClient;

        private Dictionary<string, string> extensions;

        public Downloader()
        {
            this.httpClient = new HttpClient();
            this.extensions = new Dictionary<string, string>
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
            string extension = this.extensions[content.Headers.ContentType.MediaType];
            return extension;
        }

        public async Task<string> DownloadImage(string url, string fileName) 
        {
            HttpRequestMessage get = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage getResponse = await this.httpClient.SendAsync(get, HttpCompletionOption.ResponseHeadersRead);
            getResponse.EnsureSuccessStatusCode();
            string fileNameWExtension = fileName + await this.GetExtension(url);
            using (Stream contentStream = await getResponse.Content.ReadAsStreamAsync(), fileStream = new FileStream(fileNameWExtension, FileMode.Create, FileAccess.Write, FileShare.None, 1024, true))
            {
                long totalRead = 0L;
                bool isMoreToRead = true;

                do
                {
                    byte[] buffer = new byte[1]; /*had an issue where with a larger buffer size(4096)
                the resulting image would have a lesser than expected size. Decreasing the buffer size seemed to fix the issue. 
                My guess is that reading from a stream took less time than getting the necessary bytes from the url.
                It's hard to tell what is the optimal size of the buffer, so I decided on the lowest size possible.
                Doesn't seem to have a negative effect on performance judging by the completion time of unit tests.*/
                    int read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;
                    }
                }
                while (isMoreToRead);
            }

            return fileNameWExtension;
            
        }
    }
}
