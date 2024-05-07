using FFmpeg.AutoGen;
using System;
using System.Windows;
using Unosquare.FFME;

namespace TobiiGlassesManager
{
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        public App()
        {
            // Change the default location of the ffmpeg binaries (same directory as application)
            // You can get the 64-bit binaries here: https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-full-shared.7z
            Library.FFmpegDirectory = @"f:\ffmpeg" + (Environment.Is64BitProcess ? @"\x64" : string.Empty);

            // You can pick which FFmpeg binaries are loaded. See issue #28
            // For more specific control (issue #414) you can set Library.FFmpegLoadModeFlags to:
            // FFmpegLoadMode.LibraryFlags["avcodec"] | FFmpegLoadMode.LibraryFlags["avfilter"] | ... etc.
            // Full Features is already the default.
            Library.FFmpegLoadModeFlags = FFmpegLoadMode.FullFeatures;

            // Multi-threaded video enables the creation of independent
            // dispatcher threads to render video frames. This is an experimental feature
            // and might become deprecated in the future as no real performance enhancements
            // have been detected.
            Library.EnableWpfMultiThreadedVideo = false; // !System.Diagnostics.Debugger.IsAttached; // test with true and false
        }
    }
}
