using G3SDK;
using System;
using System.Diagnostics;
using System.Windows.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using OxyPlot;
using Timer = System.Timers.Timer;
using TobiiGlassesManager.Core;

namespace TobiiGlassesManager.MVVM.ViewModels
{
    internal class DeviceViewModel : ObservableObject
    {
        private readonly string _hostName;
        public string HostName
        {
            get { return _hostName; }
        }
        private readonly IG3Api _g3;
        private readonly Timer _externalTimeReferenceTimer;

        private string _gyr;
        private bool _gazeOverlay;
        private string _msg;
        private string _gaze;
        private string _mag;
        private string _acc;
        private SpaceState _spaceState = SpaceState.Unknown;

        private int[] _frequencies = { };
        private int _frequency;
        private CardState _cardState = CardState.NotInserted;

        private string _sync;
        private string _event;

        private bool _selected;
        private double _lastExternalTimeRoundtrip;
        private int _externalTimeReferenceIndex;

        public DeviceViewModel(Dispatcher dispatcher, string hostName, IG3Api g3) : base(dispatcher)
        {
            _hostName = hostName;
            _g3 = g3;

            _externalTimeReferenceTimer = new Timer(5000);
            _externalTimeReferenceTimer.Elapsed += async (sender, args) =>
            {
                var sw = Stopwatch.StartNew();
                await _g3.Recorder.SendEvent("ExternalTimeReference",
                    new ExternalTimeReference(DateTime.UtcNow, DateTime.Now, Environment.MachineName, _lastExternalTimeRoundtrip, _externalTimeReferenceIndex++));
                _lastExternalTimeRoundtrip = sw.Elapsed.TotalSeconds;
            };
            _externalTimeReferenceTimer.Enabled = true;

            _g3.Settings.Changed.SubscribeAsync(OnSettingsChanged);
            _g3.System.Storage.StateChanged.SubscribeAsync(OnCardStateChanged);
            GazePlotEnabled = false;
        }

        #region ViewModel properties
        public string Id => _hostName;
        public bool Selected
        {
            get => _selected;
            set
            {

                if (value == _selected) return;
                _selected = value;
                OnPropertyChanged();
            }
        }
        public string Serial { get; private set; }

        public int Frequency
        {
            get => _frequency;
            set
            {
                if (value == _frequency) return;
                _frequency = value;
                OnPropertyChanged();
                _g3.Settings.SetGazeFrequency(value);
            }
        }

        public int[] Frequencies
        {
            get => _frequencies;
            set
            {
                if (Equals(value, _frequencies)) return;
                _frequencies = value;
                OnPropertyChanged();
            }
        }

        public SpaceState SpaceState
        {
            get => _spaceState;
            set
            {
                if (_spaceState == value) return;
                _spaceState = value;
                OnPropertyChanged();
                RaiseCanExecuteChange(StartRecording);
            }
        }

        public CardState CardState
        {
            get => _cardState;
            set
            {
                if (value == _cardState) return;
                _cardState = value;
                OnPropertyChanged();
                RaiseCanExecuteChange(StartRecording);

            }
        }

        public string Event
        {
            get => _event;
            set
            {
                if (value == _event) return;
                _event = value;
                OnPropertyChanged();
            }
        }

        public string Sync
        {
            get => _sync;
            set
            {
                if (value == _sync) return;
                _sync = value;
                OnPropertyChanged();
            }
        }

        public string Acc
        {
            get => _acc;
            set
            {
                if (value == _acc) return;
                _acc = value;
                OnPropertyChanged();
            }
        }
        public string Gyr
        {
            get => _gyr;
            set
            {
                if (value == _gyr) return;
                _gyr = value;
                OnPropertyChanged();
            }
        }

        public string Mag
        {
            get => _mag;
            set
            {
                if (value == _mag) return;
                _mag = value;
                OnPropertyChanged();
            }
        }

        public string Gaze
        {
            get => _gaze;
            set
            {
                if (value == _gaze) return;
                _gaze = value;
                OnPropertyChanged();
            }
        }

        public string Msg
        {
            get => _msg;
            set
            {
                if (value == _msg) return;
                _msg = value;
                OnPropertyChanged();
            }
        }

        public bool GazeOverlay
        {
            get => _gazeOverlay;
            set
            {
                if (value == _gazeOverlay) return;
                _gazeOverlay = value;
                OnPropertyChanged();
                _g3.Settings.SetGazeOverlay(value);
            }
        }

        public Uri LiveVideoUri => _g3?.LiveRtspUri();
        #endregion

        #region Commands
        public ICommand ShowCalibrationMarkerWindow { get; }
        public RelayCommand StartRecording { get; }
        public RelayCommand StopRecording { get; }
        public RelayCommand TakeSnapshot { get; }
        public RelayCommand ScanQRCode { get; }
        public RelayCommand ToggleZoom { get; }

        public ThrottlingObservableCollection<DataPoint> GazeXSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> GazeYSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> AccXSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> AccYSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> AccZSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> GyrXSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> GyrYSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> GyrZSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> MagXSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> MagYSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> MagZSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> CalibMagXSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> CalibMagYSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> CalibMagZSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> PupilLeftSeries { get; } = new ThrottlingObservableCollection<DataPoint>();
        public ThrottlingObservableCollection<DataPoint> PupilRightSeries { get; } = new ThrottlingObservableCollection<DataPoint>();

        public bool GazePlotEnabled { get; set; }
        public bool PupilPlotEnabled { get; set; }
        public bool AccPlotEnabled { get; set; }
        public bool GyrPlotEnabled { get; set; }
        public bool MagPlotEnabled { get; set; }
        public bool CalibMagPlotEnabled { get; set; }

        public ICommand CalibrateMagStop { get; }
        public ICommand CalibrateMagStart { get; }

        #endregion

        public async Task InitAsync()
        {
            Serial = await _g3.System.RecordingUnitSerial;
            SpaceState = await _g3.System.Storage.SpaceState;
            CardState = await _g3.System.Storage.CardState;
            GazeOverlay = await _g3.Settings.GazeOverlay;
            Frequencies = await _g3.System.AvailableGazeFrequencies();
            Frequency = await _g3.Settings.GazeFrequency;
        }

        private void OnCardStateChanged((SpaceState spaceState, CardState cardState) state)
        {
            CardState = state.cardState;
            SpaceState = state.spaceState;
        }

        private async void OnSettingsChanged(string s)
        {
            if (s == "gaze-overlay")
                GazeOverlay = await _g3.Settings.GazeOverlay;
        }

        public RecordingsViewModel CreateRecordingsVM()
        {
            return new RecordingsViewModel(Dispatcher, _g3);
        }

        public async Task<string> ConfigureWifiFromQR(string data)
        {
            if (WifiSettings.TryParseFromQR(data, out var wifi) && (string.IsNullOrEmpty(wifi.Encryption) || !string.IsNullOrEmpty(wifi.Pwd)))
            {
                return await ConfigureWifi(wifi);
            }

            return "invalid QR code";
        }

        private async Task<string> ConfigureWifi(WifiSettings wifi)
        {
            if (!await _g3.Network.WifiHwEnabled)
                return "wifi not supported";

            if (!await _g3.Network.WifiEnable)
                await _g3.Network.SetWifiEnable(true);
            await _g3.Network.Wifi.Disconnect();

            await _g3.Network.Wifi.Scan();

            foreach (WifiConfiguration c in await _g3.Network.Wifi.Configurations.Children())
            {
                if (await c.SsidName == wifi.Ssid && await c.Psk == wifi.Pwd)
                {
                    var configId = await c.Name;
                    if (await _g3.Network.Wifi.Connect(Guid.Parse(configId)))
                        return "Connection to existing network config successful?";
                    return "Connection to existing network config failed";
                }
            }

            var networks = await _g3.Network.Wifi.Networks.FindBySsid(wifi.Ssid);
            if (networks.Any())
            {
                var res = await _g3.Network.Wifi.ConnectNetwork(networks.First(), wifi.Pwd);
                if (res)
                    return "Connection to new network successful?";
                return "Connection to new network failed";
            }

            return "Unable to find config or network with ssid " + wifi.Ssid;
        }
    }
}
