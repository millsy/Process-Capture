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
using ProcessCapture.Screenshot;
using ProcessCapture.UserControls;
using System.Windows.Controls.Primitives;

namespace ProcessCapture
{
    /// <summary>
    /// Interaction logic for ProcessLayoutWindow.xaml
    /// </summary>
    public partial class ProcessLayoutWindow : Window
    {
        private List<ScreenImage> _images;

        public List<ScreenImage> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
            }
        }

        private ProcessCapture.TabControls.ScreenCaptureTab Parent;

        public ProcessLayoutWindow(ProcessCapture.TabControls.ScreenCaptureTab parent)
        {
            InitializeComponent();

            Parent = parent;

            this.btnClose.Click += new RoutedEventHandler(btnClose_Click);
            this.btnMinimize.Click += new RoutedEventHandler(Minimize);
            this.btnMaximize.Click += new RoutedEventHandler(Maximize);  
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        #region Setup

        private void AddImages()
        {
            screenShots.Items.Clear();

            foreach (ScreenImage si in Images)
            {
                if (!si.IsProcessDiagram)
                {
                    ImageLayout il = new ImageLayout();
                    il.Setup(si.Filename, si.Title);

                    screenShots.Items.Add(il);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region WindowControl

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
            this.DragMove();
        }

        #endregion

      
        private void window_Activated(object sender, EventArgs e)
        {
            AddImages();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (si == null)
            {
                si = new ScreenImage(Commands.GetFileName());
                si.IsProcessDiagram = true;
                si.Title = "Process Diagram";
                Parent.ScreenImages.Add(si);
            }

            si.Save(drawingCanvas);            
        }

        ScreenImage si = null;
    }
}
