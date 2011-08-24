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
using System.Windows.Shapes;
using System.Xml;
using ProcessCapture.Screenshot;
using System.Drawing;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using ProcessCapture.PDF;
using ProcessCapture.Hotkey;
using ProcessCapture.MonitorActivate;
using System.Windows.Interop;
using ProcessCapture;

namespace OpenSpanWPF
{
    /// <summary>
    /// Interaction logic for ActivitiesWindow.xaml
    /// </summary>
    public partial class OpenSpanWPFWindow : Window
    {        
        public OpenSpanWPFWindow() : base()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler((sender, args) => Close())));

            this.btnMinimize.Click += new RoutedEventHandler(Minimize);
            this.btnMaximize.Click += new RoutedEventHandler(Maximize);

            this.Closing += new System.ComponentModel.CancelEventHandler(OpenSpanWPFWindow_Closing);

            //Hotkey hk = new Hotkey();
            //hk.KeyCode = System.Windows.Forms.Keys.T;
            //hk.Control = true;
            //hk.Pressed += new System.ComponentModel.HandledEventHandler(hk_Pressed);
            //if (!hk.Register(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle))
            //{
            //    MessageBox.Show("Failed to register hotkey");
            //}    

            if (SystemParameters.WorkArea.Height < this.Height)
            {
                this.Height = SystemParameters.WorkArea.Height;
            }
            if (SystemParameters.WorkArea.Width < this.Width)
            {
                this.Width = SystemParameters.WorkArea.Width;
            }
        }

        void hk_Pressed(object sender, System.ComponentModel.HandledEventArgs e)
        {
            screenCapture.CaptureWindow();
        }

        void OpenSpanWPFWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.processInfo.ProjectObject.SaveRequired)
            {
                if (MessageBox.Show("Do you wish to save your project?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.processInfo.Save();
                }
            }

            screenCapture.Close();
            outputInfo.DisposeChildren();
            e.Cancel = false;
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }

        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            myWindow.DragMove();
        }

        

        
    }

    
}
