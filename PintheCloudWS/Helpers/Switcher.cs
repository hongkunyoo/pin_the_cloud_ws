﻿using PintheCloudWS;
using PintheCloudWS.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.Helpers
{
    public static class Switcher
    {
        private static IStorageManager CurrentManager = null;
        public static string MAIN_PLATFORM_TYPE_KEY = "MAIN_PLATFORM_TYPE_KEY";


        public static void SetStorageTo(string key)
        {
            CurrentManager = StorageHelper.GetStorageManager(key);
        }


        public static IStorageManager GetCurrentStorage()
        {
            return CurrentManager;
        }


        public static void SetMainPlatform(string key)
        {
            App.ApplicationSettings[MAIN_PLATFORM_TYPE_KEY] = key;
            App.ApplicationSettings.Save();
        }


        public static IStorageManager GetMainStorage()
        {
            IStorageManager retStorage = StorageHelper.GetStorageManager((string)App.ApplicationSettings[MAIN_PLATFORM_TYPE_KEY]);
            return retStorage;
        }


        public static void SetStorageToMainPlatform()
        {
            if (App.ApplicationSettings.Contains(MAIN_PLATFORM_TYPE_KEY))
                SetStorageTo((string)App.ApplicationSettings[MAIN_PLATFORM_TYPE_KEY]);
            else
                SetStorageTo("DEFAULT");
        }


        public static int GetCurrentIndex()
        {
            return StorageHelper.GetStorageList().IndexOf(CurrentManager);
        }


        public static int GetStorageIndex(string key)
        {
            return StorageHelper.GetStorageList().IndexOf(StorageHelper.GetStorageManager(key));
        }
    }
}
