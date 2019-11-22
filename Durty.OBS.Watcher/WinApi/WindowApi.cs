using System;

namespace Durty.OBS.Watcher.WinApi
{
    public class WindowApi
    {

        public static int GetWindowProcessId(IntPtr windowHandle, out int processId)
        {
            processId = 0;

            try
            {
                int threadId = PInvoke.User32.GetWindowThreadProcessId(windowHandle, out processId);
                return threadId;
            }
            catch (Exception e)
            {
                //TODO: maybe log exception in debug/verbose?
            }

            return default;
        }
        
        public static string GetWindowTitle(IntPtr handle)
        {
            try
            {
                return PInvoke.User32.GetWindowText(handle);
            }
            catch (Exception e)
            {
                //TODO: maybe log exception in debug/verbose?
            }
            return null;
        }

        public static IntPtr GetForegroundWindow()
        {
            try
            {
                IntPtr handle = PInvoke.User32.GetForegroundWindow();
                return handle;
            }
            catch (Exception e)
            {
                //TODO: maybe log exception in debug/verbose?
            }
            return default;
        }
    }
}
