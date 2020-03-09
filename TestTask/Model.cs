using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Drawing;
using System.IO;

namespace TestTask
{
    public class Model
    {
        private HttpClient httpClient;

        public Model() 
        {
            this.httpClient = new HttpClient();
        }
        public long? GetContentLength(string url)
        {
            HttpRequestMessage head = new HttpRequestMessage(HttpMethod.Head, url);
            Task<HttpResponseMessage> headResponse = this.httpClient.SendAsync(head);
            HttpContent content = headResponse.Result.Content;
            long? contentLength = content.Headers.ContentLength;
            return contentLength;
        }

        public string GetExtension(string url) 
        {
            return "." + url.Split(new char[] { '.' }).Last();
        }

        public async void DownloadImage(string url, string fileName) 
        {
            HttpRequestMessage get = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage getResponse = this.httpClient.SendAsync(get, HttpCompletionOption.ResponseHeadersRead).Result;
            getResponse.EnsureSuccessStatusCode();
            string fileNameWExtension = fileName + this.GetExtension(url);
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
            
        }
    }
}
