using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using FFmpegInterop;
using Windows.UI.Popups;
using Windows.Media.Core;
using Windows.Storage.Streams;

namespace TwitchClient
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private FFmpegInteropMSS FFmpegMSS;

        private async void AppBar_OpenFile(object sender, TappedRoutedEventArgs e)
        {
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
