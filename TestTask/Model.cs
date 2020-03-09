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
            using (Stream contentStream = await getResponse.Content.ReadAsStreamAsync(), fileStream = new FileStream(fileNameWExtension, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                long totalRead = 0L;
                byte[] buffer = new byte[4096];
                bool isMoreToRead = true;

                do
                {
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
