using G3SDK;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TobiiGlassesManager.Core;

namespace TobiiGlassesManager.MVVM.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand CloseAppCommand { get; set; }

        public HomeViewModel HomeViewModel { get; set; }
        public RelayCommand HomeViewCommand { get; set; }

        public ConnectToGlassesViewModel ConnectToGlassesViewModel { get; set; }
        public RelayCommand ConnectToGlassesViewCommand { get; set; }
        public RelayCommand DevicesViewCommand { get; set; }

        public RecordingsViewModel RecordingsViewModel { get; set; }
        public RelayCommand RecordingsViewCommand { get; set; }

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
            InitViewModel();
        }

        public MainViewModel(Dispatcher dispatcher) : base(dispatcher)
        {
            InitViewModel();
        }

        private void InitViewModel()
        {
            InitViewModels();
            CurrentView = HomeViewModel;

            CloseAppCommand = new RelayCommand(o =>
            {
                Application.Current.Shutdown();
            });

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeViewModel;
            });

            ConnectToGlassesViewCommand = new RelayCommand(o =>
            {
                CurrentView = ConnectToGlassesViewModel;
            });

            RecordingsViewCommand = new RelayCommand(o =>
            {
                CurrentView = ConnectToGlassesViewModel.Recordings;
            });
        }

        private void InitViewModels()
        {
            HomeViewModel = new HomeViewModel(Dispatcher);
            ConnectToGlassesViewModel = new ConnectToGlassesViewModel(Dispatcher);
        }
    }
}
