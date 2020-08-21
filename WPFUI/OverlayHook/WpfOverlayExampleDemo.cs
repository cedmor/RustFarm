using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Overlay.NET;
using Overlay.NET.Common;
using Process.NET;
using Process.NET.Memory;
using Process.NET.Native.Types;

namespace WPFUI.OverlayHook
{
    /// <summary>
    /// </summary>
    public class WpfOverlayExampleDemo
    {
        /// <summary>
        ///     The overlay
        /// </summary>
        private static OverlayPlugin _overlay;

        /// <summary>
        ///     The process sharp
        /// </summary>
        private static ProcessSharp _processSharp;

        /// <summary>
        ///     The work
        /// </summary>
        private static bool _work;

        /// <summary>
        ///     Starts the demo.
        /// </summary>
        public void StartDemo(Window mainUI)
        {
            var processName = "notepad";

            var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
            if (process == null)
            {
                Log.Warn($"No process by the name of {processName} was found.");
                Log.Warn("Please open one or use a different name and restart the demo.");
                Console.ReadLine();
                return;
            }

            _processSharp = new ProcessSharp(process, MemoryType.Remote);
            _overlay = new WpfOverlayDemoExample(mainUI);

            var wpfOverlay = (WpfOverlayDemoExample)_overlay;

            // This is done to focus on the fact the Init method
            // is overriden in the wpf overlay demo in order to set the
            // wpf overlay window instance
            wpfOverlay.Initialize(_processSharp.WindowFactory.MainWindow);
            wpfOverlay.Enable();

            _work = true;

            // Log some info about the overlay.
            Log.Debug("Starting update loop (open the process you specified and drag around)");
            Log.Debug("Update rate: " + wpfOverlay.Settings.Current.UpdateRate.Milliseconds());

            var info = wpfOverlay.Settings.Current;

            Log.Debug($"Author: {info.Author}");
            Log.Debug($"Description: {info.Description}");
            Log.Debug($"Name: {info.Name}");
            Log.Debug($"Identifier: {info.Identifier}");
            Log.Debug($"Version: {info.Version}");

            Log.Info("Note: Settings are saved to a settings folder in your main app folder.");

            Log.Info("Give your window focus to enable the overlay (and unfocus to disable..)");

            Log.Info("Close the console to end the demo.");

            wpfOverlay.OverlayWindow.Draw += OnDraw;

            //Do work
            while (_work)
            {
                _overlay.Update();
            }

            Log.Debug("Demo complete.");
        }

        private static void OnDraw(object sender, DrawingContext context)
        {
            //int taille1, taille2, posX, posY;
            //int.TryParse(ConfigurationManager.AppSettings["posX"], out posX);
            //int.TryParse(ConfigurationManager.AppSettings["posY"], out posY);
            //int.TryParse(ConfigurationManager.AppSettings["taille1"], out taille1);
            //int.TryParse(ConfigurationManager.AppSettings["taille2Draxuslol"], out taille2);


            //// Draw a formatted text string into the DrawingContext.
            //context.DrawEllipse(Brushes.White, new Pen(Brushes.White, 1),
            //    new System.Windows.Point(posX, posY), taille1, taille2);
        }
    }
}