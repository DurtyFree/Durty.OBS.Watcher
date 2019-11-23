using System.Collections.Generic;
using Durty.OBS.Watcher.Models;

namespace Durty.OBS.Watcher.Repositories
{
    public class FocusedWindowSceneSwitchActionRepository
    {
        private readonly List<FocusedWindowSceneSwitchAction> _focusedWindowSceneSwitchActions = new List<FocusedWindowSceneSwitchAction>()
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

        public List<FocusedWindowSceneSwitchAction> GetAll()
        {
            return _focusedWindowSceneSwitchActions;
        }
    }
}
