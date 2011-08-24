using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using ProcessCapture.Screenshot;
using ProcessCapture.Log;
using System.IO;

namespace ProcessCapture
{
    public class Project : DependencyObject
    {

        private static DependencyProperty ProcessNameDO = DependencyProperty.Register("ProcessName", typeof(string), typeof(Project));
        private static DependencyProperty ProcessDescDO = DependencyProperty.Register("Description", typeof(string), typeof(Project));
        private static DependencyProperty ProcessAuthorDO = DependencyProperty.Register("Author", typeof(string), typeof(Project));
        private static DependencyProperty ProcessImageDO = DependencyProperty.Register("ProcessImage", typeof(string), typeof(Project));

        public string ProjectFileLocation;

        public bool SaveRequired = false;

        public void RequestSave()
        {
            SaveRequired = true;
        }

        public string ProcessName
        {
            get
            {
                return (string) GetValue(ProcessNameDO);
            }
            set
            {
                SetValue(ProcessNameDO, value);
            }
        }

        public string Description
        {
            get
            {
                return (string)GetValue(ProcessDescDO);
            }
            set
            {
                SetValue(ProcessDescDO, value);
            }
        }

        public string Author
        {
            get
            {
                return (string)GetValue(ProcessAuthorDO);
            }
            set
            {
                SetValue(ProcessAuthorDO, value);
            }
        }

        public string ProcessImage
        {
            get
            {
                return (string)GetValue(ProcessImageDO);
            }
            set
            {
                SetValue(ProcessImageDO, value);
            }
        }

        public string ToXML(List<ScreenImage> images)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<project>" + Environment.NewLine);

            sb.Append("<name>");
            sb.Append(ProcessName);
            sb.Append("</name>" + Environment.NewLine);

            sb.Append("<description>");
            sb.Append(Description);
            sb.Append("</description>" + Environment.NewLine);

            sb.Append("<author>");
            sb.Append(Author);
            sb.Append("</author>" + Environment.NewLine);
            
            sb.Append("<image>");
            sb.Append(ProcessImage);
            sb.Append("</image>" + Environment.NewLine);

            sb.Append("<screenshots>" + Environment.NewLine);
            foreach (ScreenImage si in images)
            {
                sb.Append("<screenshot>" + Environment.NewLine);

                sb.Append("<location><![CDATA[");
                sb.Append("images\\" + Path.GetFileName(si.Filename));
                sb.Append("]]></location>" + Environment.NewLine);

                sb.Append("<url><![CDATA[");
                //sb.Append(Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(si.ApplicationURL)));
                sb.Append(si.ApplicationURL);
                sb.Append("]]></url>" + Environment.NewLine);

                sb.Append("<title><![CDATA[");
                sb.Append(si.Title);
                sb.Append("]]></title>" + Environment.NewLine);

                sb.Append("<path><![CDATA[");
                sb.Append(si.Path);
                sb.Append("]]></path>" + Environment.NewLine);

                sb.Append("<notes><![CDATA[");
                sb.Append(si.Notes);
                sb.Append("]]></notes>" + Environment.NewLine);

                sb.Append("<modules>" + Environment.NewLine);
                foreach (string m in si.Modules)
                {
                    sb.Append("<module><![CDATA[");
                    sb.Append(m);
                    sb.Append("]]></module>" + Environment.NewLine);
                }
                sb.Append("</modules>" + Environment.NewLine);

                sb.Append("</screenshot>" + Environment.NewLine);
            }
            sb.Append("</screenshots>" + Environment.NewLine);

            sb.Append("</project>");

            return sb.ToString();
        }

        public static Project FromXML(string filename, string xml, out List<ScreenImage> images)
        {
            images = new List<ScreenImage>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                string baseDir = Path.GetDirectoryName(filename);

                Project newProject = new Project();

                newProject.ProcessName = xmlDoc.SelectSingleNode("/project/name/text()") != null ? xmlDoc.SelectSingleNode("/project/name/text()").InnerText : "";
                newProject.Description = xmlDoc.SelectSingleNode("/project/description/text()") != null ? xmlDoc.SelectSingleNode("/project/description/text()").InnerText : "";
                newProject.Author = xmlDoc.SelectSingleNode("/project/author/text()") != null ? xmlDoc.SelectSingleNode("/project/author/text()").InnerText : "";
                newProject.ProcessImage = xmlDoc.SelectSingleNode("/project/image/text()") != null ? xmlDoc.SelectSingleNode("/project/image/text()").InnerText : "";

                XmlNodeList screenshots = xmlDoc.SelectNodes("/project/screenshots/screenshot");
                foreach (XmlNode screenshot in screenshots)
                {
                    string path = screenshot.SelectSingleNode("location").InnerText;

                    if (!Path.IsPathRooted(path))
                    {
                        //relative path in XML
                        //append Images directory
                        path = baseDir + "\\" + path;
                    }

                    ScreenImage si = new ScreenImage(path);

                    si.ApplicationURL = screenshot.SelectSingleNode("url") != null ? screenshot.SelectSingleNode("url").InnerText : "";
                    si.Title = screenshot.SelectSingleNode("title") != null ? screenshot.SelectSingleNode("title").InnerText : "";
                    si.Path = screenshot.SelectSingleNode("path") != null ? screenshot.SelectSingleNode("path").InnerText : "";
                    si.Notes = screenshot.SelectSingleNode("notes") != null ? screenshot.SelectSingleNode("notes").InnerText : "";

                    XmlNodeList modules = screenshot.SelectNodes("modules/module");
                    List<string> moduleList = new List<string>();

                    foreach (XmlNode module in modules)
                    {
                        moduleList.Add(module.InnerText);
                    }

                    si.Modules = moduleList.ToArray();

                    images.Add(si);
                }

                return newProject;
            }
            catch (Exception e)
            {
                string file = Logger.GetInstance().Log(e);
                MessageBox.Show("An error has occurred - it has been saved to " + file + " - please email this to millsy@openspan.com");

                return null;
            }
        }
    }
}
