using System;
using System.Threading;
using System.Threading.Tasks;
using Durty.OBS.Watcher.WinApi;

namespace Durty.OBS.Watcher
{
    public class WindowInfo
    {
        public string Title { get; set; }

        public int ProcessId { get; set; }

        public int ThreadId { get; set; }

        public IntPtr Handle { get; set; }
    }

    public class FocusedWindowTitleChangedEventArgs 
        : EventArgs
    {
        public WindowInfo OldFocusedWindow { get; set; }
        public WindowInfo NewFocusedWindow { get; set; }
    }

    public class ActiveWindowWatcher
    {
        private CancellationTokenSource _cancellationTokenSource;

        public int PollDelayMilliseconds { get; set; }
        public WindowInfo FocusedWindowInfo { get; private set; }

        public event EventHandler<FocusedWindowTitleChangedEventArgs> FocusedWindowTitleChanged;
        public event EventHandler FocusedWindowTrackLost;

        public ActiveWindowWatcher(int pollDelayMilliseconds = 1000)
        {
            PollDelayMilliseconds = pollDelayMilliseconds;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            Task listener = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    IntPtr newFocusedWindowHandle = WindowApi.GetForegroundWindow();
                    string newFocusedWindowTitle = WindowApi.GetWindowTitle(newFocusedWindowHandle);

                    //Only trigger event & update when focused window title is different
                    if (newFocusedWindowTitle != null && newFocusedWindowTitle != FocusedWindowInfo?.Title)
                    {
                        int threadId = WindowApi.GetWindowProcessId(newFocusedWindowHandle, out int processId);
                        WindowInfo newFocusedWindowInfo = new WindowInfo()
                        {
                            Title = newFocusedWindowTitle,
                            Handle = newFocusedWindowHandle,
                            ProcessId = processId,
                            ThreadId = threadId
                        };
                        FocusedWindowTitleChanged?.Invoke(this, new FocusedWindowTitleChangedEventArgs()
                        {
                            OldFocusedWindow = FocusedWindowInfo,
                            NewFocusedWindow = newFocusedWindowInfo
                        });
                        FocusedWindowInfo = newFocusedWindowInfo;
                    }

                    Thread.Sleep(PollDelayMilliseconds);
                    if (token.IsCancellationRequested)
                        break;
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            FocusedWindowTrackLost?.Invoke(this, EventArgs.Empty);
        }
    }
}
