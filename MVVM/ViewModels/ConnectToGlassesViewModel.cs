using System.Collections.Generic;
using System.Collections.ObjectModel;
using TobiiGlassesManager.Core;

namespace TobiiGlassesManager.MVVM.ViewModels
{
    internal class ConnectToGlassesViewModel : ObservableObject
    {
        private readonly HashSet<string> _deviceIds;
        public HashSet<string> DeviceIds
        {
            get { return _deviceIds; } 
        }

        private ObservableCollection<string> _devices;
        public ObservableCollection<string> Devices
        {
            get { return _devices; }
            set 
            { 
                _devices = value;
                _deviceIds.Clear();
                foreach (var device in _devices)
                {
                    _deviceIds.Add(device.Split('-')[0]);
                }
            }
        }

        public ConnectToGlassesViewModel() 
        {
            _deviceIds = new HashSet<string>();
        }
    }
}
