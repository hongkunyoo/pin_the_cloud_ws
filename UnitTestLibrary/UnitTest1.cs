using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PintheCloudWS.Managers;
using PintheCloudWS;
using Windows.Devices.Geolocation;
using PintheCloudWS.Utilities;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
using PintheCloudWS.Models;
using System.Net.NetworkInformation;

namespace UnitTestLibrary1
{
    [TestClass]
    public class UnitTest1
    {
        private int count = 0;
        [TestInitialize]
        public void TestInit1()
        {
            //App.MobileService = new MobileServiceClient(
            //    "https://pinthecloud.azure-mobile.net/",
            //    "yvulzHAGRgNsGnPLHKcEFCPJcuyzKj23"
            //);
            App.ApplicationSessions = new WSApplicationSessions();
            App.ApplicationSettings = new WSApplicationSettings();

            // Manager
            App.SpotManager = new SpotManager();
            App.Geolocator = new Geolocator();
            App.BlobStorageManager = new BlobStorageManager();
            App.LocalStorageManager = new LocalStorageManager();

            IStorageManager OneDriveManager = new OneDriveManager();
            IStorageManager DropBoxManager = new DropboxManager();
            IStorageManager GoogleDriveManger = new GoogleDriveManager();


            /////////////////////////////////////////////////////
            // This order will be displayed at every App Pages
            /////////////////////////////////////////////////////
            StorageHelper.AddStorageManager(OneDriveManager.GetStorageName(), OneDriveManager);
            StorageHelper.AddStorageManager(DropBoxManager.GetStorageName(), DropBoxManager);
            StorageHelper.AddStorageManager(GoogleDriveManger.GetStorageName(), GoogleDriveManger);
            Switcher.SetStorageToMainPlatform();
            App.AccountManager = new AccountManager();


            if (!App.ApplicationSettings.Contains(Switcher.MAIN_PLATFORM_TYPE_KEY))
                App.ApplicationSettings[Switcher.MAIN_PLATFORM_TYPE_KEY] = AppResources.OneDrive;

            // Check nick name at frist login.
            if (!App.ApplicationSettings.Contains(StorageAccount.ACCOUNT_DEFAULT_SPOT_NAME_KEY))
                App.ApplicationSettings[StorageAccount.ACCOUNT_DEFAULT_SPOT_NAME_KEY] = AppResources.AtHere;

            // Check location access consent at frist login.
            if (!App.ApplicationSettings.Contains(StorageAccount.LOCATION_ACCESS_CONSENT_KEY))
                App.ApplicationSettings[StorageAccount.LOCATION_ACCESS_CONSENT_KEY] = false;


            // Do Signin work of each cloud storage.
            if (App.AccountManager.IsSignIn())
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    TaskHelper.AddTask(App.AccountManager.GetPtcId(), App.AccountManager.SignIn());
                    using (var itr = StorageHelper.GetStorageEnumerator())
                    {
                        while (itr.MoveNext())
                            if (itr.Current.IsSignIn())
                                TaskHelper.AddSignInTask(itr.Current.GetStorageName(), itr.Current.SignIn());
                    }
                }
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            IStorageManager manager = new DropboxManager();
            Assert.IsNotNull(manager);

            //Assert.IsFalse(manager.IsSignIn());

        }

        [TestMethod]
        public void TestMethod2()
        {
            IStorageManager manager = new DropboxManager();
            //Assert.IsNull(manager);
            //App app = new App();
            Assert.IsFalse(manager.IsSignIn());

        }
    }
}
