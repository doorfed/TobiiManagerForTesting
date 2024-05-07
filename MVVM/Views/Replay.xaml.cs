using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TobiiGlassesManager.MVVM.ViewModels;

namespace TobiiGlassesManager.MVVM.Views
{
    public partial class Replay : UserControl
    {
        public Replay()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.1)
            };
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (DataContext is RecordingViewModel _vm)
                _vm.UpdateMediaPlayerData();
        }

        private async void Replay_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is RecordingViewModel _vm)
                await _vm.AttachMediaPlayer(Media, RtaVideo);
        }
    }
}
