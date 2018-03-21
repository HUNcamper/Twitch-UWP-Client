using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TwitchClient.Pages
{
    public sealed partial class TwitchLogin : Page
    {
        public TwitchLogin()
        {
            this.InitializeComponent();
		}

		private void webView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
		{
			string url1 = webView.Source.ToString();
			string url = e.Uri.ToString();
			//if (url.StartsWith("http://"))
			textBlock.Text = e.WebErrorStatus + ": " + url;
		}
	}
}
