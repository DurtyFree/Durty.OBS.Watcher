using System.Linq;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;
using Durty.OBS.Watcher.Services;
using OBSWebsocketDotNet;

namespace Durty.OBS.Watcher.Handlers
{
    public class FocusedWindowSceneSwitchHandler
        : IHandler
    {
        private readonly FocusedWindowSceneSwitchActionRepository _focusedWindowSceneSwitchActionRepository;
        private readonly WindowMatchService _windowMatchService;
        private readonly OBSWebsocket _obs;

        public FocusedWindowSceneSwitchHandler(
            FocusedWindowSceneSwitchActionRepository focusedWindowSceneSwitchActionRepository,
            ActiveWindowWatcher activeWindowWatcher,
            WindowMatchService windowMatchService,
            OBSWebsocket obs)
        {
            _focusedWindowSceneSwitchActionRepository = focusedWindowSceneSwitchActionRepository;
            _windowMatchService = windowMatchService;
            _obs = obs;

            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            FocusedWindowSceneSwitchAction foundChangeAction = _focusedWindowSceneSwitchActionRepository.GetAll()
                .FirstOrDefault(a => _windowMatchService.DoesTitleMatch(e.NewFocusedWindow.Title, a.WindowTitle));
            if (foundChangeAction == null)
                return;

            if (foundChangeAction.EnabledForSceneName != string.Empty && foundChangeAction.EnabledForSceneName != _obs.GetCurrentScene().Name)
                return;

            _obs.SetCurrentScene(foundChangeAction.SceneName);
        }
    }
}
