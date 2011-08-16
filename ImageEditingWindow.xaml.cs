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

namespace ProcessCapture
{
    /// <summary>
    /// Interaction logic for ImageEditingWindow.xaml
    /// </summary>
    public partial class ImageEditingWindow : Window
    {
        public ImageEditingWindow(ScreenImage si)
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler((sender, args) => Close())));

            this.btnMinimize.Click += new RoutedEventHandler(Minimize);
            this.btnMaximize.Click += new RoutedEventHandler(Maximize);

            ScreenImage = si;

            fullSizeImage.Stretch = Stretch.None;
            fullSizeImage.ImageSource = si.BitmapSource;

            drawingCanvas.Width = si.Bitmap.Width;
            drawingCanvas.Height = si.Bitmap.Height;
        }

        public ScreenImage ScreenImage
        {
            get
            {
                return _screenImage;
            }

            private set
            {
                _screenImage = value;
            }
        }

        private ScreenImage _screenImage;
        private List<Shape> shapes = new List<Shape>();

        #region MouseMovement
        private void drawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                pointEnd = new Point(e.GetPosition(drawingCanvas).X, e.GetPosition(drawingCanvas).Y);

                DrawRect(pointEnd);
            }
        }

        private void drawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;

            pointEnd = new Point(e.GetPosition(drawingCanvas).X, e.GetPosition(drawingCanvas).Y);

            mouseMoveRec.StrokeDashArray = null;

            DrawRect(pointEnd);

            shapes.Add(mouseMoveRec);
        }

        private void drawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;

            pointStart = new Point(e.GetPosition(drawingCanvas).X, e.GetPosition(drawingCanvas).Y);

            mouseMoveRec = new Rectangle();

            mouseMoveRec.Stroke = (SolidColorBrush)lineColour.SelectedItem;
            mouseMoveRec.StrokeThickness = Int32.Parse(lineWidth.Text);
            mouseMoveRec.Fill = (SolidColorBrush)fillColour.SelectedItem;
            mouseMoveRec.StrokeDashArray = new DoubleCollection(new double[] {1});
            drawingCanvas.Children.Add(mouseMoveRec);

        }

        private void DrawRect(Point pointEnd)
        {
            Point pointTopLeft = new Point();

            // Now we have two points
            double _width = pointEnd.X - pointStart.X;
            double _height = pointEnd.Y - pointStart.Y;

            if (_width > 0 & _height > 0)
            {
                pointTopLeft.X = pointStart.X;
                pointTopLeft.Y = pointStart.Y;
            }
            else if (_width < 0 & _height > 0)
            {
                pointTopLeft.X = pointStart.X + _width;
                pointTopLeft.Y = pointStart.Y;
            }
            else if (_width > 0 & _height < 0)
            {
                pointTopLeft.X = pointStart.X;
                pointTopLeft.Y = pointStart.Y + _height;
            }
            else
            {
                pointTopLeft.X = pointStart.X + _width;
                pointTopLeft.Y = pointStart.Y + _height;
            }

            // Start Point should be Left and Top
            mouseMoveRec.SetValue(Canvas.LeftProperty, pointTopLeft.X);
            mouseMoveRec.SetValue(Canvas.TopProperty, pointTopLeft.Y);

            mouseMoveRec.Width = Math.Abs(_width);
            mouseMoveRec.Height = Math.Abs(_height);
        }
        
        Point pointStart;
        Point pointEnd;
        bool IsMouseDown;

        Rectangle mouseMoveRec = new Rectangle();
        #endregion

        #region Commands
        private void saveImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (shapes.Count > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void saveImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScreenImage.Save(drawingCanvas);
        }

        private void hideImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void hideImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        private void undoImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (drawingCanvas != null && shapes.Count > 0)
            {
                Shape s = shapes[shapes.Count - 1];
                if (drawingCanvas.Children.Contains(s))
                {
                    e.CanExecute = true;
                    return;
                }
            }
            e.CanExecute = false;
        }

        private void undoImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Shape s = shapes[shapes.Count - 1];
            if (drawingCanvas.Children.Contains(s))
            {
                drawingCanvas.Children.Remove(s);
                shapes.Remove(s);
            }
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

        
    }
}
