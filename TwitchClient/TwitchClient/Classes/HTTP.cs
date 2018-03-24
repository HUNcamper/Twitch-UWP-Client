using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TwitchClient.Classes
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
        public HTTP()
        {
			// Don't use proxy, it just slows requests
			HttpClientHandler hch = new HttpClientHandler
			{
				Proxy = null,
				UseProxy = false
			};

			client = new HttpClient(hch);
        }

		/// <summary>
		/// Disposes the HttpClient. After firing this method, the class becomes unusable.
		/// </summary>
		public void Dispose()
		{
			client.Dispose();
		}

		/// <summary>
		/// Send GET request to the given URL.
		/// </summary>
		/// <param name="url">URL to send the request to</param>
		/// <returns>requested URL's response body</returns>
		public async Task<string> Get(string url)
        {
            Debug.WriteLine(String.Format("[HTTP] GETTING {0}", url));
            using (var r = await Client.GetAsync(new Uri(url)))
            {
				r.EnsureSuccessStatusCode();
				Debug.WriteLine("[HTTP] GET COMPLETED");
				return await r.Content.ReadAsStringAsync();
			}
        }

		/// <summary>
		/// Send a POST request to the given URL with the given parameters.
		/// </summary>
		/// <param name="url">URL to send the request to</param>
		/// <param name="parameters">Dictionary consisting of string, string pairs</param>
		/// <returns>requested URL's response body</returns>
		public async Task<string> Post(string url, Dictionary<string, string> parameters)
		{
			Debug.WriteLine(String.Format("[HTTP] POSTING {0}", url));
			var encodedContent = new FormUrlEncodedContent(parameters);

			using (var r = await Client.PostAsync(url, encodedContent))
			{
				r.EnsureSuccessStatusCode();
				Debug.WriteLine("[HTTP] POST COMPLETED");
				return await r.Content.ReadAsStringAsync();
			}
		}
    }
}
