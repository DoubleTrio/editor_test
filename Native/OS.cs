using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Avalonia;
using Avalonia.Controls;

namespace AvaloniaTest.Native
{
    public static partial class OS
    {
        public interface IBackend
        {
            void SetupApp(AppBuilder builder);
            void SetupWindow(Window window);
        }

        

        public static bool UseSystemWindowFrame
        {
            get => OperatingSystem.IsLinux() && _enableSystemWindowFrame;
            set => _enableSystemWindowFrame = value;
        }

        static OS()
        {
            if (OperatingSystem.IsWindows())
            {
                _backend = new Windows();
            }
            else if (OperatingSystem.IsMacOS())
            {
                _backend = new MacOS();
            }
            else if (OperatingSystem.IsLinux())
            {
                _backend = new Linux();
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public static void SetupApp(AppBuilder builder)
        {
            _backend.SetupApp(builder);
        }
        

        public static void SetupForWindow(Window window)
        {
            _backend.SetupWindow(window);
        }
        

        private static IBackend _backend = null;
        private static bool _enableSystemWindowFrame = false;
    }
}
