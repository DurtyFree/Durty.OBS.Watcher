using System.Linq;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;

namespace Durty.OBS.Watcher.Handlers
{
    public class FocusedWindowSourceVisibilityHandler
        : IHandler
    {
        private readonly ActiveWindowWatcher _activeWindowWatcher;
        private readonly FocusedWindowSourceVisibilityActionRepository _sourceVisibilityActionRepository;
        private readonly ObsManager _obsManager;

        public FocusedWindowSourceVisibilityHandler(
            ActiveWindowWatcher activeWindowWatcher, 
            FocusedWindowSourceVisibilityActionRepository sourceVisibilityActionRepository,
            ObsManager obsManager)
        {
            _activeWindowWatcher = activeWindowWatcher;
            _sourceVisibilityActionRepository = sourceVisibilityActionRepository;
            _obsManager = obsManager;

            _activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            FocusedWindowSourceVisibilityAction foundChangeAction = _sourceVisibilityActionRepository.GetAll()
                .FirstOrDefault(c => e.NewFocusedWindowTitle.Contains(c.WindowTitle));
            if(foundChangeAction == null)
                return;

            //if (foundChangeAction.EnabledForSceneName != string.Empty && foundChangeAction.EnabledForSceneName != _obsManager.Obs.GetCurrentScene().Name) return;

            //if (foundChangeAction.ToggleType == FocusedWindowChangeToggleType.Scene)
            //{
            //    _obsManager.Obs.SetCurrentScene(foundChangeAction.ToggleName);
            //}
            //else if(foundChangeAction.ToggleType == FocusedWindowChangeToggleType.Source)
            //{
            //    _obsManager.Obs.SetSourceRender(foundChangeAction.ToggleName, true);
            //}
        }
    }
}
