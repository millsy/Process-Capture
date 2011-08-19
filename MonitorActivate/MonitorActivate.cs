using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.Diagnostics;
using ProcessCapture.Log;

namespace ProcessCapture.MonitorActivate
{
    public class MonitorActivate
    {
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint WINEVENT_SKIPOWNPROCESS = 2;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window is minimized 
            /// or maximized, the system restores it to its original size and 
            /// position. An application should specify this flag when displaying 
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position. 
            /// This value is similar to "ShowNormal", except the window is not 
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size 
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next 
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is 
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This 
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is 
            /// minimized or maximized, the system restores it to its original size 
            /// and position. An application should specify this flag when restoring 
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
            /// that owns the window is hung. This flag should only be used when 
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        IntPtr m_hhook = IntPtr.Zero;
        WinEventDelegate winEventProc;
        Delegate Callback;
        bool hooked = false;

        public MonitorActivate(Delegate callback)
        {
            Callback = callback;
            winEventProc = new WinEventDelegate(WinEventProc);
        }

        public void Start()
        {
            m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, winEventProc, 0, 0, WINEVENT_OUTOFCONTEXT);
            hooked = true;
        }

        public void Stop()
        {
            try
            {
                if (m_hhook != null && m_hhook != IntPtr.Zero && hooked)
                {
                    UnhookWinEvent(m_hhook);
                    hooked = false;
                }
            }
            catch (Exception err)
            {
                string file = Logger.GetInstance().Log(err);
                MessageBox.Show("An error has occurred - it has been saved to " + file + " - please email this to millsy@openspan.com");
            }
        }

        public bool IsRunning()
        {
            return hooked;
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                if (eventType == EVENT_SYSTEM_FOREGROUND && hwnd != thisAppHandle && hwnd != tray && hwnd != desktopHandle)
                {
                    Callback.DynamicInvoke(new object[]{hwnd});
                }
            }
            catch (Exception err)
            {
                //report error
                string file = Logger.GetInstance().Log(err);
                MessageBox.Show("An error has occurred - it has been saved to " + file + " - please email this to millsy@openspan.com");
            }
        }

        public static string GetWindowText()
        {
            IntPtr win = ProcessCapture.MonitorActivate.ScreenCapture.User32.GetForegroundWindow();
            if (win != null)
            {
                return GetWindowText(win);
            }
            return "";
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static string GetProcessPath()
        {
            IntPtr win = ProcessCapture.MonitorActivate.ScreenCapture.User32.GetForegroundWindow();
            return GetProcessPath(win);
        }

        public static string GetProcessPath(IntPtr hwnd)
        {
            uint processID = 0;
            GetWindowThreadProcessId(hwnd, out processID);

            if (processID > 0)
            {
                Process p = Process.GetProcessById((int)processID);
                return p.MainModule.FileName;
            }

            return "";
        }

        public static string[] GetProcessModules()
        {
            IntPtr win = ProcessCapture.MonitorActivate.ScreenCapture.User32.GetForegroundWindow();
            return GetProcessModules(win);
        }

        public static string[] GetProcessModules(IntPtr hwnd)
        {
            string[] result = null;
            uint processID = 0;
            GetWindowThreadProcessId(hwnd, out processID);

            if (processID > 0)
            {
                Process p = Process.GetProcessById((int)processID);
                result = new string[p.Modules.Count];
                int i = 0;
                foreach (ProcessModule m in p.Modules)
                {
                    result[i++] = m.FileName;
                }
            }
            return result;
        }
        
        const UInt32 WS_MINIMIZE = 0x20000000;
        const UInt32 WS_MAXIMIZE = 0x1000000;

        public static void SetWindowFocus()
        {
            SetWindowFocus(thisAppHandle);
        }

        public static void SetWindowFocus(IntPtr hwnd)
        {
            int state = GetWindowLong(hwnd, (-16));
            bool minimized = (state & WS_MINIMIZE) != 0;
            //bool maximized = (state & WS_MAXIMIZE) != 0;

            if (minimized)
            {
                ShowWindow(hwnd, WindowShowStyle.Restore);
            }
            
            SetForegroundWindow(hwnd);
        }

        public static IntPtr tray = FindWindow("Shell_TrayWnd", null);
        public static IntPtr desktopHandle = FindWindow("progman", null);
        public static IntPtr thisAppHandle
        {
            get
            {
                WindowInteropHelper winh = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
                return winh.Handle;
            }
        }
    }

    public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
}
