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
        /// Per default 2 seconds
        /// </summary>
        public int NeededWindowFocusTime { get; set; } = 2;
    }
}
