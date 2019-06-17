using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using TwitchClient.Classes;
using Windows.UI.Popups;

namespace TwitchClient.Pages
{
	public sealed partial class TwitchStream : Page
	{
		public TwitchStream()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			this.InitializeComponent();
		}

		private List<M3U> m3uParsed = new List<M3U>();

		// channel, token, sig, random
		private string USHER_API = "http://usher.twitch.tv/api/channel/hls/{0}.m3u8?player=twitchweb&token={1}&sig={2}&$allow_audio_only=true&allow_source=true&type=any&p={3}";

		// channel, client_id
		private string TOKEN_API = "http://api.twitch.tv/api/channels/{0}/access_token?client_id={1}";

		// Is there stream data correctly loaded?
		private bool loaded = false;

		private struct JSONTokenApiResponse
		{
			public string token;
			public string sig;
			public bool mobile_restricted;
		}

		private async void bGetStream_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			loaded = false;
			cbQualitySelect.Items.Clear();

			HTTP http = new HTTP();

			string username = tbTest.Text;

			string json = await http.Get(String.Format(TOKEN_API, username, "ejho3mdl9ugrndt9ngwjf1dp3ebgkn"));

			JSONTokenApiResponse obj = JsonConvert.DeserializeObject<JSONTokenApiResponse>(json);
			Debug.WriteLine("JSON:");
			Debug.WriteLine(obj);

			Debug.WriteLine(json);

			Debug.WriteLine("token: " + obj.token);
			Debug.WriteLine("sig: " + obj.sig);

			string infoText;

			bool success;

			try
			{
				string m3u = await http.Get(String.Format(USHER_API, username, obj.token, obj.sig, 9999));

				m3uParsed = M3UParser.Parse(m3u);
				infoText = "Success!\nSelect the stream quality above to play the stream.";
				success = true;
			}
			catch (Exception exception)
			{
                await new MessageDialog("An error occurred. It's likely that the stream doesn't exist.", "Error").ShowAsync();
                //infoText = exception.ToString();
                Debug.WriteLine(exception.ToString());
                infoText = tInfo.Text;
				success = false;
			}

			tInfo.Text = infoText;

			if (success)
			{
				foreach (var stream in m3uParsed)
				{
					cbQualitySelect.Items.Add(stream.name);
				}

				// Load the twitch chat via embedding it
				Uri chatUri = new Uri(String.Format("https://www.twitch.tv/{0}/chat?darkpopout", username));
				webViewChat.Source = chatUri;

				loaded = true;
			}

			Debug.WriteLine("Done");
		}

		private void cbQualitySelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (loaded)
			{
				ringVideoLoading.IsActive = true;

				// Set media player source to stream URL
				int selected = cbQualitySelect.SelectedIndex;
				Uri streamUri = new Uri(m3uParsed[selected].url);
				mediaPlayer.Source = streamUri;
				mediaPlayer.Play();

				#region Set transport controls

				mediaPlayer.AreTransportControlsEnabled = true;

				mediaPlayer.TransportControls.IsFastForwardButtonVisible = false;
				mediaPlayer.TransportControls.IsFastForwardEnabled = false;

				mediaPlayer.TransportControls.IsFastRewindButtonVisible = false;
				mediaPlayer.TransportControls.IsFastRewindEnabled = false;

				mediaPlayer.TransportControls.IsNextTrackButtonVisible = false;
				mediaPlayer.TransportControls.IsPreviousTrackButtonVisible = false;

				mediaPlayer.TransportControls.IsPlaybackRateButtonVisible = false;
				mediaPlayer.TransportControls.IsPlaybackRateEnabled = false;

				mediaPlayer.TransportControls.IsSkipBackwardButtonVisible = false;
				mediaPlayer.TransportControls.IsSkipBackwardEnabled = false;

				mediaPlayer.TransportControls.IsSkipForwardButtonVisible = false;
				mediaPlayer.TransportControls.IsSkipForwardEnabled = false;

				mediaPlayer.TransportControls.IsStopButtonVisible = false;
				mediaPlayer.TransportControls.IsStopEnabled = false;

				mediaPlayer.TransportControls.IsSeekEnabled = false;
                mediaPlayer.TransportControls.IsSeekBarVisible = false;

                mediaPlayer.TransportControls.IsZoomButtonVisible = true;
				mediaPlayer.TransportControls.IsZoomEnabled = true;

				mediaPlayer.TransportControls.IsRightTapEnabled = true;

				#endregion
			}
		}


		private void mediaPlayer_MediaOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{

		}

		private void mediaPlayer_Tapped(object sender, TappedRoutedEventArgs e)
		{

		}

		private async void webViewChat_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
		{
			// Inject CSS into the twitch chat
			string functionString = "var link = document.createElement('link'); link.rel = 'stylesheet'; link.type = 'text/css'; link.href = 'ms-appx-web:///CSS/TwitchChat.css'; document.getElementsByTagName('head')[0].appendChild(link); ";
			await webViewChat.InvokeScriptAsync("eval", new string[] { functionString });
		}
	}
}
