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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Diagnostics;
using DropNetRT.Models;
using PintheCloudWS.Managers;
using PintheCloudWS.Pages;

namespace PintheCloudWS.Popups
{
    public partial class DropBoxSignInPopup : UserControl
    {
        private Popup Popup = null;


        public DropBoxSignInPopup(Popup popup, string uri)
        {
            InitializeComponent();
            this.Popup = popup;
            //uiWebBrowser.Width = Application.Current.Host.Content.ActualWidth;
            //uiWebBrowser.Height = Application.Current.Host.Content.ActualHeight;
            uiWebBrowser.Margin = new Thickness(0, PtcPage.STATUS_BAR_HEIGHT, 0, 0);
            //uiWebBrowser.IsScriptEnabled = true;
            uiWebBrowser.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }


        //private async void webBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    //Debug.WriteLine(e.Uri.ToString());
        //    if (e.Uri.ToString().StartsWith("http://")
        //        && e.Uri.ToString().Contains(DropboxManager.DROPBOX_AUTH_URI))
        //    {
        //        this.Popup.IsOpen = false;
        //        await uiWebBrowser.ClearCookiesAsync();
        //    }
        //}

        private void uiWebBrowser_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationCompletedEventArgs args)
        {
        	// TODO: Add event handler implementation here.
        }
    }
}