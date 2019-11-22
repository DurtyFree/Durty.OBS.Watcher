using System;
using Durty.OBS.Watcher.Contracts;
using Durty.OBS.Watcher.Repositories;

namespace Durty.OBS.Watcher.Handlers
{
    public class FocusedWindowChangedDebugHandler
        : IHandler
    {
        private readonly SettingsRepository _settingsRepository;
        private readonly ILogger _logger;

        public FocusedWindowChangedDebugHandler(
            ActiveWindowWatcher activeWindowWatcher,
            SettingsRepository settingsRepository,
            ILogger logger)
        {
            _settingsRepository = settingsRepository;
            _logger = logger;

            activeWindowWatcher.FocusedWindowTitleChanged += OnFocusedWindowTitleChanged;
        }

        private void OnFocusedWindowTitleChanged(object sender, FocusedWindowTitleChangedEventArgs e)
        {
            if (_settingsRepository.Get().DebugMode)
            {
                _logger.Write(LogLevel.Debug, $"[FocusedWindowChange][Old] {e.OldFocusedWindow.Title} [{e.OldFocusedWindow.ProcessId}] [{e.OldFocusedWindow.ThreadId}]");
                _logger.Write(LogLevel.Debug, $"[FocusedWindowChange][New] {e.NewFocusedWindow.Title} [{e.NewFocusedWindow.ProcessId}] [{e.NewFocusedWindow.ThreadId}]");
            }
        }
    }
}
