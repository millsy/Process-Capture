using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using OpenSpanWPF;
using System.IO;

namespace ProcessCapture
{
    public class Commands
    {
        public static readonly RoutedUICommand DeleteRecord = new RoutedUICommand("Delete image", "DeleteImage", typeof(OpenSpanWPF.OpenSpanWPFWindow));

        public static readonly RoutedUICommand ShowImage = new RoutedUICommand("Show image", "ShowImage", typeof(OpenSpanWPF.OpenSpanWPFWindow));

        public static readonly RoutedUICommand HideImage = new RoutedUICommand("Hide image", "HideImage", typeof(OpenSpanWPF.OpenSpanWPFWindow));

        public static readonly RoutedUICommand SaveImage = new RoutedUICommand("Save image", "SaveImage", typeof(OpenSpanWPF.OpenSpanWPFWindow));

        public static readonly RoutedUICommand Undo = new RoutedUICommand("Undo", "Undo", typeof(OpenSpanWPF.OpenSpanWPFWindow));

        public static string GetFileName()
        {
            return string.Format(@"{0}Screen-{1}.png", OutputDir(), GetFileNumber());
        }

        public static string GetFileNumber()
        {
            Guid g = Guid.NewGuid();
            return g.ToString();
        }

        public static string OutputDir()
        {
            OpenSpanWPFWindow window = (OpenSpanWPFWindow)Application.Current.MainWindow;
            Project proj = window.processInfo.ProjectObject;
            if (proj != null)
            {
                string projFile = proj.ProjectFileLocation;
                if (projFile != null && projFile != string.Empty)
                {
                    string dirName = Path.GetDirectoryName(projFile) + @"\" + ImagesFolderName + @"\";
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
                    return dirName;
                }
            }

            return System.IO.Path.GetTempPath();
        }

        private static string ImagesFolderName = "images";
    }
}
