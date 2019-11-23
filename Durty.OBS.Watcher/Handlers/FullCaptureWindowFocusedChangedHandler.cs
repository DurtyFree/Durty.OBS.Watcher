using System;
using System.Linq;
using System.Threading;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;
using Durty.OBS.Watcher.Services;
using OBS.WebSocket.NET;
using OBS.WebSocket.NET.Types;

namespace Durty.OBS.Watcher.Handlers
{
    public class FullCaptureWindowFocusedChangedHandler
        : IHandler
    {
        private readonly CaptureFullWindowActionRepository _captureFullWindowActionRepository;
        private readonly ObsWebSocketApi _obs;
        private readonly WindowMatchService _windowMatchService;
        private readonly ILogger _logger;

        private Timer _focusedCheckTimer;
        private bool _fullCaptureWindowSourceVisible;
        private WindowInfo _currentFullCaptureWindowInfo;
        private CaptureFullWindowAction _currentActiveFullCaptureAction;

        public FullCaptureWindowFocusedChangedHandler(
            ActiveWindowWatcher activeWindowWatcher,
            CaptureFullWindowActionRepository captureFullWindowActionRepository,
            ObsWebSocketApi obs,
            WindowMatchService windowMatchService,
            ILogger logger)
        {
            _captureFullWindowActionRepository = captureFullWindowActionRepository;
            _obs = obs;
            _windowMatchService = windowMatchService;
            _logger = logger;

            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
            activeWindowWatcher.FocusedWindowTrackLost += OnFocusedWindowTrackLost;
        }

        private void OnFocusedWindowTrackLost(object sender, EventArgs e)
        {
            if (_fullCaptureWindowSourceVisible)
            {
                ToggleObsSourceRender(_currentActiveFullCaptureAction.DisplayCaptureSourceName, false);
            }
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            if (_currentFullCaptureWindowInfo != null //Full capture window focus is only lost if we ever had it in focus
                && (
                    _currentActiveFullCaptureAction.AutoCaptureSubWindows && e.NewFocusedWindow.ProcessId != e.OldFocusedWindow.ProcessId //If new focused window is no parent of old focused window
                || !_currentActiveFullCaptureAction.AutoCaptureSubWindows && e.NewFocusedWindow.Title != _currentFullCaptureWindowInfo.Title //If new window title is not old window title
                )) 
            {
                OnFullCapturedWindowFocusLost();
                return;
            }

            //Abort "on focused" when we already are full capturing and the new focused window is our current full capture window
            if (_currentFullCaptureWindowInfo != null && _currentFullCaptureWindowInfo.Title == e.NewFocusedWindow.Title)
                return;

            foreach (CaptureFullWindowAction action in _captureFullWindowActionRepository.GetAll())
            {
                if (e.NewFocusedWindow.Title != string.Empty 
                    && _windowMatchService.DoesTitleMatch(e.NewFocusedWindow.Title, action.CaptureWindowTitle))
                {
                    OnFullCaptureWindowFocused(action, e.NewFocusedWindow);
                    break;
                }
            }
        }

        private void OnFullCaptureWindowFocused(CaptureFullWindowAction action, WindowInfo newFocusedWindowInfo)
        {
            _logger.Write(LogLevel.Debug, $"Full Capture Window focused, waiting {action.NeededWindowFocusTime} seconds...");

            _currentActiveFullCaptureAction = action;
            _currentFullCaptureWindowInfo = newFocusedWindowInfo;

            //Check if focus is still focused in configured time
            _focusedCheckTimer = new Timer(OnFullCaptureWindowReallyFocused, null, action.NeededWindowFocusTime * 1000, Timeout.Infinite);
        }

        private void OnFullCaptureWindowReallyFocused(object state)
        {
            _logger.Write(LogLevel.Info, $"Full Capture Window really focused, full capture '{_currentActiveFullCaptureAction.DisplayCaptureSourceName}' is now visible");

            if (!_fullCaptureWindowSourceVisible)
            {
                ToggleObsSourceRender(_currentActiveFullCaptureAction.DisplayCaptureSourceName, true);
            }
        }

        private void OnFullCapturedWindowFocusLost()
        {
            _logger.Write(LogLevel.Info, $"Full Capture Window focus lost, disabling '{_currentActiveFullCaptureAction.DisplayCaptureSourceName}'");

            //If capture window focus lost, kill timer
            _focusedCheckTimer.Dispose();
            _focusedCheckTimer = null;

            //Reset visibility of full capture source
            if (_fullCaptureWindowSourceVisible)
            {
                ToggleObsSourceRender(_currentActiveFullCaptureAction.DisplayCaptureSourceName, false);
            }

            _currentActiveFullCaptureAction = null;
            _currentFullCaptureWindowInfo = null;
        }

        private bool ToggleObsSourceRender(string captureSourceName, bool visible)
        {
            SceneItem obsDisplayCaptureSource = GetCurrentSceneFullDisplayCaptureSource(captureSourceName);
            if (obsDisplayCaptureSource.InternalType == null)
                return false;

            _obs.SetSourceRender(obsDisplayCaptureSource.SourceName, visible);
            _fullCaptureWindowSourceVisible = visible;
            return true;
        }

        private SceneItem GetCurrentSceneFullDisplayCaptureSource(string captureSourceName)
        {
            SceneItem obsDisplayCaptureSource = _obs.GetCurrentScene().Items.FirstOrDefault(i => i.SourceName == captureSourceName);
            if (obsDisplayCaptureSource?.InternalType == null || obsDisplayCaptureSource.InternalType != "monitor_capture")
                return null;

            return obsDisplayCaptureSource;
        }
    }
}
