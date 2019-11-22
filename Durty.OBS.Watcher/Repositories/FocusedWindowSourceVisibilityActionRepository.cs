using System.Collections.Generic;
using Durty.OBS.Watcher.Models;

namespace Durty.OBS.Watcher.Repositories
{
    public class FocusedWindowSourceVisibilityActionRepository
    {
        private readonly List<FocusedWindowSourceVisibilityAction> _focusedWindowSourceVisibilityActions = new List<FocusedWindowSourceVisibilityAction>()
        {
            new FocusedWindowSourceVisibilityAction()
            {
                WindowTitle = "DurtyMapEditor - Microsoft Visual Studio",
                SourceName = "AltV Dev",
                HideOnFocusLust = true
            }
        };

        public List<FocusedWindowSourceVisibilityAction> GetAll()
        {
            return _focusedWindowSourceVisibilityActions;
        }
    }
}
