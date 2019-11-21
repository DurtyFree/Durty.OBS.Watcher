using System;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Repositories;

namespace Durty.OBS.Watcher.Handlers
{
    public class FocusedWindowChangedDebugHandler
        : IHandler
    {
        private readonly SettingsRepository _settingsRepository;

        public FocusedWindowChangedDebugHandler(
            ActiveWindowWatcher activeWindowWatcher,
            SettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;

            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            if (_settingsRepository.Get().DebugMode)
            {
                Console.WriteLine($"[DEBUG][FocusedWindowChange] {e.OldFocusedWindowTitle} -> {e.NewFocusedWindowTitle}");
            }
        }
    }
}
