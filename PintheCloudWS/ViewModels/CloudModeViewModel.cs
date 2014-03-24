﻿using PintheCloudWS.Helpers;
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
        public ObservableCollection<CloudModeViewItem> Items { get; set; }


        public CloudModeViewModel()
        {
            this.Items = new ObservableCollection<CloudModeViewItem>();
            using (var itr = StorageHelper.GetStorageEnumerator())
            {
                while (itr.MoveNext())
                    this.Items.Add(new CloudModeViewItem(itr.Current.GetStorageName(),
                        itr.Current.GetStorageImageUri(), itr.Current.GetStorageColorHexString(), AppResources.Empty));
            }
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
