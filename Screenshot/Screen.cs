using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Threading;
//using AviFile;
using System.IO;

namespace ProcessCapture.Screenshot
{
    public class ScreenCapture
    {
        public ScreenCapture()
        {
            Recording = false;
        }

        public ScreenImage TakeScreenshot()
        {
            int x;
            int y;

            GetDimensions(out x, out y);

            Bitmap result = new Bitmap(x, y);
            {
                using (Graphics graphics = Graphics.FromImage(result))
                {
                    graphics.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(x, y));
                }
            }
            return null;// new ScreenImage(result);
        }

        public void GetDimensions(out int X, out int Y)
        {
            X = 0;
            Y = 0;

            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                int x = screen.Bounds.X + screen.Bounds.Width;
                if (x > X)
                {
                    X = x;
                }

                int y = screen.Bounds.Y + screen.Bounds.Height;
                if (y > Y)
                {
                    Y = y;
                }
            }
        }

        public void SaveImage(Bitmap image, string path, string name)
        {
            image.Save(path + @"\" + name);
        }

        public void SaveImageAsJpg(Bitmap image, string path, string name)
        {
            image.Save(path + @"\" + name, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        public bool Recording { get; set; }

        public void Record(string path)
        {
            if (Recording)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(delegate
            {
                List<string> files = new List<string>();

                int i = 1;
                
                Recording = true;

                while (Recording)
                {
                    Bitmap b = new Bitmap(1,1);// TakeScreenshot();

                    string filename = string.Format(@"IMG-{0}.jpg", i);
                    string fullPath = string.Format(@"{0}\{1}", path, filename);

                    files.Add(fullPath);

                    SaveToDiskAsync(b, path, filename);
                   
                    i++;

                    Thread.Sleep(100);
                }

                //ConvertToVideo(files, path);
            });
        }

        private void SaveToDiskAsync(Bitmap b, string path, string filename)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                SaveImageAsJpg(b, path, filename);
                b.Dispose();
            });
        }
        /*
        public void ConvertToVideo(List<string> files, string outputDir)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(files[0]);
            //create a new AVI file
            AviManager aviManager = new AviManager(outputDir + @"\recording.avi", false);
            //add a new video stream and one frame to the new file

            VideoStream aviStream = aviManager.AddVideoStream(true, 7, bitmap);
            
            foreach(string file in files)
            {
                if (File.Exists(file))
                {
                    bitmap = (Bitmap)Bitmap.FromFile(file);
                    aviStream.AddFrame(bitmap);
                    bitmap.Dispose();

                    File.Delete(file);
                }
            }
            //aviStream.Close();

            aviManager.Close();
        }
         */
    }
}
