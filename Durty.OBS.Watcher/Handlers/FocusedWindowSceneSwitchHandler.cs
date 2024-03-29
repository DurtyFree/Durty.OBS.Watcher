﻿using System.Linq;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;
using Durty.OBS.Watcher.Services;
using OBS.WebSocket.NET;

namespace Durty.OBS.Watcher.Handlers
{
    public class FocusedWindowSceneSwitchHandler
        : IHandler
    {
        private readonly FocusedWindowSceneSwitchActionRepository _focusedWindowSceneSwitchActionRepository;
        private readonly WindowMatchService _windowMatchService;
        private readonly ObsWebSocketApi _obs;
        private readonly ILogger _logger;

        private bool _sceneSwitched;
        private WindowInfo _currentFocusedWindowInfo;
        private FocusedWindowSceneSwitchAction _currentFocusAction;
        private string _previousSceneName;

        public FocusedWindowSceneSwitchHandler(
            FocusedWindowSceneSwitchActionRepository focusedWindowSceneSwitchActionRepository,
            ActiveWindowWatcher activeWindowWatcher,
            WindowMatchService windowMatchService,
            ObsWebSocketApi obs,
            ILogger logger)
        {
            _focusedWindowSceneSwitchActionRepository = focusedWindowSceneSwitchActionRepository;
            _windowMatchService = windowMatchService;
            _obs = obs;
            _logger = logger;

            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            if (_currentFocusedWindowInfo != null //Full capture window focus is only lost if we ever had it in focus
                && (
                    _currentFocusAction.IncludeSubWindows && e.NewFocusedWindow.ProcessId != e.OldFocusedWindow.ProcessId //If new focused window is no parent of old focused window
                    || !_currentFocusAction.IncludeSubWindows && e.NewFocusedWindow.Title != _currentFocusedWindowInfo.Title //If new window title is not old window title
                ))
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
                _logger.Write(LogLevel.Info, $"Scene Switch Window focus lost, switching '{_currentFocusAction.SceneName}' to previous scene '{_previousSceneName}'");
                _obs.SetCurrentScene(_previousSceneName);
                _previousSceneName = null;
                _sceneSwitched = false;
            }
            else
            {
                _logger.Write(LogLevel.Info, $"Scene Switch Window focus lost");
            }

            _currentFocusAction = null;
            _currentFocusedWindowInfo = null;
        }

        private void OnActionWindowFocused(FocusedWindowSceneSwitchAction action, WindowInfo newFocusedWindow)
        {
            string currentSceneName = _obs.GetCurrentScene().Name;
            if (action.EnabledForScenes.Count != 0 && !action.EnabledForScenes.Any(s => s == currentSceneName))
                return;
            if (action.DisabledForScenes.Count != 0 && action.DisabledForScenes.Any(s => s == currentSceneName))
                return;
            _logger.Write(LogLevel.Info, $"Scene Switch Window focused, switching '{action.SceneName}' to previous scene '{currentSceneName}'");

            _currentFocusAction = action;
            _currentFocusedWindowInfo = newFocusedWindow;

            if (!_sceneSwitched)
            {
                _previousSceneName = currentSceneName;
                _obs.SetCurrentScene(_currentFocusAction.SceneName);
                _sceneSwitched = true;
            }
        }
    }
}
