using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchClient.Classes;
using Newtonsoft.Json;
using System.Diagnostics;
using TwitchClient.Classes.JSONTwitchAPI;
using Windows.Security.Credentials;

namespace TwitchClient.Classes
{
	/// <summary>
	/// Class for handling the Twitch API v5
	/// </summary>
	class TwitchAPI
	{
		public string OAuthToken { get; set; }
		public string username { get; set; }

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
			httpClient = new HTTP();

			if (twitchOAuthToken != null)
			{
				OAuthToken = twitchOAuthToken;
				RecordToken();
			}
		}


		/// <summary>
		/// Record the user's OAuthToken if it doesn't exist in the Password Vault yet
		/// </summary>
		public async void RecordToken()
		{
			PasswordVault pVault = new PasswordVault();
			IReadOnlyList<PasswordCredential> pvList;

			if (username == null)
			{
				JSONTwitch.User user = await this.GetUser();
				username = user.name;
			}

			try
			{
				pvList = pVault.FindAllByResource("twitchAPI");
			}
			catch(Exception)
			{
				PasswordCredential credentials = new PasswordCredential("twitchAPI", username, OAuthToken);
				pVault.Add(credentials);
			}
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

		/// <summary>
		/// Check if the user is authenticated, and if yes, return the token.
		/// </summary>
		/// <returns>Oauth token</returns>
		public async static Task<string> GetSavedToken()
		{
			PasswordVault pVault = new PasswordVault();
			PasswordCredential credentials;
			try
			{
				credentials = pVault.FindAllByResource("twitchAPI")[0];
				
				credentials.RetrievePassword();
			}
			catch (Exception)
			{
				return null;
			}

			Debug.WriteLine(String.Format("Resource: '{0}', User: '{1}', Token: '{2}'", credentials.Resource, credentials.UserName, credentials.Password));

			if (await CheckToken(credentials.Password))
				return credentials.Password;
			else
			{
				Debug.WriteLine("Token is INVALID, deleting");
				pVault.Remove(credentials);
				return null;
			}
		}


		/// <summary>
		/// Checks token's validation. If valid, returns true, otherwise false.
		/// </summary>
		/// <param name="OAuthToken">The token to validate</param>
		/// <returns>If valid, returns true, otherwise false</returns>
		public async static Task<bool> CheckToken(string OAuthToken)
		{
			string url = String.Format("https://api.twitch.tv/kraken/user?oauth_token={0}", OAuthToken);

			HTTP httpClient = new HTTP();
			string json = await httpClient.Get(url);
			httpClient.Dispose();

			// If the json returns with an error key,
			// that means that the token is invalid
			JSONTwitch.User user = JsonConvert.DeserializeObject<JSONTwitch.User>(json);

			if (user.error == null)
				return true;
			else
				return false;
		}
    }
}
