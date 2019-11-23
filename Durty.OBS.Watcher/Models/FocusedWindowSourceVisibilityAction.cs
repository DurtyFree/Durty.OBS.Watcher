using System.Collections.Generic;

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
        public string SourceName { get; set; }

        /// <summary>
        /// Defines which scene(s) must be selected for this action to be enabled
        /// Optional, if none defined it will always check for given source name in current scene
        /// </summary>
        public List<string> EnabledForScenes { get; set; }

        /// <summary>
        /// Defines which scene(s) must not be selected for this action to be enabled
        /// Optional
        /// </summary>
        public List<string> DisabledForScenes { get; set; }

        /// <summary>
        /// When enabled, source is hide on given window focus lost
        /// </summary>
        public bool HideOnFocusLust { get; set; }
    }
}