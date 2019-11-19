﻿using System;
using Durty.OBS.Watcher.Handlers;
using Durty.OBS.Watcher.Repositories;

namespace Durty.OBS.Watcher
{
    /* TODO / IDEAS
     - Debug Mode to diagnose issues
     - On startup check all defined actions if scene & source names are valid
     - All actions should be defined in json files
     - Application configuration should be outsourced to configuration json

     - Focused Window Change title should support placeholders 'DurtyMapEditor%Visual Studio' to check for multiple contain or exact window title match

     - Change Scene Action on focused window changed
     - Optional only enabled when in specific scene currently

     - Change Scene Source Render/Visibility on focused window changed
     - Optional only enabled when in specific scene currently
     - Only render source when specified window is focused
     - Support changing source render toggle for all available scenes, not only current one
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
            var activeWindowWatcher = new ActiveWindowWatcher();
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
            //List<OBSScene> scenes = obsManager.Obs.ListScenes();

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
