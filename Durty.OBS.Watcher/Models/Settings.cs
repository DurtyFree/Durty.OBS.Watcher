namespace Durty.OBS.Watcher.Models
{
    public class Settings
    {
        /// <summary>
        /// OBS WebSocket IP
        /// </summary>
        public string ObsWebSocketsIp { get; set; }

        /// <summary>
        /// OBS WebSocket Port
        /// </summary>
        public int ObsWebSocketsPort { get; set; }

        /// <summary>
        /// OBS WebSocket Authentication Password
        /// </summary>
        public string ObsWebSocketsAuthPassword { get; set; }

        /// <summary>
        /// Defines the delay between window changed checks in milliseconds
        /// Lower number = more response but might also be not performant
        /// Default is 100
        /// </summary>
        public int WindowWatcherPollingDelay { get; set; } = 100;

        /// <summary>
        /// Debug Mode, prints debug messages to console if enabled
        /// </summary>
        public bool DebugMode { get; set; }
    }
}
