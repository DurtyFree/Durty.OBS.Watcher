using System.Collections.Generic;
using System.IO;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Newtonsoft.Json;

namespace Durty.OBS.Watcher.Repositories
{
    public class CaptureFullWindowActionRepository
    {
        private readonly ILogger _logger;
        private const string FileName = "fullCaptureActions.json";
        private readonly string _filePath;
        private readonly List<CaptureFullWindowAction> _exampleData = new List<CaptureFullWindowAction>()
        {
            new CaptureFullWindowAction()
            {
                CaptureWindowTitle = "Microsoft Visual Studio",
                DisplayCaptureSourceName = "All IDE Capture",
                NeededWindowFocusTime = 2,
                AutoCaptureSubWindows = true
            }
        };

        private List<CaptureFullWindowAction> _data;

        public CaptureFullWindowActionRepository(
            string baseFolderPath, 
            ILogger logger)
        {
            _logger = logger;
            _filePath = Path.Combine(baseFolderPath, FileName);

            Load();
        }

        public List<CaptureFullWindowAction> GetAll()
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
            List<CaptureFullWindowAction> data = JsonConvert.DeserializeObject<List<CaptureFullWindowAction>>(fileContent);
            _data = data;
        }
    }
}
