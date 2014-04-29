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

using System.Net;
using System.Windows;
using System.Diagnostics;
using PintheCloudWS.Managers;
using PintheCloudWS.Pages;
using System.Threading.Tasks;

namespace PintheCloudWS.Popups
{
    public partial class DropBoxSignInPopup : UserControl
    {
        private Popup Popup;
        private int count;


        public DropBoxSignInPopup(Popup popup, string uri)
        {
            InitializeComponent();
            this.Popup = popup;
            this.count = 0;
            uiWebBrowser.Margin = new Thickness(0, PtcPage.STATUS_BAR_HEIGHT, 0, 0);
            uiWebBrowser.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
            uiWebBrowser.ScriptNotify += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                System.Diagnostics.Debug.WriteLine(e.Value);
            };
        }


        public async Task ClearCache()
        {
            //await uiWebBrowser.ClearInternetCacheAsync();
            //await uiWebBrowser.ClearCookiesAsync();
        }


        private void uiWebBrowser_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationCompletedEventArgs args)
        {
            count++;
            //if (e.Uri.ToString().StartsWith("http://")
            //    && e.Uri.ToString().Contains(DropboxManager.DROPBOX_AUTH_URI))
            if (count == 3)
            {
                this.Popup.IsOpen = false;
                //await uiWebBrowser.ClearCookiesAsync();
            }
        }
    }
}