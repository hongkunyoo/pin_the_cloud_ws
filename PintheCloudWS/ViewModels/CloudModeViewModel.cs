using PintheCloudWS.Locale;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.ViewModels
{
    public class CloudModeViewModel : INotifyPropertyChanged
    {
        private const string ONE_DRIVE_IMAGE_URI = "/Assets/pajeon/pin/png/ico_onedrive.png";
        private const string DROPBOX_IMAGE_URI = "/Assets/pajeon/pin/png/ico_dropbox.png";
        private const string GOOGLE_DRIVE_IMAGE_URI = "/Assets/pajeon/pin/png/ico_googledrive.png";
        
        private const string ONE_DRIVE_COLOR_HEX_STRING = "2458A7";
        private const string DROPBOX_COLOR_HEX_STRING = "26A4DD";
        private const string GOOGLE_DRIVE_COLOR_HEX_STRING = "F1AE1D";
            
                
        public ObservableCollection<CloudModeViewItem> Items { get; set; }


        public CloudModeViewModel()
        {
            this.Items = new ObservableCollection<CloudModeViewItem>();
            this.Items.Add(new CloudModeViewItem(App.ResourceLoader.GetString(ResourcesKeys.OneDrive), 
                ONE_DRIVE_IMAGE_URI, ONE_DRIVE_COLOR_HEX_STRING, App.ResourceLoader.GetString(ResourcesKeys.SignIn)));
            this.Items.Add(new CloudModeViewItem(App.ResourceLoader.GetString(ResourcesKeys.Dropbox), 
                DROPBOX_IMAGE_URI, DROPBOX_COLOR_HEX_STRING, App.ResourceLoader.GetString(ResourcesKeys.SignIn)));
            this.Items.Add(new CloudModeViewItem(App.ResourceLoader.GetString(ResourcesKeys.GoogleDrive), 
                GOOGLE_DRIVE_IMAGE_URI, GOOGLE_DRIVE_COLOR_HEX_STRING, App.ResourceLoader.GetString(ResourcesKeys.SignIn)));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
