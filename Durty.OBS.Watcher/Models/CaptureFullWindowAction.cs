namespace Durty.OBS.Watcher.Models
{
    public class CaptureFullWindowAction
    {
        /// <summary>
        /// Full or part of the title name to capture.
        /// Example: Microsoft Visual Studio
        /// </summary>
        public string CaptureWindowTitle { get; set; }

        /// <summary>
        /// Name of the display capture source, needed to "full capture" the wanted window.
        /// IMPORTANT: Scene needs to exist / be created in before.
        /// Example: Full IDE Capture
        /// </summary>
        public string DisplayCaptureSourceName { get; set; }

        /// <summary>
        /// Time the window needs to be in focus to be "full captured" (in seconds).
        /// This is for safety reasons, to prevent windows beeing shown up whenever users "fast" switches between windows.
        /// Per default 2 seconds
        /// </summary>
        public int NeededWindowFocusTime { get; set; } = 2;

        /// <summary>
        /// If enabled, windows / processes spawned by the full captured main window will not end the full capture
        /// Per default enabled
        /// </summary>
        public bool AutoCaptureSubWindows { get; set; } = true;
    }
}
