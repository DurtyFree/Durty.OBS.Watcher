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

        private bool _sceneSwitched;
        private WindowInfo _currentFocusedWindowInfo;
        private FocusedWindowSceneSwitchAction _currentFocusAction;
        private string _previousSceneName;

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
            if (_currentFocusedWindowInfo != null //Full capture window focus is only lost if we ever had it in focus
                && e.NewFocusedWindow.Title != _currentFocusedWindowInfo.Title) //If new window title is not old window title
            {
                OnActionWindowFocusLost(e.NewFocusedWindow);
            }

            FocusedWindowSceneSwitchAction foundChangeAction = _focusedWindowSceneSwitchActionRepository.GetAll()
                .FirstOrDefault(a => _windowMatchService.DoesTitleMatch(e.NewFocusedWindow.Title, a.WindowTitle));
            if (foundChangeAction == null)
                return;

            OnActionWindowFocused(foundChangeAction, e.NewFocusedWindow);
        }

        private void OnActionWindowFocusLost(WindowInfo newFocusedWindow)
        {
            if (_sceneSwitched && _currentFocusAction.BackToPreviousScene)
            {
                _obs.SetCurrentScene(_previousSceneName);
                _previousSceneName = null;
                _sceneSwitched = false;
            }

            _currentFocusAction = null;
            _currentFocusedWindowInfo = null;
        }

        private void OnActionWindowFocused(FocusedWindowSceneSwitchAction action, WindowInfo newFocusedWindow)
        {
            if (action.EnabledForSceneName != string.Empty && action.EnabledForSceneName != _obs.GetCurrentScene().Name)
                return;

            _currentFocusAction = action;
            _currentFocusedWindowInfo = newFocusedWindow;

            if (!_sceneSwitched)
            {
                _previousSceneName = _obs.GetCurrentScene().Name;
                _obs.SetCurrentScene(_currentFocusAction.SceneName);
                _sceneSwitched = true;
            }
        }
    }
}
