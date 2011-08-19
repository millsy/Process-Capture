using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ProcessCapture.Log
{
    public class Logger
    {
        private Logger()
        {

        }

        public static Logger GetInstance()
        {
            if (instance == null)
            {
                instance = new Logger();
            }

            return instance;
        }

        private static Logger instance;

        public string Log(Exception e)
        {
            if (e == null)
            {
                return "N/A";
            }
            string tempPath = System.IO.Path.GetTempPath();
            Guid id = Guid.NewGuid();
            string file = tempPath + id.ToString() + ".log";
            System.IO.File.WriteAllText(file, e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace + Environment.NewLine + Environment.NewLine + (e.InnerException != null ? e.InnerException.StackTrace : "No inner exception"));

            Process.Start("explorer.exe", "/select," + file);

            return file;
        }
    }
}
