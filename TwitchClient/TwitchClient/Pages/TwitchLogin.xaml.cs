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

namespace TwitchClient.Pages
{
    public sealed partial class TwitchLogin : Page
	{
		private HTTP httpClient;

		public TwitchLogin()
        {
			this.InitializeComponent();
		}

		// Regexes for extracting tokens from the auth url
		private Regex regexToken = new Regex(@"#access_token=([a-zA-Z0-9._-]+)&");
		private Regex regexIDToken = new Regex(@"&id_token=([a-zA-Z0-9._-]+)&");

		private void bLogin_Click(object sender, RoutedEventArgs e)
		{
			loginScrollViewer.Visibility = Visibility.Visible;
			loginWebview.Source = new Uri("https://id.twitch.tv/oauth2/authorize?client_id=ejho3mdl9ugrndt9ngwjf1dp3ebgkn&redirect_uri=http://localhost&response_type=token+id_token&scope=openid+user_read+user_subscriptions+chat_login+channel_feed_read+channel_feed_edit+channel_editor+channel_check_subscription&force_verify=true");
		}

		private async void loginWebview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
		{
			Debug.WriteLine(String.Format("Resizing {0}", args.Uri.ToString()));
			int width;
			int height;

			var widthString = await loginWebview.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });
			var heightString = await loginWebview.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });
			await loginWebview.InvokeScriptAsync("eval", new[] { "document.body.style.overflow = 'hidden'; document.body.style.background = '#fff';" });

			if (!int.TryParse(widthString, out width))
			{
				throw new Exception("Unable to get page width");
			}
			if (!int.TryParse(heightString, out height))
			{
				throw new Exception("Unable to get page height");
			}

			// resize the webview to the content
			loginWebview.Width = width;
			loginWebview.Height = height;

            //int windowHeight = Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight);
            //loginScrollViewer.Height = windowHeight;
        }

		private void bStream_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(TwitchStream));
		}

		private async void loginWebview_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
		{
			string url = args.Uri.ToString();
			Debug.WriteLine(String.Format("Attemptiong to visit {0}", url));

			// Success, the authentication was successfully redirected.
			// However for some reason the webview would redirect back
			// to the auth page, making it useless. We have to check
			// the url before it actually redirects.
			if (url.StartsWith("http://localhost/?error"))
			{
				Debug.WriteLine("The user canceled the authorization proccess.");
                loginScrollViewer.Visibility = Visibility.Collapsed;
			}
			else if (url.StartsWith("http://localhost/"))
			{
				Debug.WriteLine(String.Format("FOUND AUTH URL: {0}", url));

				Match matchToken = regexToken.Match(url);
				Match matchIDToken = regexIDToken.Match(url);

				if (matchToken.Success && matchIDToken.Success)
				{
					string token = matchToken.Groups[1].Value;
					string id_token = matchIDToken.Groups[1].Value;

					Debug.WriteLine(String.Format("User's ID token: {0}", id_token));
					Debug.WriteLine(String.Format("User's token: {0}", token));

					TwitchAPI twitch = new TwitchAPI("ejho3mdl9ugrndt9ngwjf1dp3ebgkn", token);

					Frame.Navigate(typeof(MainPage));

				}

                loginScrollViewer.Visibility = Visibility.Collapsed;
			}
		}
	}
}
