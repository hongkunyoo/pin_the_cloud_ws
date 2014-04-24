﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PintheCloudWS.Helpers
{
    public class GeoHelper
    {
        public static async Task<Geoposition> GetGeopositionAsync()
        {
            try
            {
                return await App.Geolocator.GetGeopositionAsync(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(10));
            }
            catch
            {
                return null;
            }
        }

        public static PositionStatus GetLocationStatus()
        {
            return App.Geolocator.LocationStatus;
        }
    }
}
