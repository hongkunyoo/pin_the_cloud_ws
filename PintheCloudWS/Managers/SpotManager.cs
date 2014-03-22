﻿using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using PintheCloudWS.Helpers;
using PintheCloudWS.Models;
using PintheCloudWS.Pages;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PintheCloudWS.Managers
{
    public class SpotManager
    {
        private List<SpotObject> spotList;
        private List<SpotObject> mySpotList;
        private SpotObject newSpot;



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
            JArray spots = null;
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
        private async Task<JArray> GetMySpotsAsync(string ptcAccountId)
        {
            string json = @"{'ptcAccountId':'" + ptcAccountId + "'}";
            JToken jToken = JToken.Parse(json);
            JArray spots = new JArray();
            try
            {
                // Load current account's spots
                spots = (JArray)await App.MobileService.InvokeApiAsync("select_my_spots_async", jToken);
            }
            catch (MobileServiceInvalidOperationException)
            {
                return null;
            }
            return spots;
        }


        public async Task<bool> CreateSpotAsync(SpotObject so)
        {
            MSSpotObject spot = SpotObject.ConvertToMSSpotObject(so);
            try
            {
                await App.MobileService.GetTable<MSSpotObject>().InsertAsync(spot);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
            so.Id = spot.id;
            this.newSpot = so;
            return true;
        }


        public async Task<bool> DeleteSpotAsync(string spotId)
        {
            MSSpotObject msso = new MSSpotObject("", 0, 0, "", "", 0, false, "");
            msso.id = spotId;
            try
            {
                await App.MobileService.GetTable<MSSpotObject>().DeleteAsync(msso);
            }
            catch (MobileServiceInvalidOperationException)
            {
                return false;
            }
            return true;
        }


        public async Task<List<SpotObject>> GetNearSpotListAsync(Geoposition currentGeoposition)
        {
            List<SpotObject> list = new List<SpotObject>();

            // Get current coordinate
            double currentLatitude = currentGeoposition.Coordinate.Latitude;
            double currentLongtitude = currentGeoposition.Coordinate.Longitude;

            // Get spots formed JArray
            JArray jSpots = await this.GetNearSpotsAsync(currentLatitude, currentLongtitude);

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
                    string accountId = (string)jSpot["ptcaccount_id"];
                    string accountName = (string)jSpot["ptcaccount_name"];
                    double spotDistance = (double)jSpot["spot_distance"];
                    bool isPrivate = (bool)jSpot["is_private"];
                    string spot_password = (string)jSpot["spot_password"];
                    string create_at = (string)jSpot["create_at"];

                    SpotObject spot = new SpotObject(spotName, spotLatitude, spotLongtitude, accountId, accountName, spotDistance, isPrivate, spot_password, create_at);
                    spot.Id = spotId;
                    list.Add(spot);
                }
            }
            else
            {
                return null;
            }
            this.spotList = list;
            return list;
        }


        public async Task<List<SpotObject>> GetMySpotList()
        {
            // If signed in id is over one number, get my spots.
            // Othewise, return null
            List<SpotObject> spots = new List<SpotObject>();
            // Get spots formed JArray
            JArray jSpots = await this.GetMySpotsAsync(App.AccountManager.GetPtcId());

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
                    string accountId = (string)jSpot["ptcaccount_id"];
                    string accountName = (string)jSpot["ptcaccount_name"];
                    double spotDistance = (double)jSpot["spot_distance"];
                    bool isPrivate = (bool)jSpot["is_private"];
                    string spot_password = (string)jSpot["spot_password"];
                    string create_at = (string)jSpot["create_at"];

                    SpotObject spot = new SpotObject(spotName, spotLatitude, spotLongtitude, accountId, accountName, spotDistance, isPrivate, spot_password, create_at);
                    spot.Id = spotId;
                    spots.Add(spot);
                }
            }
            else
            {
                return null;
            }
            this.mySpotList = spots;
            return spots;
        }


        public SpotObject GetSpotObject(string spotId)
        {
            if (this.spotList != null)
                for (var i = 0; i < this.spotList.Count; i++)
                    if (spotList[i].Id.Equals(spotId)) return spotList[i];

            if (this.mySpotList != null)
                for (var i = 0; i < this.mySpotList.Count; i++)
                    if (mySpotList[i].Id.Equals(spotId)) return mySpotList[i];

            if (this.newSpot != null)
                if (this.newSpot.Id.Equals(spotId)) return this.newSpot;

            return null;
        }

    }
}