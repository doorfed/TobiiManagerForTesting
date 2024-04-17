using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
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

        private ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }
            set 
            { 
                _devices = value;
                _deviceIds.Clear();
                foreach (var device in _devices)
                {
                    _deviceIds.Add(device.Id);
                }
            }
        }

        public ConnectToGlassesViewModel(Dispatcher dispatcher) : base(dispatcher)
        {
            _deviceIds = new HashSet<string>();
        }
    }
}
