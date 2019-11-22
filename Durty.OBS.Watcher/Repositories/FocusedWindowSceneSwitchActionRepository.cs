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
                SceneName = "Pause Scene",
                BackToPreviousScene = true,
                EnabledForSceneName = ""
            }
        };

        public List<FocusedWindowSceneSwitchAction> GetAll()
        {
            return _focusedWindowSceneSwitchActions;
        }
    }
}
