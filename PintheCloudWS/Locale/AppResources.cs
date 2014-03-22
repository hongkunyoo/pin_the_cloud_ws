using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace PintheCloudWS.Locale
{
    public static class AppResources
    {
        private static ResourceLoader ResourceLoader = new ResourceLoader();

        //private static const string AtHere = "AtHere";

        //private static const string Pick = "Pick";
        //private static const string Pin = "Pin";

        //private static const string OK = "OK";
        //private static const string Cancel = "Cancel";

        //private static const string SignIn = "SignIn";
        //private static const string SignOut = "SignOut";
        //private static const string Empty = "Empty";

        //private static const string GB = "GB";
        //private static const string MB = "MB";
        //private static const string KB = "KB";
        //private static const string Bytes = "Bytes";

        //private static const string GoogleDoc = "GoogleDoc";

        //private static const string OneDrive = "OneDrive";
        //private static const string Dropbox = "Dropbox";
        //private static const string GoogleDrive = "GoogleDrive";

        //private static const string InternetUnavailableMessage = "InternetUnavailableMessage";
        //private static const string LocationAccessMessage = "LocationAccessMessage";

        //private static const string NoNearSpotMessage = "NoNearSpotMessage";

        //private static const string BadLoadingSpotMessage = "BadLoadingSpotMessage";
        //private static const string BadLocationServiceMessage = "BadLocationServiceMessage";

        //private static const string NoLocationServiceMessage = "NoLocationServiceMessage";
        //private static const string NoFileInSpotMessage = "NoFileInSpotMessage";
        //private static const string NoFileInFolderMessage = "NoFileInFolderMessage";

        //private static const string Loading = "Loading";
        //private static const string Refrshing = "Refrshing";
        
        public static string OneDrive
        {
            get
            {
                return ResourceLoader.GetString("OneDrive");
            }
        }
        public static string Dropbox
        {
            get
            {
                return ResourceLoader.GetString("Dropbox");
            }
        }
        public static string GoogleDrive
        {
            get
            {
                return ResourceLoader.GetString("GoogleDrive");
            }
        }
    }
}
