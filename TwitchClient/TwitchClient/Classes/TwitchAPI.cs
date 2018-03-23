using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchClient.Classes;
using Newtonsoft.Json;
using System.Diagnostics;
using TwitchClient.Classes.JSONTwitchAPI;

namespace TwitchClient.Classes
{
	/// <summary>
	/// Class for handling the Twitch API v5
	/// </summary>
	class TwitchAPI
	{
		public string OAuthToken { get; set; }

		private HTTP httpClient;
		private string twitchClientId;
		
		/// <summary>
		/// Constructor of the TwitchAPI class
		/// </summary>
		/// <param name="clientId">Twitch App's Client ID</param>
		/// <param name="twitchOAuthToken">Authorized user's OAuth Token</param>
		public TwitchAPI(string clientId, string twitchOAuthToken = null)
		{
			twitchClientId = clientId ?? throw new ArgumentException("clientId must not be null!", "clientId");
			if (twitchOAuthToken != null) OAuthToken = twitchOAuthToken;

			httpClient = new HTTP();
		}

		/// <summary>
		/// Get the current user's info
		/// </summary>
		/// <returns>JSONTwitch.User object</returns>
		public async Task<JSONTwitch.User> GetUser()
		{
			if (OAuthToken == null) throw new Exception("Tried to request without OAuth set");
			string url = String.Format("https://api.twitch.tv/kraken/user?oauth_token={0}", OAuthToken);
			string json = await httpClient.Get(url);

			JSONTwitch.User user = JsonConvert.DeserializeObject<JSONTwitch.User>(json);

			return user;
		}

		/// <summary>
		/// Get the current user's dashboard (25 streams)
		/// </summary>
		/// <returns>JSONTwitch.Streams object</returns>
		public async Task<JSONTwitch.Streams> GetStreams()
		{
			string url;
			if (OAuthToken != null)
				url = String.Format("https://api.twitch.tv/kraken/streams?oauth_token={0}", OAuthToken);
			else
				url = String.Format("https://api.twitch.tv/kraken/streams?client_id={0}", twitchClientId);

			string json = await httpClient.Get(url);

			JSONTwitch.Streams streams = JsonConvert.DeserializeObject<JSONTwitch.Streams>(json);

			return streams;
		}
    }
}
