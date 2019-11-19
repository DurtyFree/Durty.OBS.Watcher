using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Durty.OBS.Watcher.WinApi
{
    public class WindowApi
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();
            return GetWindowText(handle, buff, nChars) > 0 
                ? buff.ToString() 
                : null;
        }
    }
}
