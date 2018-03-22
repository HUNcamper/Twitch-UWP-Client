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
		private HTTP httpClient;
		private string twitchClientId;
		private string twitchOAuthToken;
		
		public TwitchAPI(string clientId)
		{
			twitchClientId = clientId ?? throw new ArgumentException("clientId must not be null!", "clientId");
			httpClient = new HTTP();
		}

		public async Task<JSONTwitch.User> Authorize(string OAuthToken)
		{
			string user_name = null;
			string url = String.Format("https://api.twitch.tv/kraken/user?oauth_token={0}", OAuthToken);
			string json = await httpClient.Get(url);
			JSONTwitch.User user = JsonConvert.DeserializeObject<JSONTwitch.User>(json);
			if (user.error != null)
			{
				Debug.WriteLine(String.Format("Authorization failed: {0}", user.error));
				return null;
			}
			try
			{
				user_name = user.display_name;
				Debug.WriteLine(String.Format("Authorization successful. Username: {0}", user_name));
			}
			catch(Exception e)
			{
				Debug.WriteLine(String.Format("Authorization failed. Error: {0}", e.ToString()));
			}

			return user;
		}
    }
}
