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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

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
		private JSONTwitch.User User;

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
		public void RecordToken()
		{
			PasswordVault pVault = new PasswordVault();
			IReadOnlyList<PasswordCredential> pvList;

			try
			{
				pvList = pVault.FindAllByResource("twitchAPI");
			}
			catch(Exception)
			{
				PasswordCredential credentials = new PasswordCredential("twitchAPI", "twitch", OAuthToken);
				pVault.Add(credentials);
			}
		}

		/// <summary>
		/// Remove the user
		/// </summary>
		public void LogOut()
		{
			PasswordVault pVault = new PasswordVault();
			try
			{
				// Remove every credential recorded for a fresh login
				IReadOnlyList<PasswordCredential> credentials;
				credentials = pVault.FindAllByResource("twitchAPI");

				foreach (var credential in credentials)
				{
					pVault.Remove(credential);
				}
			}
			catch(Exception)
			{
				Debug.WriteLine("WARNING logging out while there are no records in the Password Vault?");
			}
		}

		/// <summary>
		/// Get the user's avatar
		/// </summary>
		/// <returns>ImageSource of the avatar</returns>
		public async Task<ImageSource> GetUserAvatar()
		{
			JSONTwitch.User user = await GetUser();

			return new BitmapImage(new Uri(user.logo));
		}

		/// <summary>
		///	Get the user's name
		/// </summary>
		/// <returns>username</returns>
		public async Task<string> GetUserName()
		{
			JSONTwitch.User user = await GetUser();

			return user.name;
		}

		/// <summary>
		/// Get the current user's info
		/// </summary>
		/// <returns>JSONTwitch.User object</returns>
		public async Task<JSONTwitch.User> GetUser()
		{
			if (User == null)
			{
				if (OAuthToken == null) throw new Exception("Tried to request without OAuth set");
				string url = String.Format("https://api.twitch.tv/kraken/user?oauth_token={0}", OAuthToken);
				string json = await httpClient.Get(url);

				User = JsonConvert.DeserializeObject<JSONTwitch.User>(json);
			}

			return User;
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
