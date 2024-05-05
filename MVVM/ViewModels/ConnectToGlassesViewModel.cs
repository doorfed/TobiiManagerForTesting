using G3SDK;
using Tobii.Research;
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

        private ObservableCollection<IEyeTracker> _devices;
        public ObservableCollection<IEyeTracker> Devices
        {
            get { return _devices; }
        }

        private IEyeTracker _currentDevice;
        public IEyeTracker CurrentDevice
        {
            get { return _currentDevice; }
            set
            {
                if ( _currentDevice != null)
                {
                    _currentDevice.GazeDataReceived -= EyeTracker_GazeDataReceived;
                }

                _currentDevice = value;
                _currentDevice.GazeDataReceived += EyeTracker_GazeDataReceived;
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
            _devices = new ObservableCollection<IEyeTracker>();
            _currentDevice = null;

            ConnectToDeviceCommand = new RelayCommand(o =>
            {
                _currentDevice = (IEyeTracker)o;
            });

            SearchForGlassesCommand = new RelayCommand(async o =>
            {
                await BrowseForGlasses();
            });
        }

        private Task BrowseForGlasses()
        {
            EyeTrackerCollection devices = EyeTrackingOperations.FindAllEyeTrackers();

            _devices.Clear();

            foreach (var d in devices)
            {
                _devices.Add(d);
            }

            return Task.CompletedTask;
        }

        private static void EyeTracker_GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            Console.WriteLine(
                "Got gaze data with {0} left eye gaze point at point ({1}, {2}, {3}) in the user coordinate system.",
                e.LeftEye.GazePoint.Validity,
                e.LeftEye.GazePoint.PositionInUserCoordinates.X,
                e.LeftEye.GazePoint.PositionInUserCoordinates.Y,
                e.LeftEye.GazePoint.PositionInUserCoordinates.Z);
            
            Console.WriteLine(
                "Got gaze data with {0} right eye gaze point at point ({1}, {2}, {3}) in the user coordinate system.",
                e.RightEye.GazePoint.Validity,
                e.RightEye.GazePoint.PositionInUserCoordinates.X,
                e.RightEye.GazePoint.PositionInUserCoordinates.Y,
                e.RightEye.GazePoint.PositionInUserCoordinates.Z);
        }
    }
}
