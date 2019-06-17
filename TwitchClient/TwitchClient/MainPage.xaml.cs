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
using Windows.UI.Popups;

namespace TwitchClient
{
    public sealed partial class MainPage : Page
	{
		private HTTP httpClient;
		private TwitchAPI twitch;
        private JSONTwitch.Streams streams;


        public MainPage()
		{
            this.InitializeComponent();
			textWelcomeBack.Text = "";

            TwitchAuthenticate();

            this.ViewModel = new StreamViewModel();
        }

        public StreamViewModel ViewModel { get; set; }

        private async void TwitchAuthenticate()
		{
			string OAuthToken = await TwitchAPI.GetSavedToken();

			twitch = new TwitchAPI("ejho3mdl9ugrndt9ngwjf1dp3ebgkn", OAuthToken);
			
			textWelcomeBack.Text = String.Format("Welcome back, {0}!", await twitch.GetUserName());

			imageAvatar.Source = await twitch.GetUserAvatar();

            RetrieveStreams();
        }

        private void FillGridView()
        {
            ViewModel.FillStreams(streams);
        }

        private async void RetrieveStreams()
        {
            streams = await twitch.GetStreams();
            FillGridView();
        }

		private void bLogOut_Click(object sender, RoutedEventArgs e)
		{
			twitch.LogOut();
			Frame.Navigate(typeof(Pages.TwitchLogin));
		}

        private async void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //await new MessageDialog((sender as Grid).Name, "Stream").ShowAsync();
            Frame.Navigate(typeof(TwitchStream), (sender as Grid).Name);
        }
    }
}
