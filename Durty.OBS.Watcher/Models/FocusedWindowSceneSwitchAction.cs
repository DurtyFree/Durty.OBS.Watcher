using System.Collections.Generic;

namespace Durty.OBS.Watcher.Models
{
    public class FocusedWindowSceneSwitchAction
    {
        /// <summary>
        /// Full or part of window title that triggers scene switch on focus
        /// Example: GTA V
        /// </summary>
        public string WindowTitle { get; set; }
        
        /// <summary>
        /// Scene name to be switched to
        /// </summary>
        public string SceneName { get; set; }

        /// <summary>
        /// Defines which scene(s) must be selected for this action to be enabled
        /// Optional
        /// </summary>
        public List<string> EnabledForScenes { get; set; }

        /// <summary>
        /// Defines which scene(s) must not be selected for this action to be enabled
        /// Optional
        /// </summary>
        public List<string> DisabledForScenes { get; set; }

        /// <summary>
        /// When enabled, scene is switched back to previously selected scene on given window focus lost
        /// </summary>
        public bool BackToPreviousScene { get; set; }
    }
}
