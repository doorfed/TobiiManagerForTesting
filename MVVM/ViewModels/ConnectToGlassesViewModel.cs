using G3SDK;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using TobiiGlassesManager.Core;

namespace TobiiGlassesManager.MVVM.ViewModels
{
    internal class ConnectToGlassesViewModel : ObservableObject
    {
        private G3Browser _browser;
        private Task _initialBrowseTask;

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

        private DeviceViewModel _currentDevice;
        public DeviceViewModel CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; }
        }

        public RelayCommand ConnectToDeviceCommand { get; set; }

        public ConnectToGlassesViewModel(Dispatcher dispatcher) : base(dispatcher)
        {
            _browser = new G3Browser();

            _deviceIds = new HashSet<string>();

            ConnectToDeviceCommand = new RelayCommand(o =>
            {
                _currentDevice = (DeviceViewModel)o;
            });

            _initialBrowseTask = BrowseForGlasses();
        }



        private async Task BrowseForGlasses()
        {
            if (_initialBrowseTask != null && !_initialBrowseTask.IsCompleted)
                return;

            var devices = await _browser.ScanZeroConf();

            foreach (var d in devices)
            {
                if (!DeviceIds.Contains(d.Id))
                {
                    DeviceIds.Add(d.Id);
                    Devices.Clear();
                    Devices.Add(new DeviceViewModel(Dispatcher, d.Id, new G3Api(d.IPAddress)));
                }
            }

            _initialBrowseTask = null;
        }
    }
}
