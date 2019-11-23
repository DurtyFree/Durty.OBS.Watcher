using System.Linq;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Models;
using Durty.OBS.Watcher.Repositories;
using Durty.OBS.Watcher.Services;
using OBS.WebSocket.NET;

namespace Durty.OBS.Watcher.Handlers
{
    public class FocusedWindowSourceVisibilityHandler
        : IHandler
    {
        private readonly FocusedWindowSourceVisibilityActionRepository _sourceVisibilityActionRepository;
        private readonly ObsWebSocketApi _obs;
        private readonly WindowMatchService _windowMatchService;
        private readonly ILogger _logger;

        private bool _actionSourceVisible;
        private WindowInfo _currentFocusedWindowInfo;
        private FocusedWindowSourceVisibilityAction _currentFocusAction;

        public FocusedWindowSourceVisibilityHandler(
            ActiveWindowWatcher activeWindowWatcher, 
            FocusedWindowSourceVisibilityActionRepository sourceVisibilityActionRepository,
            ObsWebSocketApi obs,
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
            string currentSceneName = _obs.GetCurrentScene().Name;
            if (action.EnabledForScenes.Count != 0 && !action.EnabledForScenes.Any(s => s == currentSceneName))
                return;
            if (action.DisabledForScenes.Count != 0 && action.DisabledForScenes.Any(s => s == currentSceneName))
                return;
            _logger.Write(LogLevel.Info, $"Source Visibility Window focused, switching '{action.SourceName}' visibility to visible");

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
