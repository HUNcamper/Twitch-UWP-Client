using System;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using TwitchClient.Pages;
using System.Text.RegularExpressions;
using TwitchClient.Classes;
using TwitchClient.Classes.JSONTwitchAPI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.Security.Credentials;

namespace TwitchClient
{
    public sealed partial class MainPage : Page
	{
		private HTTP httpClient;
		private TwitchAPI twitch;

		public MainPage()
		{
            this.InitializeComponent();
			TwitchAuthenticate();
		}

		private async void TwitchAuthenticate()
		{
			string OAuthToken = await TwitchAPI.GetSavedToken();

			twitch = new TwitchAPI("ejho3mdl9ugrndt9ngwjf1dp3ebgkn", OAuthToken);

			
			textWelcomeBack.Text = String.Format("Welcome back, {0}!", twitch.username);
		}
	}
}
