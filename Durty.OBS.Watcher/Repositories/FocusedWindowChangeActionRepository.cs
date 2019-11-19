using System.Collections.Generic;
using Durty.OBS.Watcher.Models;

namespace Durty.OBS.Watcher.Repositories
{
    public class FocusedWindowChangeActionRepository
    {
        private readonly List<FocusedWindowChangeAction> _focusedWindowChangeActions = new List<FocusedWindowChangeAction>()
        {
            new FocusedWindowChangeAction()
            {
                WindowTitle = "DurtyMapEditor - Microsoft Visual Studio",
                ToggleType = FocusedWindowChangeToggleType.Source,
                ToggleName = "Full IDE Capture",
                EnabledForScene = "AltV Dev"
            }
        };

        public List<FocusedWindowChangeAction> GetAll()
        {
            return _focusedWindowChangeActions;
        }
    }
}
