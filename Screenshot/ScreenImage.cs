using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;

namespace ProcessCapture.Screenshot
{
    public class ScreenImage : DependencyObject
    {
        ~ScreenImage()
        {

        }

        private string _filename;
       
        private string _notes;
        private string _title;
        private string _path;
        private string[] _modules = {};
        private string _appURL;

        private bool _isProcessDiagram = false;

        public bool IsProcessDiagram
        {
            get
            {
                return _isProcessDiagram;
            }
            set
            {
                _isProcessDiagram = value;
            }
        }

        public string ApplicationURL
        {
            get
            {
                return _appURL;
            }
            set
            {
                _appURL = value;
            }
        }

        public ScreenImage(string image)
        {
            if (image != null && File.Exists(image))
            {
                Bitmap = new Bitmap(image);
            }

            _filename = image;
            _notes = "";
        }

        public ScreenImage(ScreenImage si, string image)
        {
            if (image != null && File.Exists(image))
            {
                Bitmap = new Bitmap(image);
            }

            _filename = image;
            _notes = si.Notes;
            //_appURL = si.ApplicationURL;
            //_modules = si.Modules;
            //_path = si.Path;
            _title = si.Title;
            //_isProcessDiagram = si.IsProcessDiagram;
        }

        public string Filename
        {
            get
            {
                return _filename;
            }
        }

        public BitmapSource BitmapSource { 
            get
            {
                return (BitmapSource)GetValue(BitmapSourceProperty);
            }

            private set
            {
                SetValue(BitmapSourceProperty, value);
            }
        }

        public Bitmap Bitmap
        {
            get
            {
                return (Bitmap)GetValue(BitmapProperty);
            }
            private set
            {
                SetValue(BitmapProperty, value);

                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                            ((Bitmap)Bitmap).GetHbitmap(),
                                            IntPtr.Zero,
                                            Int32Rect.Empty,
                                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                BitmapSource = bitmapSource;
            }
        }

        public static readonly DependencyProperty BitmapProperty = DependencyProperty.Register("Bitmap", typeof(Bitmap), typeof(ScreenImage));
        public static readonly DependencyProperty BitmapSourceProperty = DependencyProperty.Register("BitmapSource", typeof(BitmapSource), typeof(ScreenImage));

        public void Save(Canvas workspace)
        {
            if(Bitmap != null)
                Bitmap.Dispose();

            FileStream fs = new FileStream(_filename, FileMode.Create);

            int width = (int)workspace.ActualWidth;
            if((int)workspace.Width > width)
                width = (int) workspace.Width;

            int height = (int)workspace.ActualHeight;
            if ((int)workspace.Height > height)
                height = (int)workspace.Height;
            
            RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

            //this is a hack to prevent only the visable items being rendered when using the scrollviewer
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(workspace);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), new System.Windows.Size(width, height)));
            }
            bmp.Render(dv);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(fs);
            fs.Close();

            Bitmap = new Bitmap(_filename);
        }

        public string Title { get { return _title; } set { _title = value; } }
        public string Path { get { return _path; } set { _path = value; } }
        public string[] Modules { get { return _modules; } set { _modules = value; } }
        public string Notes { get { return _notes; } set { _notes = value; } }
    }
}
