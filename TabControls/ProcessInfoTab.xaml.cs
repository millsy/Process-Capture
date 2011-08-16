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
using System.IO;
using OpenSpanWPF;
using ProcessCapture.Screenshot;

namespace ProcessCapture.TabControls
{
    /// <summary>
    /// Interaction logic for ProcessInfoTab.xaml
    /// </summary>
    public partial class ProcessInfoTab : TabItem
    {
        public ProcessInfoTab()
        {
            InitializeComponent();

            ProjectObject = new Project();
        }

        private static DependencyProperty ProjectDO = DependencyProperty.Register("ProjectObject", typeof(Project), typeof(ProcessInfoTab));

        public Project ProjectObject
        {
            get
            {
                return (Project)GetValue(ProjectDO);
            }

            set
            {
                SetValue(ProjectDO, value);
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";

            bool? result = dialog.ShowDialog(Application.Current.MainWindow);

            if (result.HasValue && result.Value)
            {
                fileLocation.Text = dialog.FileName;
                ProjectObject.ProcessImage = dialog.FileName;
                processDiagram.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            processDiagram.Source = null;
            ProjectObject.ProcessImage = null;
            fileLocation.Text = "";
        }

        private void btnSaveProject_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "XML File|*.xml";
            dialog.FileName = "project.xml";
            bool? result = dialog.ShowDialog(Application.Current.MainWindow);

            if (result.HasValue && result.Value)
            {
                string projectFilename = dialog.FileName;

                string contents = File.ReadAllText(projectFilename);

                List<ScreenImage> images;

                Project p = Project.FromXML(contents, out images);

                if (p != null)
                {
                    processDiagram.Source = null;

                    ProjectObject = p;
                    ProjectObject.ProjectFileLocation = projectFilename;

                    OpenSpanWPFWindow window = (OpenSpanWPFWindow)Application.Current.MainWindow;
                    foreach (ScreenImage si in images)
                    {
                        window.screenCapture._screenImages.Add(si);
                    }

                    window.screenCapture.IsEnabled = true;
                    window.outputInfo.IsEnabled = true;
                }
            }
        }

        private void CloseProject()
        {
            ProjectObject = null;
            OpenSpanWPFWindow window = (OpenSpanWPFWindow)Application.Current.MainWindow;
            window.screenCapture._screenImages.Clear();

            window.screenCapture.IsEnabled = false;
            window.outputInfo.IsEnabled = false;
        }

        public bool CanSave()
        {
            if (ProjectObject.ProjectFileLocation != null && ProjectObject.ProjectFileLocation != string.Empty)
            {
                return true;
            }

            return false;
        }

        public void Save()
        {
            if (!CanSave())
            {
                Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                dialog.DefaultExt = "xml";
                dialog.FileName = "project.xml";
                dialog.CheckPathExists = true;
                dialog.AddExtension = true;
                dialog.Filter = "XML Files|*.xml";
                bool? result = dialog.ShowDialog(Application.Current.MainWindow);
                if (result.HasValue && result.Value)
                {
                    ProjectObject.ProjectFileLocation = dialog.FileName;
                }
                else
                {
                    return;
                }
            }
            OpenSpanWPFWindow window = (OpenSpanWPFWindow)Application.Current.MainWindow;

            File.WriteAllText(ProjectObject.ProjectFileLocation, ProjectObject.ToXML(window.screenCapture._screenImages.ToList<ScreenImage>()));

            window.screenCapture.IsEnabled = true;
            window.outputInfo.IsEnabled = true;
        }

        private void btnCloseProject_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you wish to save your project?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Save();
            }

            CloseProject();
        }
    }
}
