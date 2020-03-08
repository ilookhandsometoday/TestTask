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
    class Model
    {
        private HttpClient httpClient;
        public async void DownloadImage(string url, string fileName) 
        {
            HttpRequestMessage get = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = await this.httpClient.SendAsync(get, HttpCompletionOption.ResponseHeadersRead);
            using (Stream stream = await response.Content.ReadAsStreamAsync()) 
            {
                stream.Length;
            };
        }
    }
}
