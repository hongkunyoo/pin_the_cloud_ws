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
        public static string BadLoadingFileMessage
        {
            get
            {
                return ResourceLoader.GetString("BadLoadingFileMessage");
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
        public static string Empty
        {
            get
            {
                return ResourceLoader.GetString("Empty");
            }
        }
    }
}
