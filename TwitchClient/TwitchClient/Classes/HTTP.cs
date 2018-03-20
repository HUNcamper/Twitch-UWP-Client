using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TwitchClient
{
    /// <summary>
    /// Class for handling HTTP requests
    /// </summary>
    class HTTP
    {
        // Http Client
        private HttpClient client;

        public HttpClient Client
        {
            get { return client; }
            set { client = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="httpclient">Http Client</param>
        public HTTP()
        {
            // Don't use proxy, it just slows requests
            HttpClientHandler hch = new HttpClientHandler();
            hch.Proxy = null;
            hch.UseProxy = false;

            client = new HttpClient(hch);
        }

        /// <summary>
        /// Send GET request.
        /// </summary>
        /// <param name="client">Http Client</param>
        /// <param name="url">URI to send the request to</param>
        /// <returns>requested URI's response</returns>
        public async Task<string> Get(string url)
        {
            Debug.WriteLine("[HTTP] GETTING {0}", url);
            using (var r = await Client.GetAsync(new Uri(url)))
            {
                string result = await r.Content.ReadAsStringAsync();
                Debug.WriteLine("[HTTP] COMPLETED");
                return result;
            }
        }
    }
}
