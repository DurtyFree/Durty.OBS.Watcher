namespace Durty.OBS.Watcher.Models
{
    public enum FocusedWindowChangeToggleType
    {
        Scene,
        Source
    }

    public class FocusedWindowChangeAction
    {
        public string WindowTitle { get; set; }

        public FocusedWindowChangeToggleType ToggleType { get; set; }

        /// <summary>
        /// Scene or Source name to be toggled
        /// </summary>
        public string ToggleName { get; set; }

        /// <summary>
        /// Defines which scene must be selected for this action to be enabled
        /// </summary>
        public string EnabledForScene { get; set; }
    }
}
