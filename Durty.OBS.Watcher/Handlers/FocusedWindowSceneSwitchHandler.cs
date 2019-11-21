using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Repositories;

namespace Durty.OBS.Watcher.Handlers
{
    public class FocusedWindowSceneSwitchHandler
        : IHandler
    {
        private readonly FocusedWindowSceneSwitchActionRepository _focusedWindowSceneSwitchActionRepository;

        public FocusedWindowSceneSwitchHandler(
            FocusedWindowSceneSwitchActionRepository focusedWindowSceneSwitchActionRepository,
            ActiveWindowWatcher activeWindowWatcher)
        {
            _focusedWindowSceneSwitchActionRepository = focusedWindowSceneSwitchActionRepository;

            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            
        }
    }
}
