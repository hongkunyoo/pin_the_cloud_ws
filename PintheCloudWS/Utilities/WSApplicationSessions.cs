using PintheCloudWS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.Utilities
{
    public class WSApplicationSessions
    {

        public ICollection<string> Keys
        {
            get
            {
                return SuspensionManager.SessionState.Keys;
            }
        }
        public ICollection<object> Values
        {
            get
            {
                return SuspensionManager.SessionState.Values;
            }
        }

        public object this[string key]
        {
            get
            {
                return SuspensionManager.SessionState[key];
            }
            set
            {
                SuspensionManager.SessionState[key] = value;
            }
        }

        public void Add(string key, object value)
        {
            SuspensionManager.SessionState.Add(key, value);
        }
        public bool Contains(string key)
        {
            return SuspensionManager.SessionState.ContainsKey(key);
        }
        public void Save()
        {
            ;
        }
        public bool Remove(string key)
        {
            return SuspensionManager.SessionState.Remove(key);
        }
    }
}
