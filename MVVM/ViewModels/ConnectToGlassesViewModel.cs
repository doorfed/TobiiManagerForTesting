using G3SDK;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using TobiiGlassesManager.Core;
using System;

namespace TobiiGlassesManager.MVVM.ViewModels
{
    internal class ConnectToGlassesViewModel : ObservableObject
    {
        private G3Browser _browser;

        private ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }
        }

        private DeviceViewModel _currentDevice;
        public DeviceViewModel CurrentDevice
        {
            get { return _currentDevice; }
            set
            {
                _currentDevice = value;
                _recordings = _currentDevice.CreateRecordingsVM();
            }
        }

        private RecordingsViewModel _recordings;
        public RecordingsViewModel Recordings
        {
            get => _recordings;
            private set
            {
                if (Equals(value, _recordings)) return;
                _recordings = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SearchForGlassesCommand { get; set; }
        public RelayCommand ConnectToDeviceCommand { get; set; }

        public ConnectToGlassesViewModel(Dispatcher dispatcher) : base(dispatcher)
        {
            _browser = new G3Browser();
            _devices = new ObservableCollection<DeviceViewModel>();
            _currentDevice = null;

            ConnectToDeviceCommand = new RelayCommand(o =>
            {
                _currentDevice = (DeviceViewModel)o;
            });

            SearchForGlassesCommand = new RelayCommand(async o =>
            {
                await BrowseForGlasses();
            });
        }

        private async Task BrowseForGlasses()
        {
            var devices = await _browser.ScanZeroConf();

            _devices.Clear();

            foreach (var d in devices)
            {
                _devices.Add(new DeviceViewModel(Dispatcher, d.DisplayName, new G3Api(d.IPAddress)));
            }
        }
    }
}
