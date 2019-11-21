using System.Collections.Generic;
using System.Linq;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;
using OBSWebsocketDotNet;

namespace Durty.OBS.Watcher.Handlers
{
    public class FullCaptureWindowFocusedChangedHandler
        : IHandler
    {
        private readonly CaptureFullWindowActionRepository _captureFullWindowActionRepository;
        private readonly OBSWebsocket _obs;
        private readonly ILogger _logger;
        private bool _fullCaptureWindowInFocus;

        public FullCaptureWindowFocusedChangedHandler(
            ActiveWindowWatcher activeWindowWatcher,
            CaptureFullWindowActionRepository captureFullWindowActionRepository,
            OBSWebsocket obs,
            ILogger logger)
        {
            _captureFullWindowActionRepository = captureFullWindowActionRepository;
            _obs = obs;
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
                    DoesTitleMatch(e.NewFocusedWindowTitle, action.CaptureWindowTitle))
                {
                    OnFullCaptureWindowFocused(action);
                    break;
                }

                if (e.OldFocusedWindowTitle != string.Empty && _fullCaptureWindowInFocus &&
                    DoesTitleMatch(e.OldFocusedWindowTitle, action.CaptureWindowTitle))
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

            _obs.SetSourceRender(obsDisplayCaptureSource.SourceName, false);
            _fullCaptureWindowInFocus = false;
        }

        private void OnFullCaptureWindowFocused(CaptureFullWindowAction action)
        {
            SceneItem obsDisplayCaptureSource = GetCurrentSceneFullDisplayCaptureSource(action);
            if (obsDisplayCaptureSource.InternalType == null)
                return;

            _obs.SetSourceRender(obsDisplayCaptureSource.SourceName, true);
            _fullCaptureWindowInFocus = true;
        }

        private SceneItem GetCurrentSceneFullDisplayCaptureSource(CaptureFullWindowAction action)
        {
            SceneItem obsDisplayCaptureSource = _obs.GetCurrentScene().Items.FirstOrDefault(i => i.SourceName == action.DisplayCaptureSourceName);
            if (obsDisplayCaptureSource.InternalType == null || obsDisplayCaptureSource.InternalType != "monitor_capture")
                return default;

            return obsDisplayCaptureSource;
        }

        private bool DoesTitleMatch(string fullTitle, string titleSearch)
        {
            //TODO: Implement super crazy search / contains / placeholder/ variables thing
            //"Durtys OBS Watcher - Microsoft Visual Studio" Matches with "Durtys%Watcher%Microsoft Visual Studio"
            return fullTitle.Contains(titleSearch);
        }
    }
}
