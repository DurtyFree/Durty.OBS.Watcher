using System.Collections.Generic;
using System.IO;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Newtonsoft.Json;

namespace Durty.OBS.Watcher.Repositories
{
    public class FocusedWindowSourceVisibilityActionRepository
    {
        private readonly ILogger _logger;
        private const string FileName = "sourceVisibilityActions.json";
        private readonly string _filePath;
        private readonly List<FocusedWindowSourceVisibilityAction> _exampleData = new List<FocusedWindowSourceVisibilityAction>()
        {
            new FocusedWindowSourceVisibilityAction()
            {
                WindowTitle = "Opera",
                SourceName = "Opera Browser",
                EnabledForScenes = new List<string>()
                {
                    "AltV Dev Scene"
                },
                DisabledForScenes = new List<string>()
                {
                    "Pause Scene"
                },
                HideOnFocusLost = true
            }
        };
        
        private List<FocusedWindowSourceVisibilityAction> _data;

        public FocusedWindowSourceVisibilityActionRepository(
            string baseFolderPath,
            ILogger logger)
        {
            _logger = logger;
            _filePath = Path.Combine(baseFolderPath, FileName);

            Load();
        }

        public List<FocusedWindowSourceVisibilityAction> GetAll()
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
            List<FocusedWindowSourceVisibilityAction> data = JsonConvert.DeserializeObject<List<FocusedWindowSourceVisibilityAction>>(fileContent);
            _data = data;
        }
    }
}
