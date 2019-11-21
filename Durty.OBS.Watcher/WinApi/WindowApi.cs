using System;

namespace Durty.OBS.Watcher.WinApi
{
    public class WindowApi
    {
        public static string GetActiveWindowTitle()
        {
            try
            {
                IntPtr handle = PInvoke.User32.GetForegroundWindow();
                return PInvoke.User32.GetWindowText(handle);
            }
            catch (Exception e)
            {
                //TODO: maybe log exception in debug/verbose?
            }
            return null;
        }
    }
}
