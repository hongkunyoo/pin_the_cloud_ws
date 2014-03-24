using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PintheCloudWS.Utilities
{
    public class WSApplicationSettings
    {
        private ApplicationDataContainer _ApplicationSettings = ApplicationData.Current.LocalSettings;
        

        public ICollection<string> Keys
        {
            get
            {
                return _ApplicationSettings.Values.Keys;
            }
        }
        public ICollection<object> Values
        {
            get
            {
                return _ApplicationSettings.Values.Values;
            }
        }

        public object this[string key]
        {
            get
            {
                return _ApplicationSettings.Values[key];
            }
            set
            {
                _ApplicationSettings.Values[key] = value;
            }
        }

        //public void Add(string key, object value)
        //{
        //    _ApplicationSettings.Values.Add(key, value);
        //}
        public bool Contains(string key)
        {
            return _ApplicationSettings.Values.ContainsKey(key);
        }
        public void Save()
        {
            ;
        }
        public bool Remove(string key)
        {
            return _ApplicationSettings.Values.Remove(key);
        }
    }
}
