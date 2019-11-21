namespace Durty.OBS.Watcher.Models
{
    public class Settings
    {
        public string ObsWebSocketsIp { get; set; }

        public int ObsWebSocketsPort { get; set; }

        public string ObsWebSocketsAuthPassword { get; set; }

        public bool DebugMode { get; set; }
    }
}
