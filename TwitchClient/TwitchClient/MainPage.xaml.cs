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

		public MainPage()
		{
            this.InitializeComponent();
        }
	}
}
