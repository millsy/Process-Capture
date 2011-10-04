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
using OpenSpanWPF;
using ProcessCapture.UserControls;

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
        private List<UIElement> shapes = new List<UIElement>();

        #region MouseMovement
        private void drawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                pointEnd = new Point(e.GetPosition(drawingCanvas).X, e.GetPosition(drawingCanvas).Y);

                if (btnRect.IsChecked == true)
                {
                    DrawRect(pointEnd);
                }
                else if (btnLine.IsChecked == true || btnArrow.IsChecked == true)
                {
                    DrawLine(pointEnd);
                }
                else if (btnText.IsChecked == true)
                {
                    mouseMoveText.SetValue(Canvas.LeftProperty, pointEnd.X);
                    mouseMoveText.SetValue(Canvas.TopProperty, pointEnd.Y);
                }
                else if (btnPointer.IsChecked == true && moveableObject != null)
                {
                    if (moveableObject.GetType() == typeof(TextBlock))
                    {
                        (moveableObject as TextBlock).SetValue(Canvas.LeftProperty, pointEnd.X);
                        (moveableObject as TextBlock).SetValue(Canvas.TopProperty, pointEnd.Y);
                    }
                }
            }
        }

        private void drawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;

            pointEnd = new Point(e.GetPosition(drawingCanvas).X, e.GetPosition(drawingCanvas).Y);

            if (btnText.IsChecked == true)
            {
                mouseMoveText.SetValue(Canvas.LeftProperty, pointEnd.X);
                mouseMoveText.SetValue(Canvas.TopProperty, pointEnd.Y);
                //mouseMoveText.LostFocus += new RoutedEventHandler(mouseMoveText_LostFocus);
                mouseMoveText.IsEditable = true;
                mouseMoveText.IsInEditMode = true;
                mouseMoveText.Focus();
                shapes.Add(mouseMoveText);
            }
            else if (btnRect.IsChecked == true)
            {
                mouseMoveRec.StrokeDashArray = null;
                DrawRect(pointEnd);
                shapes.Add(mouseMoveRec);
            }
            else if(btnArrow.IsChecked == true || btnLine.IsChecked == true)
            {
                mouseMoveLine.StrokeDashArray = null;
                DrawLine(pointEnd);
                shapes.Add(mouseMoveLine);
            }
            else if (btnPointer.IsChecked == true && moveableObject != null)
            {
                if (moveableObject.GetType() == typeof(EditableTextBlock))
                {
                    (moveableObject as EditableTextBlock).SetValue(Canvas.LeftProperty, pointEnd.X);
                    (moveableObject as EditableTextBlock).SetValue(Canvas.TopProperty, pointEnd.Y);
                }
                moveableObject = null;
            }
        }

        void mouseMoveText_LostFocus(object sender, RoutedEventArgs e)
        {            
            (sender as EditableTextBlock).IsEditable = false;
            (sender as EditableTextBlock).IsInEditMode = false;
        }

        private void drawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            
            pointStart = new Point(e.GetPosition(drawingCanvas).X, e.GetPosition(drawingCanvas).Y);

            if (btnRect.IsChecked == true)
            {
                mouseMoveRec = new Rectangle();
                mouseMoveRec.Stroke = (SolidColorBrush)lineColour.SelectedItem;
                mouseMoveRec.StrokeThickness = Int32.Parse(lineWidth.Text);
                mouseMoveRec.Fill = (SolidColorBrush)fillColour.SelectedItem;
                mouseMoveRec.StrokeDashArray = new DoubleCollection(new double[] { 1 });
                drawingCanvas.Children.Add(mouseMoveRec);
            }
            else if (btnText.IsChecked == true)
            {
                mouseMoveText = new EditableTextBlock();
                mouseMoveText.IsEditable = false;
                mouseMoveText.Text = "Sample Text";
                mouseMoveText.Foreground = (SolidColorBrush)lineColour.SelectedItem;
                drawingCanvas.Children.Add(mouseMoveText);
            }
            else if (btnArrow.IsChecked == true || btnLine.IsChecked == true)
            {
                mouseMoveLine = new Arrow();
                mouseMoveLine.Stroke = (SolidColorBrush)lineColour.SelectedItem;
                mouseMoveLine.StrokeThickness = Int32.Parse(lineWidth.Text);
                mouseMoveLine.StrokeDashArray = new DoubleCollection(new double[] { 1 });

                if (btnArrow.IsChecked == true)
                {
                    mouseMoveLine.HeadWidth = Int32.Parse(lineWidth.Text) * 3;
                    mouseMoveLine.HeadHeight = Int32.Parse(lineWidth.Text) * 3;
                }

                drawingCanvas.Children.Add(mouseMoveLine);
            }
            else if (btnPointer.IsChecked == true)
            {
                //moveableObject = drawingCanvas.Children[drawingCanvas.Children.Count - 1];// e.OriginalSource;
            }
        }

        private void DrawLine(Point pointEnd)
        {
            mouseMoveLine.X1 = pointStart.X;
            mouseMoveLine.Y1 = pointStart.Y;
            mouseMoveLine.X2 = pointEnd.X;
            mouseMoveLine.Y2 = pointEnd.Y;
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

        object moveableObject;
        Point pointStart;
        Point pointEnd;
        bool IsMouseDown;

        EditableTextBlock mouseMoveText = new EditableTextBlock();
        Rectangle mouseMoveRec = new Rectangle();
        Arrow mouseMoveLine = new Arrow();
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

            (Application.Current.MainWindow as OpenSpanWPFWindow).processInfo.ProjectObject.RequestSave();
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
                UIElement s = shapes[shapes.Count - 1];
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
            UIElement s = shapes[shapes.Count - 1];
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
