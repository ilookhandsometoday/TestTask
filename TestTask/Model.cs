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
            //exceptions will be handled globally???
        }
    }
}
