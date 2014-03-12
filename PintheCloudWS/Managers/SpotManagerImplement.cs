﻿using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using PintheCloudWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PintheCloudWS.Managers
{
    public class SpotManagerImplement : SpotManager
    {
        /*** Implementation ***/

        public async Task<bool> PinSpotAsync(Spot spot)
        {
            try
            {
                await App.MobileService.GetTable<Spot>().InsertAsync(spot);
            }
            catch (MobileServiceInvalidOperationException)
            {
                return false;
            }
            return true;
        }


        public async Task<bool> DeleteSpotAsync(Spot spot)
        {
            try
            {
                await App.MobileService.GetTable<Spot>().DeleteAsync(spot);
            }
            catch (MobileServiceInvalidOperationException)
            {
                return false;
            }
            return true;
        }


        // Get spot view item from spot list.
        public async Task<List<Spot>> GetNearSpotViewItemsAsync(Geoposition currentGeoposition)
        {
            // Get current coordinate
            double currentLatitude = currentGeoposition.Coordinate.Latitude;
            double currentLongtitude = currentGeoposition.Coordinate.Longitude;

            // Get spots formed JArray
            JArray jSpots = await this.GetNearSpotsAsync(currentLatitude, currentLongtitude);

            // If loading spot doesn't occur error, Convert jarray spots to spot list
            List<Spot> spots = new List<Spot>();
            if (jSpots != null)
            {
                foreach (JObject jSpot in jSpots)
                {
                    // Set new spot view item
                    string spotId = (string)jSpot["id"];
                    string spotName = (string)jSpot["spot_name"];
                    double spotLatitude = (double)jSpot["spot_latitude"];
                    double spotLongtitude = (double)jSpot["spot_longtitude"];
                    string accountId = (string)jSpot["account_id"];
                    string accountName = (string)jSpot["account_name"];
                    double spotDistance = (double)jSpot["spot_distance"];
                    bool isPrivate = (bool)jSpot["is_private"];
                    string spot_password = (string)jSpot["spot_password"];

                    Spot spot = new Spot(spotName, spotLatitude, spotLongtitude, accountId, accountName, spotDistance, isPrivate, spot_password);
                    spot.id = spotId;
                    spots.Add(spot);
                }
            }
            else
            {
                return null;
            }
            return spots;
        }


        // Get spot view item from spot list.
        public async Task<List<Spot>> GetMySpotViewItemsAsync()
        {
            // Get My signed in ids
            List<string> ids = new List<string>();
            for (int i = 0; i < App.IStorageManagers.Length; i++)
            {
                if (App.IStorageManagers[i].IsSignIn())
                {
                    // Wait task
                    IStorageManager iStorageManager = App.IStorageManagers[i];
                    await App.TaskHelper.WaitSignInTask(iStorageManager.GetStorageName());
                    await App.TaskHelper.WaitSignOutTask(iStorageManager.GetStorageName());

                    // If it wasn't signed out, set list.
                    // Othersie, show sign in grid.
                    if (iStorageManager.GetAccount() != null)  // Wasn't signed out.
                        ids.Add(iStorageManager.GetAccount().account_platform_id);
                }
            }

            // If signed in id is over one number, get my spots.
            // Othewise, return null
            List<Spot> spots = new List<Spot>();
            if (ids.Count > 0)
            {
                // Get spots formed JArray
                JArray jSpots = await this.GetMySpotsAsync(ids);

                // If loading spot doesn't occur error, Convert jarray spots to spot list
                if (jSpots != null)
                {
                    foreach (JObject jSpot in jSpots)
                    {
                        // Set new spot view item
                        string spotId = (string)jSpot["id"];
                        string spotName = (string)jSpot["spot_name"];
                        double spotLatitude = (double)jSpot["spot_latitude"];
                        double spotLongtitude = (double)jSpot["spot_longtitude"];
                        string accountId = (string)jSpot["account_id"];
                        string accountName = (string)jSpot["account_name"];
                        double spotDistance = (double)jSpot["spot_distance"];
                        bool isPrivate = (bool)jSpot["is_private"];
                        string spot_password = (string)jSpot["spot_password"];

                        Spot spot = new Spot(spotName, spotLatitude, spotLongtitude, accountId, accountName, spotDistance, isPrivate, spot_password);
                        spot.id = spotId;
                        spots.Add(spot);
                    }
                }
                else
                {
                    return null;
                }
            }
            return spots;
        }


        // Check Spot Password
        public async Task<bool> CheckSpotPasswordAsync(string spotId, string spotPassword)
        {
            string json = @"{'spotId':'" + spotId + "','spotPassword':'" + spotPassword + "'}";
            JToken jToken = JToken.Parse(json);
            try
            {
                // Load current account's spots
                await App.MobileService.InvokeApiAsync("check_spot_password_async", jToken);
            }
            catch (MobileServiceInvalidOperationException)
            {
                return false;
            }
            return true;
        }



        /*** Private Methods ***/

        // Get spots 300m away from here
        private async Task<JArray> GetNearSpotsAsync(double currentLatitude, double currentLongtitude)
        {
            string currentLatitudeString = currentLatitude.ToString().Replace(',', '.');
            string currentLongtitudeString = currentLongtitude.ToString().Replace(',', '.');
            string json = @"{'currentLatitude':" + currentLatitudeString + ",'currentLongtitude':" + currentLongtitudeString + "}";
            JToken jToken = JToken.Parse(json);
            JArray spots = new JArray();
            try
            {
                // Load near spots use custom api in server script
                spots = (JArray)await App.MobileService.InvokeApiAsync("select_near_spots_async", jToken);
            }
            catch (MobileServiceInvalidOperationException)
            {
                return null;
            }
            return spots;
        }


        // Get spots from DB
        private async Task<JArray> GetMySpotsAsync(List<string> ids)
        {
            JArray spots = new JArray();
            try
            {
                // Load current account's spots
                spots = (JArray)await App.MobileService.InvokeApiAsync<List<string>, JArray>("select_my_spots_async", ids);
            }
            catch (MobileServiceInvalidOperationException)
            {
                return null;
            }
            return spots;
        }
    }
}
