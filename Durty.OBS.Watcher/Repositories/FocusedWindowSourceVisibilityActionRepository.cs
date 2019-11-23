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

        public List<FocusedWindowSourceVisibilityAction> GetAll()
        {
            return _focusedWindowSourceVisibilityActions;
        }
    }
}
