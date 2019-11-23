using System.Linq;
using System.Threading;
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
        private readonly ILogger _logger;

        private bool _actionSourceVisible;
        private WindowInfo _currentFocusedWindowInfo;
        private FocusedWindowSourceVisibilityAction _currentFocusAction;

        public FocusedWindowSourceVisibilityHandler(
            ActiveWindowWatcher activeWindowWatcher, 
            FocusedWindowSourceVisibilityActionRepository sourceVisibilityActionRepository,
            OBSWebsocket obs,
            WindowMatchService windowMatchService,
            ILogger logger)
        {
            _sourceVisibilityActionRepository = sourceVisibilityActionRepository;
            _obs = obs;
            _windowMatchService = windowMatchService;
            _logger = logger;
            
            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            if (_currentFocusedWindowInfo != null //Full capture window focus is only lost if we ever had it in focus
                && e.NewFocusedWindow.Title != _currentFocusedWindowInfo.Title) //If new window title is not old window title
            {
                OnActionWindowFocusLost(e.NewFocusedWindow);
            }

            FocusedWindowSourceVisibilityAction foundChangeAction = _sourceVisibilityActionRepository.GetAll()
                .FirstOrDefault(a => _windowMatchService.DoesTitleMatch(e.NewFocusedWindow.Title, a.WindowTitle));
            if(foundChangeAction == null)
                return;
            
            OnActionWindowFocused(foundChangeAction, e.NewFocusedWindow);
        }

        private void OnActionWindowFocusLost(WindowInfo newFocusedWindow)
        {
            if (_actionSourceVisible && _currentFocusAction.HideOnFocusLust)
            {
                _logger.Write(LogLevel.Info, $"Source Visibility Window focus lost, switching '{_currentFocusAction.SourceName}' visibility to invisible");
                _obs.SetSourceRender(_currentFocusAction.SourceName, false);
                _actionSourceVisible = false;
            }
            else
            {
                _logger.Write(LogLevel.Info, $"Source Visibility Window focus lost");
            }

            _currentFocusAction = null;
            _currentFocusedWindowInfo = null;
        }

        private void OnActionWindowFocused(FocusedWindowSourceVisibilityAction action, WindowInfo newFocusedWindow)
        {
            if (action.EnabledForSceneName != string.Empty && action.EnabledForSceneName != _obs.GetCurrentScene().Name)
                return;
            _logger.Write(LogLevel.Info, $"Source Visibility Window focused, switching '{_currentFocusAction.SourceName}' visibility to visible");

            _currentFocusAction = action;
            _currentFocusedWindowInfo = newFocusedWindow;

            if (!_actionSourceVisible)
            {
                _obs.SetSourceRender(action.SourceName, true);
                _actionSourceVisible = true;
            }
        }
    }
}
