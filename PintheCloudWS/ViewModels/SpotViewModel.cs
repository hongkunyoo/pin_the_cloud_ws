﻿using PintheCloudWS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.ViewModels
{
    public class SpotViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SpotViewItem> Items { get; set; }

        // Mutex
        public bool IsDataLoaded { get; set; }


        public SpotViewModel()
        {
            this.Items = new ObservableCollection<SpotViewItem>();
        }


        public void SetItems(List<SpotObject> spots)
        {
            // If items have something, clear.
            this.Items.Clear();

            // Sorting spots
            spots.Sort((s1, s2) =>
            {
                return s1.SpotName.CompareTo(s2.SpotName);
            });
            spots.Sort((s1, s2) =>
            {
                return s1.SpotDistance.CompareTo(s2.SpotDistance);
            });

            // Convert jarray spots to spot view items and set to view model
            foreach (SpotObject spot in spots)
            {
                // Set new spot view item
                SpotViewItem spotViewItem = new SpotViewItem(spot);

                if (spot.IsPrivate)
                    spotViewItem.IsPrivateImage = FileObjectViewModel.IS_PRIVATE_IMAGE_URI;
                else
                    spotViewItem.IsPrivateImage = FileObjectViewModel.TRANSPARENT_IMAGE_URI;

                this.Items.Add(spotViewItem);
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
