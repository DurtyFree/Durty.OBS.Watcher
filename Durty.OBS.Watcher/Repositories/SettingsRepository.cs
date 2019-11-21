using Durty.OBS.Watcher.Models;

namespace Durty.OBS.Watcher.Repositories
{
    public class SettingsRepository
    {
        public Settings Get()
        {
            return new Settings()
            {
                ObsWebSocketsIp = "127.0.0.1",
                ObsWebSocketsPort = 4444,
                ObsWebSocketsAuthPassword = "",
                DebugMode = true
            };
        }
    }
}
