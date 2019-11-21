using System;
using System.Threading;
using System.Threading.Tasks;
using Durty.OBS.Watcher.WinApi;

namespace Durty.OBS.Watcher
{
    public class FocusedWindowTitleChangedEventArgs 
        : EventArgs
    {
        public string OldFocusedWindowTitle { get; set; }
        public string NewFocusedWindowTitle { get; set; }

    }

    public class ActiveWindowWatcher
    {
        private CancellationTokenSource _cancellationTokenSource;

        public int PollDelayMilliseconds { get; set; }
        public string FocusedWindowTitle { get; private set; }

        public event EventHandler<FocusedWindowTitleChangedEventArgs> FocusedWindowTitleChanged;

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
                    string newFocusedWindowTitle = WindowApi.GetActiveWindowTitle();

                    //Only trigger event & update when focused window title is different
                    if (newFocusedWindowTitle != null && newFocusedWindowTitle != FocusedWindowTitle)
                    {
                        FocusedWindowTitleChanged?.Invoke(this, new FocusedWindowTitleChangedEventArgs()
                        {
                            OldFocusedWindowTitle = FocusedWindowTitle,
                            NewFocusedWindowTitle = newFocusedWindowTitle
                        });
                        FocusedWindowTitle = newFocusedWindowTitle;
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
        }
    }
}
