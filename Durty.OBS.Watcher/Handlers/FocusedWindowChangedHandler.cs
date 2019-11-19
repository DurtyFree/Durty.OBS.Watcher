using System.Linq;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;

namespace Durty.OBS.Watcher.Handlers
{
    public class FocusedWindowChangedHandler
    {
        private readonly ActiveWindowWatcher _activeWindowWatcher;
        private readonly FocusedWindowChangeActionRepository _changeActionRepository;
        private readonly ObsManager _obsManager;

        public FocusedWindowChangedHandler(
            ActiveWindowWatcher activeWindowWatcher, 
            FocusedWindowChangeActionRepository changeActionRepository,
            ObsManager obsManager)
        {
            _activeWindowWatcher = activeWindowWatcher;
            _changeActionRepository = changeActionRepository;
            _obsManager = obsManager;

            _activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            FocusedWindowChangeAction foundChangeAction = _changeActionRepository.GetAll()
                .FirstOrDefault(c => e.NewFocusedWindowTitle.Contains(c.WindowTitle));
            if(foundChangeAction == null)
                return;

            if (foundChangeAction.EnabledForScene != string.Empty && foundChangeAction.EnabledForScene != _obsManager.Obs.GetCurrentScene().Name) return;

            if (foundChangeAction.ToggleType == FocusedWindowChangeToggleType.Scene)
            {
                _obsManager.Obs.SetCurrentScene(foundChangeAction.ToggleName);
            }
            else if(foundChangeAction.ToggleType == FocusedWindowChangeToggleType.Source)
            {
                _obsManager.Obs.SetSourceRender(foundChangeAction.ToggleName, true);
            }
        }
    }
}
