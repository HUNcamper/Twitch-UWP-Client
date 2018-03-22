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

namespace TwitchClient
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.InitializeComponent();
        }

		// Regexes for extracting tokens from the auth url
		private Regex regexToken =		new Regex(@"#access_token=([a-zA-Z0-9._-]+)&");
		private Regex regexIDToken =	new Regex(@"&id_token=([a-zA-Z0-9._-]+)&");

		private void bLogin_Click(object sender, RoutedEventArgs e)
		{
			loginFrame.Visibility = Visibility.Visible;
			loginWebview.Source = new Uri("https://id.twitch.tv/oauth2/authorize?client_id=ejho3mdl9ugrndt9ngwjf1dp3ebgkn&redirect_uri=http://localhost&response_type=token+id_token&scope=openid+user_read&force_verify=true");
		}

		private async void loginWebview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
		{
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
		}

		/*
		private async void bLogin_Click(object sender, RoutedEventArgs e)
		{
			string token = null;
			string id_token = null;

			CoreApplicationView newView = CoreApplication.CreateNewView();
			int newViewId = 0;
			await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				Frame frame = new Frame();
				frame.Navigate(typeof(TwitchLogin), null);
				Window.Current.Content = frame;
				Window.Current.Activate();
				newViewId = ApplicationView.GetForCurrentView().Id;
			});
			bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);

			await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				//do something with the secondary view's UI
				//this code now runs on the secondary view's thread
				//Window.Current here refers to the secondary view's Window

				//the following example changes the background color of the page
				var frame = (Frame)Window.Current.Content;
				var page = (TwitchLogin)frame.Content;
				var grid = (Grid)page.Content;
				var webView = (WebView) grid.Children[0];
				var textBlock = (TextBlock)grid.Children[1];

				string authUrl = "https://id.twitch.tv/oauth2/authorize?client_id=ejho3mdl9ugrndt9ngwjf1dp3ebgkn&redirect_uri=http://localhost&response_type=token+id_token&scope=openid&force_verify=true";

				Uri loginUri = new Uri(authUrl);
				webView.Source = loginUri;

				string error = null;

				while (!webView.Source.ToString().StartsWith("http://localhost"))
				{
					frame = (Frame)Window.Current.Content;
					page = (TwitchLogin)frame.Content;
					grid = (Grid)page.Content;
					webView = (WebView)grid.Children[0];
					textBlock = (TextBlock)grid.Children[1];

					if (!Window.Current.Visible)
					{
						error = "Window was closed";
						break;
					}

					if (textBlock.Text.StartsWith("http://localhost"))
					{
						Window.Current.Close();
						break;
					}

					//Debug.WriteLine(String.Format("Waiting... {0}", webView.Source.ToString()));
					await Task.Delay(25);
				}

				if (error != null)
				{
					Debug.WriteLine(String.Format("Canceled: {0}", error));
				}
				else
				{
					string url = textBlock.Text;
					Debug.WriteLine(String.Format("DONE! Link: {0}", url));

					Match matchToken =		regexToken.Match(url);
					Match matchIDToken =	regexIDToken.Match(url);
					
					if (matchToken.Success && matchIDToken.Success)
					{
						token =				matchToken.Groups[1].Value;
						id_token =			matchIDToken.Groups[1].Value;

						Debug.WriteLine(String.Format("User's ID token: {0}", id_token));
						Debug.WriteLine(String.Format("User's token: {0}", token));
					}
				}
			});

			//Frame.Navigate(typeof(TwitchStream));
		}*/

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
			if (url.StartsWith("http://localhost"))
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

					TwitchAPI twitch = new TwitchAPI("ejho3mdl9ugrndt9ngwjf1dp3ebgkn");
					 JSONTwitch.User user = await twitch.Authorize(token);
					if (user.name != null)
						textBlock.Text = String.Format("Welcome back, {0}!", user.name);
					if (user.logo != null)
						imageUser.Source = new BitmapImage (new Uri(user.logo));
				}

				loginFrame.Visibility = Visibility.Collapsed;
			}
		}
	}
}
