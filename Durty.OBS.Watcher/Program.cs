using System;
using System.Collections.Generic;
using System.Linq;
using Durty.OBS.Watcher.Handlers;
using Durty.OBS.Watcher.Repositories;
using OBSWebsocketDotNet;

namespace Durty.OBS.Watcher
{
    /* TODO / IDEAS
    - Debug Mode to diagnose issues
    - On startup check all defined actions if scene & source names are valid
    - All actions should be defined in json files
    - Application configuration should be outsourced to configuration json
    - Maybe consider porting to .NET Console project, because of OBS /OBS WebSockets dependency & focused window change API

    - Focused Window Change title should support placeholders 'DurtyMapEditor%Visual Studio' to check for multiple contain or exact window title match

    - Change Scene Action on focused window changed
    -> Optional only enabled when in specific scene currently

    - Change Scene Source Render/Visibility on focused window changed
    -> Optional only enabled when in specific scene currently
    -> Only render source when specified window is focused
    -> Support changing source render toggle for all available scenes, not only current one

    - Capture Window with tooltips mode
    -> Creates / Uses display capture on monitor with defined window
    -> Toggles Display Capture source visibility whenever defined window is in focus
    -> When defined window is out of focus, hide display capture & show window capture
    -> Only change to display capture if defined window is focused for more than x seconds? (Safety / fast switch protection)
    -> Auto raise focused window watcher polling?
    -> Remove display capture with transform on source by adding 42 to transform size y (Obs transform extension method)
    -> Use System.Windows.Forms.Screen to retrieve working area of given display (without task bar)
     */

    class Program
    {
        public static bool DebugMode = true;

        public static string ServerIp = "127.0.0.1";
        public static int Port = 4444;
        public static string Password = "";
        
        static void Main(string[] args)
        {
            Console.Title = "Durtys OBS Watcher Tool";

            Console.WriteLine("Starting OBS Watcher...");
            var obsManager = new ObsManager(ServerIp, Port, Password);
            var focusedWindowChangeActionRepository = new FocusedWindowChangeActionRepository();
            var captureFullWindowActionRepository = new CaptureFullWindowActionRepository();
            var activeWindowWatcher = new ActiveWindowWatcher(100);
            activeWindowWatcher.Start();
            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
            var focusedWindowChangedHandler = new FocusedWindowChangedHandler(activeWindowWatcher, focusedWindowChangeActionRepository, obsManager);

            Console.WriteLine($"Trying to connect to WebSockets {ServerIp}:{Port} ...");
            
            if (!obsManager.Connect())
            {
                Console.WriteLine("Failed to connect.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Successfully connected.");
            List<OBSScene> scenes = obsManager.Obs.ListScenes();

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
        
        private static void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            if (DebugMode)
            {
                Console.WriteLine($"[DEBUG][FocusedWindowChange] {e.OldFocusedWindowTitle} -> {e.NewFocusedWindowTitle}");
            }
        }
    }
}
