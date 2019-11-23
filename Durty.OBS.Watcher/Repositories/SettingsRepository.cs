using System.IO;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Newtonsoft.Json;

namespace Durty.OBS.Watcher.Repositories
{
    public class SettingsRepository
    {
        private readonly ILogger _logger;
        private const string FileName = "settings.json";
        private readonly string _filePath;
        private readonly Settings _exampleData = new Settings()
        {
            ObsWebSocketsIp = "127.0.0.1",
            ObsWebSocketsPort = 4444,
            ObsWebSocketsAuthPassword = "",
            DebugMode = false,
            WindowWatcherPollingDelay = 100
        };

        private Settings _data;

        public SettingsRepository(
            string baseFolderPath,
            ILogger logger)
        {
            _logger = logger;
            _filePath = Path.Combine(baseFolderPath, FileName);

            Load();
        }

        public void Load()
        {
            //Create default / example file if not exists
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(_exampleData, Formatting.Indented));
                _data = _exampleData;
                _logger.Write(LogLevel.Info, $"Created example {FileName} config. Please configure it on your own.");
                return;
            }

            string fileContent = File.ReadAllText(_filePath);
            Settings settings = JsonConvert.DeserializeObject<Settings>(fileContent);
            _data = settings;
        }

        public Settings Get()
        {
            return _data;
        }
    }
}
