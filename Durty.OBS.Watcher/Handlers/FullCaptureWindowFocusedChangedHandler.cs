using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;
using Durty.OBS.Watcher.Services;
using OBSWebsocketDotNet;

namespace Durty.OBS.Watcher.Handlers
{
    public class FullCaptureWindowFocusedChangedHandler
        : IHandler
    {
        private readonly CaptureFullWindowActionRepository _captureFullWindowActionRepository;
        private readonly OBSWebsocket _obs;
        private readonly WindowMatchService _windowMatchService;
        private readonly ILogger _logger;
        private Timer _focusedCheckTimer;
        private bool _fullCaptureWindowSourceVisible;
        private WindowInfo _currentFullCaptureWindowInfo;
        private CaptureFullWindowAction _currentActiveFullCaptureAction;

        public FullCaptureWindowFocusedChangedHandler(
            ActiveWindowWatcher activeWindowWatcher,
            CaptureFullWindowActionRepository captureFullWindowActionRepository,
            OBSWebsocket obs,
            WindowMatchService windowMatchService,
            ILogger logger)
        {
            _captureFullWindowActionRepository = captureFullWindowActionRepository;
            _obs = obs;
            _windowMatchService = windowMatchService;
            _logger = logger;

            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
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

            List<CaptureFullWindowAction> actions = _captureFullWindowActionRepository
                .GetAll();

            foreach (CaptureFullWindowAction action in actions)
            {
                if (e.NewFocusedWindow.Title != string.Empty 
                    && _windowMatchService.DoesTitleMatch(e.NewFocusedWindow.Title, action.CaptureWindowTitle))
                {
                    OnFullCaptureWindowFocused(action, e.NewFocusedWindow);
                    break;
                }
            }
        }

        private void OnFullCapturedWindowFocusLost()
        {
            SceneItem obsDisplayCaptureSource = GetCurrentSceneFullDisplayCaptureSource(_currentActiveFullCaptureAction);
            if (obsDisplayCaptureSource.InternalType == null)
                return;
            _logger.Write(LogLevel.Debug, $"Full Capture Window focus lost");

            //If capture window focus lost, kill timer
            _focusedCheckTimer.Dispose();
            _focusedCheckTimer = null;

            _currentActiveFullCaptureAction = null;
            _currentFullCaptureWindowInfo = null;
            
            //Reset visibility of full capture source
            if (_fullCaptureWindowSourceVisible)
            {
                _obs.SetSourceRender(obsDisplayCaptureSource.SourceName, false);
                _fullCaptureWindowSourceVisible = false;
            }
        }

        private void OnFullCaptureWindowFocused(CaptureFullWindowAction action, WindowInfo newFocusedWindowInfo)
        {
            SceneItem obsDisplayCaptureSource = GetCurrentSceneFullDisplayCaptureSource(action);
            if (obsDisplayCaptureSource.InternalType == null)
                return;
            _logger.Write(LogLevel.Debug, $"Full Capture Window focused, waiting {action.NeededWindowFocusTime} seconds...");

            _currentActiveFullCaptureAction = action;
            _currentFullCaptureWindowInfo = newFocusedWindowInfo;

            //Check if focus is still focused in configured time
            _focusedCheckTimer = new Timer(OnFullCaptureWindowReallyFocused, obsDisplayCaptureSource, action.NeededWindowFocusTime * 1000, Timeout.Infinite);
        }

        private void OnFullCaptureWindowReallyFocused(object state)
        {
            _logger.Write(LogLevel.Debug, "Full Capture Window really focused, full capture is now visible");

            SceneItem sceneItem = (SceneItem) state;
            if (!_fullCaptureWindowSourceVisible)
            {
                _obs.SetSourceRender(sceneItem.SourceName, true);
                _fullCaptureWindowSourceVisible = true;
            }
        }

        private SceneItem GetCurrentSceneFullDisplayCaptureSource(CaptureFullWindowAction action)
        {
            SceneItem obsDisplayCaptureSource = _obs.GetCurrentScene().Items.FirstOrDefault(i => i.SourceName == action.DisplayCaptureSourceName);
            if (obsDisplayCaptureSource.InternalType == null || obsDisplayCaptureSource.InternalType != "monitor_capture")
                return default;

            return obsDisplayCaptureSource;
        }
    }
}
