using System;
using System.Diagnostics;
using System.Linq;

namespace Durty.OBS.Watcher.WinApi
{
    public class WindowApi
    {
        public static string GetActiveWindowTitle()
        {
            try
            {
                IntPtr handle = PInvoke.User32.GetForegroundWindow();
                int processId = PInvoke.Kernel32.GetProcessId(handle);
                Process process = Process.GetProcessById(processId);
                
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
