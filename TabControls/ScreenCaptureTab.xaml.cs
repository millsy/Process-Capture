using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProcessCapture.Screenshot;
using ProcessCapture.PDF;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing;
using ProcessCapture.MonitorActivate;
using System.IO;
using OpenSpanWPF;

namespace ProcessCapture.TabControls
{
    /// <summary>
    /// Interaction logic for ScreenCaptureTab.xaml
    /// </summary>
    public partial class ScreenCaptureTab : TabItem
    {
        private int TIMEOUT = 500;

        public ScreenCaptureTab()
        {
            InitializeComponent();

            monActivate = new ProcessCapture.MonitorActivate.MonitorActivate(new CallbackHandler(CaptureScreen));

            plw = new ProcessLayoutWindow(this);

            _screenImages.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_screenImages_CollectionChanged);
        }

        private void _screenImages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            (Application.Current.MainWindow as OpenSpanWPFWindow).processInfo.ProjectObject.RequestSave();
            screenShots.Items.Refresh();
        }

        private ProcessLayoutWindow plw;
        ProcessCapture.MonitorActivate.WindowMgr wm = new ProcessCapture.MonitorActivate.WindowMgr();

        public void Close()
        {
            if (plw != null)
            {
                plw.Close();
            }
        }

        public ObservableCollection<ScreenImage> _screenImages = new ObservableCollection<ScreenImage>();
        public ObservableCollection<ScreenImage> ScreenImages
        {
            get
            {
                return _screenImages;
            }
        }

        public ObservableCollection<OSWindow> OSWindows
        {
            get
            {
                return wm.OSWindows;
            }
        }

        private ProcessCapture.Screenshot.ScreenCapture sc = new ProcessCapture.Screenshot.ScreenCapture();
        private MonitorActivate.MonitorActivate monActivate = null;

        private void btnTakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            CaptureDesktop();
        }

        private delegate void CallbackHandler(IntPtr hwnd);

        private void btnAutomaticCapture_Click(object sender, RoutedEventArgs e)
        {
            if (monActivate == null)
            {
                monActivate = new ProcessCapture.MonitorActivate.MonitorActivate(new CallbackHandler(CaptureScreen));
            }

            if (!monActivate.IsRunning())
            {
                //Application.Current.MainWindow.WindowState = WindowState.Minimized;

                monActivate.Start();
            }
            else
            {
                monActivate.Stop();
            }

            UpdateButton();
        }

        private void CaptureScreen(IntPtr hwnd)
        {
            Thread.Sleep(TIMEOUT);

            CaptureWindow(hwnd);
        }

        public void CaptureWindow()
        {
            string filename = Commands.GetFileName();
            screenCapture.CaptureWindowToFile(filename, ImageFormat.Png);

            MonitorActivate.MonitorActivate.SetWindowFocus();

            ScreenImage si = new ScreenImage(filename);
            si.Title = MonitorActivate.MonitorActivate.GetWindowText();
            si.Path = MonitorActivate.MonitorActivate.GetProcessPath();
            si.Modules = MonitorActivate.MonitorActivate.GetProcessModules();
            si.ApplicationURL = wm.GetURL(ProcessCapture.MonitorActivate.ScreenCapture.User32.GetForegroundWindow().ToInt32());
            
            _screenImages.Add(si);
        }

        public void CaptureWindow(IntPtr hwnd)
        {
            string filename = Commands.GetFileName();
            screenCapture.CaptureWindowToFile(hwnd, filename, ImageFormat.Png);

            //Thread.Sleep(TIMEOUT);

            MonitorActivate.MonitorActivate.SetWindowFocus();

            if (screenShots.SelectedIndex > -1 && screenShots.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Replace selected image?", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    ScreenImage osi = screenShots.SelectedItem as ScreenImage;
                    int oldIndex = _screenImages.IndexOf(osi);
                    ScreenImage newImage = new ScreenImage(osi, filename);
                    DeleteImage(osi);
                    newImage.Path = MonitorActivate.MonitorActivate.GetProcessPath(hwnd);
                    newImage.Modules = MonitorActivate.MonitorActivate.GetProcessModules(hwnd);
                    newImage.ApplicationURL = wm.GetURL(hwnd.ToInt32());
                    _screenImages.Insert(oldIndex, newImage);
                    return;
                }
            }
            ScreenImage si = new ScreenImage(filename);
            si.Title = MonitorActivate.MonitorActivate.GetWindowText(hwnd);
            si.Path = MonitorActivate.MonitorActivate.GetProcessPath(hwnd);
            si.Modules = MonitorActivate.MonitorActivate.GetProcessModules(hwnd);
            si.ApplicationURL = wm.GetURL(hwnd.ToInt32());
            
            _screenImages.Add(si);        
        }

        public void CaptureDesktop()
        {
            Application.Current.MainWindow.Hide();

            Thread.Sleep(TIMEOUT);

            string filename = Commands.GetFileName();
            screenCapture.CaptureScreenToFile(filename, ImageFormat.Png);

            Application.Current.MainWindow.Show();
            
            MonitorActivate.MonitorActivate.SetWindowFocus();

            if (screenShots.SelectedIndex > -1 && screenShots.SelectedItem != null)
            {
                //MessageBoxResult result = ;
                if (MessageBox.Show("Replace selected image?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    ScreenImage si = screenShots.SelectedItem as ScreenImage;
                    int oldIndex = _screenImages.IndexOf(si);
                    ScreenImage newImage = new ScreenImage(si, filename);
                    DeleteImage(si);
                    _screenImages.Insert(oldIndex, newImage);
                }
                else
                {
                    AddScreenShot(filename, "Desktop Screenshot");
                }
            }
            else
            {
                AddScreenShot(filename, "Desktop Screenshot");
            }

            Application.Current.MainWindow.Show();
        }

        private void AddScreenShot(string filename, string title)
        {
            ScreenImage si = new ScreenImage(filename);
            si.Title = title;
            si.Path = "n/a";
            si.ApplicationURL = "n/a";
            _screenImages.Add(si);
        }

        ProcessCapture.MonitorActivate.ScreenCapture screenCapture = new ProcessCapture.MonitorActivate.ScreenCapture();

        private void DeleteRecord_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void DeleteRecord_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScreenImage sc = e.Parameter as ScreenImage;

            DeleteImage(sc);

            e.Handled = true;
        }

        private void DeleteImage(ScreenImage sc)
        {
            _screenImages.Remove(sc);
            if (sc != null && sc.Bitmap != null)
            {
                sc.Bitmap.Dispose();
            }

            if (File.Exists(sc.Filename))
            {
                File.Delete(sc.Filename);
            }
        }

        private void ShowImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ImageEditingWindow iew = new ImageEditingWindow((ScreenImage)e.Parameter);            
            iew.ShowDialog();
        }


        private void btnCaptureWindow_Click(object sender, RoutedEventArgs e)
        {
            wm.Refresh();
            
            SelectWindowPopup.IsOpen = true;
        }

        public void UpdateButton()
        {            
            if (monActivate.IsRunning())
            {
                btnAutomaticCaptureLabel.Text = "Stop";
            }
            else
            {
                btnAutomaticCaptureLabel.Text = "Start";
            }
        }

        private void SelectWindowPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            SelectWindowPopup.IsOpen = false;
        }

        private void windows_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectWindowPopup.IsOpen = false;

            OSWindow oswin = windows.SelectedItem as OSWindow;
            IntPtr win = new IntPtr(oswin.Hwnd);

            MonitorActivate.MonitorActivate.SetWindowFocus(win);

            Thread.Sleep(TIMEOUT);

            CaptureWindow(win);

            //Application.Current.MainWindow.Focus();
        }

        private void btnProcessLayout_Click(object sender, RoutedEventArgs e)
        {
            plw.Images = ScreenImages.ToList<ScreenImage>();
            plw.ShowDialog();
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            ScreenImage img = ((ListBoxItem)screenShots.ContainerFromElement((Button)sender)).Content as ScreenImage;
            int currentIndex = _screenImages.IndexOf(img);
            int newIndex = currentIndex + 1;
            _screenImages.Move(currentIndex, newIndex);
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            ScreenImage img = ((ListBoxItem)screenShots.ContainerFromElement((Button)sender)).Content as ScreenImage;
            int currentIndex = _screenImages.IndexOf(img);
            int newIndex = currentIndex - 1;
            _screenImages.Move(currentIndex, newIndex);
        }

        private void txtTitle_KeyDown(object sender, KeyEventArgs e)
        {
            (Application.Current.MainWindow as OpenSpanWPFWindow).processInfo.ProjectObject.RequestSave();
        }
    }
}
