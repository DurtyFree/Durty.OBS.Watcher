using System.Linq;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;
using Durty.OBS.Watcher.Services;
using OBSWebsocketDotNet;

namespace Durty.OBS.Watcher.Handlers
{
    public class FocusedWindowSourceVisibilityHandler
        : IHandler
    {
        private readonly FocusedWindowSourceVisibilityActionRepository _sourceVisibilityActionRepository;
        private readonly OBSWebsocket _obs;
        private readonly WindowMatchService _windowMatchService;

        public FocusedWindowSourceVisibilityHandler(
            ActiveWindowWatcher activeWindowWatcher, 
            FocusedWindowSourceVisibilityActionRepository sourceVisibilityActionRepository,
            OBSWebsocket obs,
            WindowMatchService windowMatchService)
        {
            _sourceVisibilityActionRepository = sourceVisibilityActionRepository;
            _obs = obs;
            _windowMatchService = windowMatchService;

            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            FocusedWindowSourceVisibilityAction foundChangeAction = _sourceVisibilityActionRepository.GetAll()
                .FirstOrDefault(a => _windowMatchService.DoesTitleMatch(e.NewFocusedWindow.Title, a.WindowTitle));
            if(foundChangeAction == null)
                return;

            if (foundChangeAction.EnabledForSceneName != string.Empty && foundChangeAction.EnabledForSceneName != _obs.GetCurrentScene().Name) 
                return;

            _obs.SetSourceRender(foundChangeAction.SourceName, true);
        }
    }
}
