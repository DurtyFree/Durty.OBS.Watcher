namespace Durty.OBS.Watcher.Models
{
    public class FocusedWindowSourceVisibilityAction
    {
        /// <summary>
        /// Full or part of window title that triggers source visibility change on focus
        /// Example: GTA V
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Source name that should be visible whenever given window is focused
        /// </summary>
        public string SceneName { get; set; }
        
        /// <summary>
        /// When enabled, source is hide on given window focus lost
        /// </summary>
        public bool HideOnFocusLust { get; set; }
    }
}
