using System.Windows;
using System.Windows.Controls;
using TobiiGlassesManager.MVVM.ViewModels;

namespace TobiiGlassesManager.MVVM.Views
{
    public partial class Replay : UserControl
    {
        public Replay()
        {
            InitializeComponent();
        }

        private async void Replay_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is RecordingViewModel _vm)
                await _vm.AttachMediaPlayer(Media, RtaVideo);
        }
    }
}
