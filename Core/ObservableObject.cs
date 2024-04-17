using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace TobiiGlassesManager.Core
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Dispatcher Dispatcher { get; }

        public ObservableObject()
        {

        }

        public ObservableObject(Dispatcher dispatcher) : this()
        {
            Dispatcher = dispatcher == null ? Dispatcher.CurrentDispatcher : dispatcher;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaiseCanExecuteChange(RelayCommand command)
        {
            Dispatcher.Invoke(command.RaiseCanExecuteChanged);
        }
    }
}
