using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using TobiiGlassesManager.MVVM.ViewModels;

namespace TobiiGlassesManager.MVVM.Views
{
    public partial class Replay : UserControl
    {
        double lastX = int.MinValue;
        double lastY = int.MinValue;

        public Replay()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (DataContext is RecordingViewModel _vm)
            {
                _vm.UpdateMediaPlayerData();
                DrawGazeLine(_vm.GazeX, _vm.GazeY);
            }
        }
        
        void DrawGazeLine(double xData, double yData)
        {
            if (lastX != int.MinValue && lastY != int.MinValue)
            {
                Line line = new Line()
                {
                    X1 = lastX,
                    Y1 = lastY,
                    X2 = xData,
                    Y2 = yData,
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 2,
                    Visibility = Visibility.Visible,
                };

                CanvasMap.Children.Add(line);
            }

            lastX = xData;
            lastY = yData;
        }

        private async void Replay_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is RecordingViewModel _vm)
                await _vm.AttachMediaPlayer(Media, RtaVideo);
        }
    }
}
