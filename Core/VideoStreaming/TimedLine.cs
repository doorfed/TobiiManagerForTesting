using System;
using System.Windows.Shapes;

namespace TobiiGlassesManager.Core.VideoStreaming
{
    internal class TimedLine
    {
        public Line Line { get; private set; }
        public double TimeOfCreation { get; private set; }

        public TimedLine(Line drawnLine, double drawnTimeInSeconds) { 
            Line = drawnLine;
            TimeOfCreation = drawnTimeInSeconds;
        }
    }
}
