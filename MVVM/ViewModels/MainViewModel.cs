using G3SDK;
using System;
using System.Threading.Tasks;
using TobiiGlassesManager.Core;

namespace TobiiGlassesManager.MVVM.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        private Task _initialBrowseTask;
        private readonly G3Browser _browser;

        public HomeViewModel HomeViewModel { get; set; }
        public RelayCommand HomeViewCommand { get; set; }

        public ConnectToGlassesViewModel ConnectToGlassesViewModel { get; set; }
        public RelayCommand ConnectToGlassesViewCommand { get; set; }
        public RelayCommand BrowseForGlassesCommand { get; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set 
            { 
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            InitViewModels();
            CurrentView = HomeViewModel;

            _browser = new G3Browser();

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeViewModel;
            });

            ConnectToGlassesViewCommand = new RelayCommand(async o =>
            {
                CurrentView = ConnectToGlassesViewModel;
                await BrowseForGlasses();
            });

            BrowseForGlassesCommand = new RelayCommand(async o =>
            {
                await BrowseForGlasses();
            });

            _initialBrowseTask = BrowseForGlasses();
        }

        private void InitViewModels()
        {
            HomeViewModel = new HomeViewModel();
            ConnectToGlassesViewModel = new ConnectToGlassesViewModel();
        }

        private async Task BrowseForGlasses()
        {
            if (_initialBrowseTask != null && !_initialBrowseTask.IsCompleted)
                return;

            var devices = await _browser.ScanZeroConf();

            foreach (var d in devices)
            {
                if (!ConnectToGlassesViewModel.DeviceIds.Contains(d.Id))
                {
                    ConnectToGlassesViewModel.DeviceIds.Add(d.Id);
                    ConnectToGlassesViewModel.Devices.Add(String.Join("-", d.Id, d.DisplayName, d.Services));
                }
            }

            _initialBrowseTask = null;
        }
    }
}
