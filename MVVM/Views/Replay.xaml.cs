using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using TobiiGlassesManager.Core.VideoStreaming;
using TobiiGlassesManager.MVVM.ViewModels;

namespace TobiiGlassesManager.MVVM.Views
{
    public partial class Replay : UserControl
    {
        double lastX = -1;
        double lastY = -1;

        List<TimedLine> TimedLines = new List<TimedLine>();

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
                DrawGazeLine(_vm.GazeX, _vm.GazeY, _vm.PositionInSeconds);

                int deleteCounter = 0;

                foreach (var line in TimedLines)
                {
                    if (_vm.PositionInSeconds < line.TimeOfCreation){
                        deleteCounter++;
                    }
                }

                TimedLines.RemoveRange(TimedLines.Count - deleteCounter, deleteCounter);
                CanvasMap.Children.RemoveRange(TimedLines.Count, deleteCounter);
            }
        }
        
        void DrawGazeLine(double xData, double yData, double drawnInSeconds)
        {
            if (lastX >= 0 && lastY >= 0)
            {
                Line line = new Line()
                {
                    X1 = lastX,
                    Y1 = lastY,
                    X2 = xData.Equals(int.MinValue) ? lastX : xData,
                    Y2 = yData.Equals(int.MinValue) ? lastY : yData,
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 2,
                    Visibility = Visibility.Visible,
                };

                TimedLines.Add(new TimedLine(line, drawnInSeconds));

                CanvasMap.Children.Add(TimedLines[TimedLines.Count - 1].Line);
            }

            lastX = xData;
            lastY = yData;
        }

        private async void Replay_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is RecordingViewModel _vm)
                await _vm.AttachMediaPlayer(Media, RtaVideo);
        }

        private void LinesButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanvasMap.Visibility == Visibility.Visible)
            {
                CanvasMap.Visibility = Visibility.Hidden;
            } 
            else
            {
                CanvasMap.Visibility = Visibility.Visible;
            }
        }
    }
}
