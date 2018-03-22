using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
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
			string url = e.Uri.ToString();

			Debug.WriteLine(String.Format("FAILED TO LOAD WEBPAGE {0}: {1}", url, e.WebErrorStatus));
		}

		private void webView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
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
				textBlock.Text = url;
			}
		}
	}
}
