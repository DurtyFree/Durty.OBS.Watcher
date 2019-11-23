using System.Collections.Generic;
using System.IO;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Newtonsoft.Json;

namespace Durty.OBS.Watcher.Repositories
{
    public class FocusedWindowSceneSwitchActionRepository
    {
        private readonly ILogger _logger;
        private const string FileName = "sceneActions.json";
        private readonly string _filePath;
        private readonly List<FocusedWindowSceneSwitchAction> _exampleData = new List<FocusedWindowSceneSwitchAction>()
        {
            new FocusedWindowSceneSwitchAction()
            {
                WindowTitle = "Opera",
                SceneName = "AltV Dev Scene",
                BackToPreviousScene = true,
                EnabledForScenes = new List<string>()
                {
                    "Pause Scene"
                },
                DisabledForScenes = new List<string>(),
                IncludeSubWindows = true
            }
        };

        private List<FocusedWindowSceneSwitchAction> _data;

        public FocusedWindowSceneSwitchActionRepository(
            string baseFolderPath,
            ILogger logger)
        {
            _logger = logger;
            _filePath = Path.Combine(baseFolderPath, FileName);

            Load();
        }

        public List<FocusedWindowSceneSwitchAction> GetAll()
        {
            return _data;
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
            List<FocusedWindowSceneSwitchAction> data = JsonConvert.DeserializeObject<List<FocusedWindowSceneSwitchAction>>(fileContent);
            _data = data;
        }
    }
}
