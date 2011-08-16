using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using Interop.shdocvw;
using System.Collections;

namespace ProcessCapture.MonitorActivate
{
    public class WindowMgr
    {
        public WindowMgr()
        {
            callBackPtr = new CallBackPtr(Report);  
        }

        public delegate bool CallBackPtr(int hwnd, int lParam);

        private CallBackPtr callBackPtr;

        [DllImport("user32.dll")]
        private static extern int EnumWindows(CallBackPtr callPtr, int lPar);
        [DllImport("user32", EntryPoint = "GetWindowLongA")]
        private static extern int GetWindowLongPtr(int hwnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, CallBackPtr lpEnumCallbackFunction, IntPtr lParam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int WS_EX_APPWINDOW = 0x40000;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_EX_WINDOWEDGE = 0x100;

        private bool Report(int hwnd, int lParam)
        {
            int lExStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
            int lStyle = GetWindowLongPtr(hwnd, GWL_STYLE);

            if ((lExStyle & WS_EX_WINDOWEDGE) == WS_EX_WINDOWEDGE && (lStyle & WS_VISIBLE) == WS_VISIBLE && hwnd != MonitorActivate.desktopHandle.ToInt32()
                && hwnd != MonitorActivate.tray.ToInt32() && hwnd != MonitorActivate.thisAppHandle.ToInt32())
            {
                OSWindow oswindow = new OSWindow();
                oswindow.Hwnd = hwnd;
                oswindow.Title = MonitorActivate.GetWindowText(new IntPtr(hwnd));
                oswindow.Path = MonitorActivate.GetProcessPath(new IntPtr(hwnd));

                _oswindows.Add(oswindow);
            }
            
            return true;
        }

        public void Refresh()
        {
            _oswindows.Clear();

            EnumDesktopWindows(IntPtr.Zero, callBackPtr, IntPtr.Zero);
            //EnumWindows(callBackPtr, 0);
        }

        private ObservableCollection<OSWindow> _oswindows = new ObservableCollection<OSWindow>();

        public ObservableCollection<OSWindow> OSWindows
        {
            get
            {
                return _oswindows;
            }
        }

        public string GetURL(int hwnd)
        {
            ShellWindows sw = new ShellWindows();
            
            IEnumerator windows = new ShellWindowsClass().GetEnumerator();
            while (windows.MoveNext())
            {
                if ((windows.Current as IWebBrowser2).HWND == hwnd)
                {
                    if ((windows.Current is IWebBrowser2))
                    {
                        IntPtr hw; 
                        IOleWindow win = ((windows.Current as IWebBrowser2).Document as IOleWindow);
                        if (win != null)
                        {
                            win.GetWindow(out hw);

                            if (IsWindowVisible(hw))
                            {
                                string activeTabUrl = ((windows.Current as IWebBrowser2).Document as mshtml.IHTMLDocument2).url;
                                return activeTabUrl;
                            }
                        }
                        else
                        {
                            return (windows.Current as IWebBrowser2).LocationURL;
                        }
                    }
                }
            }

            return "n/a";
        }
    }

    [ComImport]
    [Guid("00000114-0000-0000-C000-000000000046")]
    [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleWindow
    {
        /// <summary>
        /// Returns the window handle to one of the windows participating in in-place activation 
        /// (frame, document, parent, or in-place object window).
        /// </summary>
        /// <param name="phwnd">Pointer to where to return the window handle.</param>
        void GetWindow (out IntPtr phwnd) ;

        /// <summary>
        /// Determines whether context-sensitive help mode should be entered during an 
        /// in-place activation session.
        /// </summary>
        /// <param name="fEnterMode"><c>true</c> if help mode should be entered; 
        /// <c>false</c> if it should be exited.</param>
        void ContextSensitiveHelp ([In, MarshalAs(UnmanagedType.Bool)] bool fEnterMode) ;
    }

    public class OSWindow
    {
        public int Hwnd
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public override string ToString()
        {
            if (Title == null || Title == string.Empty)
                return Path;
            else
                return Title;
        }
    }
}
