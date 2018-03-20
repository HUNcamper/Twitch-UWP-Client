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

namespace TwitchClient
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.InitializeComponent();
        }

        private FFmpegInteropMSS FFmpegMSS;

        private struct JSONTokenApiResponse
        {
            public string token;
            public string sig;
            public bool mobile_restricted;
        }

        private async void AppBar_OpenFile(object sender, TappedRoutedEventArgs e)
        {
            // channel, token, sig, random
            string USHER_API = "http://usher.twitch.tv/api/channel/hls/{0}.m3u8?player=twitchweb&token={1}&sig={2}&$allow_audio_only=true&allow_source=true&type=any&p={3}";

            // channel, client_id
            string TOKEN_API = "http://api.twitch.tv/api/channels/{0}/access_token?client_id={1}";

            HTTP http = new HTTP();

            string json = await http.Get(String.Format(TOKEN_API, "arteezy", "ejho3mdl9ugrndt9ngwjf1dp3ebgkn"));

            JSONTokenApiResponse obj = JsonConvert.DeserializeObject<JSONTokenApiResponse>(json);
            Debug.WriteLine("JSON:");
            Debug.WriteLine(obj);

            Debug.WriteLine(json);

            Debug.WriteLine("token: " + obj.token);
            Debug.WriteLine("sig: " + obj.sig);

            string m3u = await http.Get(String.Format(USHER_API, "arteezy", obj.token, obj.sig, 9999));

            Debug.WriteLine("M3U8:");
            Debug.WriteLine(m3u);

            List<M3U> m3uParsed = M3UParser.Parse(m3u);

            Debug.WriteLine("Done");

            //Uri streamUri = new Uri(m3uParsed[0].url);

            //mediaPlayer.Source = MediaSource.CreateFromUri(streamUri);

            #region File Media Player

            /*
            var picker = new FileOpenPicker();

            picker.ViewMode = PickerViewMode.Thumbnail;

            picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;

            picker.FileTypeFilter.Add(".3gp");
            picker.FileTypeFilter.Add(".avi");
            picker.FileTypeFilter.Add(".fla");
            picker.FileTypeFilter.Add(".flv");
            picker.FileTypeFilter.Add(".mkv");
            picker.FileTypeFilter.Add(".mov");
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".mpeg");
            picker.FileTypeFilter.Add(".mpeg2");
            picker.FileTypeFilter.Add(".mpg");
            picker.FileTypeFilter.Add(".rm");
            picker.FileTypeFilter.Add(".rmvb");
            picker.FileTypeFilter.Add(".vob");
            picker.FileTypeFilter.Add(".wmv");
            picker.FileTypeFilter.Add(".webm");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                // Try and pause or stop the current media
                if (mediaPlayer.CanPause == true)
                {
                    try
                    {
                        mediaPlayer.Pause();
                    }
                    catch(Exception)
                    {
                        
                    }
                }
                else
                {
                    try
                    {
                        mediaPlayer.Stop();
                    }
                    catch(Exception)
                    {

                    }
                }

                IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);

                try
                {
                    FFmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(readStream, true, true);
                    MediaStreamSource mss = FFmpegMSS.GetMediaStreamSource();

                    if (mss != null)
                    {

                        #region Enable the hidden transport controls

                        mediaPlayer.AreTransportControlsEnabled = true;

                        mediaPlayer.TransportControls.IsFastForwardButtonVisible = true;
                        mediaPlayer.TransportControls.IsFastForwardEnabled = true;

                        mediaPlayer.TransportControls.IsFastRewindButtonVisible = true;
                        mediaPlayer.TransportControls.IsFastRewindEnabled = true;

                        mediaPlayer.TransportControls.IsNextTrackButtonVisible = true;
                        mediaPlayer.TransportControls.IsPreviousTrackButtonVisible = true;

                        mediaPlayer.TransportControls.IsPlaybackRateButtonVisible = true;
                        mediaPlayer.TransportControls.IsPlaybackRateEnabled = true;

                        mediaPlayer.TransportControls.IsSkipBackwardButtonVisible = true;
                        mediaPlayer.TransportControls.IsSkipBackwardEnabled = true;

                        mediaPlayer.TransportControls.IsSkipForwardButtonVisible = true;
                        mediaPlayer.TransportControls.IsSkipForwardEnabled = true;

                        mediaPlayer.TransportControls.IsStopButtonVisible = true;
                        mediaPlayer.TransportControls.IsStopEnabled = true;

                        mediaPlayer.TransportControls.IsRightTapEnabled = true;

                        #endregion

                        mediaPlayer.SetMediaStreamSource(mss);

                        mediaPlayer.Play();
                    }
                    else
                    {
                        var msg = new MessageDialog("An error has occurred while opening the file.");
                        await msg.ShowAsync();
                    }
                }
                catch(Exception)
                {

                }
            }
            */

            #endregion
        }

        private void mediaPlayer_MediaOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CMDBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void mediaPlayer_MediaEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CMDBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void mediaPlayer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (CMDBar.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                CMDBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                CMDBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }
    }
}
