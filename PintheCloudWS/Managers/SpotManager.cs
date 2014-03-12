using PintheCloudWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PintheCloudWS.Managers
{
    public interface SpotManager
    {
        Task<bool> PinSpotAsync(Spot spot);
        Task<bool> DeleteSpotAsync(Spot spot);
        Task<List<Spot>> GetNearSpotViewItemsAsync(Geoposition currentGeoposition);
        Task<List<Spot>> GetMySpotViewItemsAsync();
        Task<bool> CheckSpotPasswordAsync(string spotId, string spotPassword);
    }
}
