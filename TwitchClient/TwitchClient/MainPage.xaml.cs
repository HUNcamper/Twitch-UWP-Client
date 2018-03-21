using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using FFmpegInterop;
using Windows.UI.Popups;
using Windows.Media.Core;
using Windows.Storage.Streams;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using TwitchClient.Pages;

namespace TwitchClient
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.InitializeComponent();
        }

		private async void bLogin_Click(object sender, RoutedEventArgs e)
		{
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

					if (!Window.Current.Visible)
					{
						error = "Window was closed";
						break;
					}

					Debug.WriteLine(String.Format("Waiting... {0}", webView.Source.ToString()));
					await Task.Delay(25);
				}

				if (error.Length != 0)
				{
					Debug.WriteLine(String.Format("Error: {0}", error));
				}
				else
				{
					Debug.WriteLine(String.Format("DONE! Link: {0}", webView.Source.ToString()));
				}
			});

			//Frame.Navigate(typeof(TwitchStream));
		}

		private void bStream_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(TwitchStream));
		}
	}
}
