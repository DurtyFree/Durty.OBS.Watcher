using System;
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
        private bool _fullCaptureWindowInFocus;
        private Timer _focusedCheckTimer;
        private bool _fullCaptureWindowSourceVisible;

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
            List<CaptureFullWindowAction> actions = _captureFullWindowActionRepository
                .GetAll();

            foreach (CaptureFullWindowAction action in actions)
            {
                if (e.NewFocusedWindowTitle != string.Empty &&
                    _windowMatchService.DoesTitleMatch(e.NewFocusedWindowTitle, action.CaptureWindowTitle))
                {
                    OnFullCaptureWindowFocused(action);
                    break;
                }

                if (e.OldFocusedWindowTitle != string.Empty && _fullCaptureWindowInFocus &&
                    _windowMatchService.DoesTitleMatch(e.OldFocusedWindowTitle, action.CaptureWindowTitle))
                {
                    OnFullCapturedWindowFocusLost(action);
                    break;
                }
            }
        }

        private void OnFullCapturedWindowFocusLost(CaptureFullWindowAction action)
        {
            SceneItem obsDisplayCaptureSource = GetCurrentSceneFullDisplayCaptureSource(action);
            if (obsDisplayCaptureSource.InternalType == null)
                return;

            //If capture window focus lost, kill timer
            _focusedCheckTimer.Dispose();
            _focusedCheckTimer = null;

            _fullCaptureWindowInFocus = false;

            //Reset visibility of full capture source
            if (_fullCaptureWindowSourceVisible)
            {
                _obs.SetSourceRender(obsDisplayCaptureSource.SourceName, false);
                _fullCaptureWindowSourceVisible = false;
            }
        }

        private void OnFullCaptureWindowFocused(CaptureFullWindowAction action)
        {
            SceneItem obsDisplayCaptureSource = GetCurrentSceneFullDisplayCaptureSource(action);
            if (obsDisplayCaptureSource.InternalType == null)
                return;
            _logger.Write(LogLevel.Debug, $"Full Capture Window focused, waiting {action.NeededWindowFocusTime} seconds...");

            _fullCaptureWindowInFocus = true;

            //Check if focus is still focused in configured time
            _focusedCheckTimer = new Timer(OnFullCaptureWindowReallyFocused, obsDisplayCaptureSource, action.NeededWindowFocusTime * 1000, Timeout.Infinite);
        }

        private void OnFullCaptureWindowReallyFocused(object state)
        {
            SceneItem sceneItem = (SceneItem) state;
            
            _obs.SetSourceRender(sceneItem.SourceName, true);
            _fullCaptureWindowSourceVisible = true;
            _logger.Write(LogLevel.Debug, "Full Capture Window really focused, full capture is now visible");
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
