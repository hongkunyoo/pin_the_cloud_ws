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
        public static string Loading
        {
            get
            {
                return ResourceLoader.GetString("Loading");
            }
        }
        public static string GB
        {
            get
            {
                return ResourceLoader.GetString("GB");
            }
        }
        public static string MB
        {
            get
            {
                return ResourceLoader.GetString("MB");
            }
        }
        public static string KB
        {
            get
            {
                return ResourceLoader.GetString("KB");
            }
        }
        public static string Bytes
        {
            get
            {
                return ResourceLoader.GetString("Bytes");
            }
        }
        public static string GoogleDoc
        {
            get
            {
                return ResourceLoader.GetString("GoogleDoc");
            }
        }
        public static string AtHere
        {
            get
            {
                return ResourceLoader.GetString("AtHere");
            }
        }
        public static string Pick
        {
            get
            {
                return ResourceLoader.GetString("Pick");
            }
        }
        public static string Pin
        {
            get
            {
                return ResourceLoader.GetString("Pin");
            }
        }
        public static string InternetUnavailableMessage
        {
            get
            {
                return ResourceLoader.GetString("InternetUnavailableMessage");
            }
        }
        public static string NoFileInFolderMessage
        {
            get
            {
                return ResourceLoader.GetString("NoFileInFolderMessage");
            }
        }
        public static string Refrshing
        {
            get
            {
                return ResourceLoader.GetString("Refrshing");
            }
        }
        public static string NoFileInSpotMessage
        {
            get
            {
                return ResourceLoader.GetString("NoFileInSpotMessage");
            }
        }
        public static string OK
        {
            get
            {
                return ResourceLoader.GetString("OK");
            }
        }
        public static string NoNearSpotMessage
        {
            get
            {
                return ResourceLoader.GetString("NoNearSpotMessage");
            }
        }
        public static string BadLoadingSpotMessage
        {
            get
            {
                return ResourceLoader.GetString("BadLoadingSpotMessage");
            }
        }
        public static string BadLocationServiceMessage
        {
            get
            {
                return ResourceLoader.GetString("BadLocationServiceMessage");
            }
        }
        public static string BadSignInMessage
        {
            get
            {
                return ResourceLoader.GetString("BadSignInMessage");
            }
        }
        public static string BadSignInCaption
        {
            get
            {
                return ResourceLoader.GetString("BadSignInCaption");
            }
        }
        public static string InternetUnavailableCaption
        {
            get
            {
                return ResourceLoader.GetString("InternetUnavailableCaption");
            }
        }
        public static string DoingSignIn
        {
            get
            {
                return ResourceLoader.GetString("DoingSignIn");
            }
        }

    }
}
