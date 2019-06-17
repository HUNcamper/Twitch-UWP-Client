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

        // Restore the page number to where the user was before.
        private int currentPage = App.StreamListPage;

        public MainPage()
		{
            this.InitializeComponent();
			textWelcomeBack.Text = "";

            TwitchAuthenticate();

            this.ViewModel = new StreamViewModel();
            
            UpdatePageNum();
        }

        public StreamViewModel ViewModel { get; set; }

        private async void TwitchAuthenticate()
		{
			string OAuthToken = await TwitchAPI.GetSavedToken();

			twitch = new TwitchAPI("ejho3mdl9ugrndt9ngwjf1dp3ebgkn", OAuthToken);
			
			textWelcomeBack.Text = String.Format("Welcome back, {0}!", await twitch.GetUserName());

			imageAvatar.Source = await twitch.GetUserAvatar();

            RetrieveStreams(currentPage * 25);
        }

        private void FillGridView()
        {
            ViewModel.Streams.Clear();
            ViewModel.FillStreams(streams);
        }

        /// <summary>
		/// Changes page
		/// </summary>
        /// <param name="startFrom">Start offset, for pagination</param>
        private async void RetrieveStreams(int startFrom)
        {
            streams = await twitch.GetStreams(startFrom);
            FillGridView();
            ringLoading.IsActive = false;
        }

		private void bLogOut_Click(object sender, RoutedEventArgs e)
		{
			twitch.LogOut();
            HandleFrameChange(typeof(Pages.TwitchLogin));
		}

        private async void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // Send streamer's name to stream watch page
            HandleFrameChange(typeof(TwitchStream), (sender as Grid).Name);
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                RetrieveStreams(25 * currentPage);
            }
            ChangePage();
            UpdatePageNum();
        }

        private void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            buttonBack.IsEnabled = true;
            currentPage++;
            ChangePage();
            UpdatePageNum();
        }

        /// <summary>
		/// Changes page
		/// </summary>
        private void ChangePage()
        {
            ringLoading.IsActive = true;
            RetrieveStreams(25 * currentPage);
        }

        /// <summary>
		/// Updates the page number textblock, and update buttons
		/// </summary>
        private void UpdatePageNum()
        {
            textPageNum.Text = (currentPage + 1).ToString();
            
            if (currentPage == 0)
            {
                buttonBack.IsEnabled = false;
            }
            else
            {
                buttonBack.IsEnabled = true;
            }
        }

        /// <summary>
        /// Updates the page number textblock
        /// </summary>
        /// <param name="page">typeof page to navigate to</param>
        /// <param name="parameters">parameters to send</param>
        private void HandleFrameChange(Type page, object parameters=null)
        {
            App.StreamListPage = currentPage;
            Frame.Navigate(page, parameters);
        }
    }
}
