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
using OpenSpanWPF;
using ProcessCapture.PDF;
using ProcessCapture.Screenshot;
using Microsoft.Win32;
using System.Diagnostics;

namespace ProcessCapture.TabControls
{
    /// <summary>
    /// Interaction logic for OutputTab.xaml
    /// </summary>
    public partial class OutputTab : TabItem
    {
        bool Word2007Installed = false;
        bool Word2003Installed = false;

        public OutputTab()
        {
            InitializeComponent();

            //Check HKEY_CLASSES_ROOT\Word.Application\CurVer
            //to see if Word 2007 is installed

            RegistryKey regWord = Registry.ClassesRoot.OpenSubKey("Word.Application");

            if (regWord != null)
            {
                string value = (string)regWord.OpenSubKey("CurVer").GetValue("");
                if (value != null && value == "Word.Application.12")
                {
                    Word2007Installed = true;
                }
                else if (value != null && value == "Word.Application.11")
                {
                    Word2003Installed = true;
                }
            }
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            if (Word2007Installed)
            {
                dialog.Filter = "Word Doc|*.docx|PDF File|*.pdf";
            }
            else if (Word2003Installed)
            {
                dialog.Filter = "Word Doc|*.doc|PDF File|*.pdf";
            }
            else
            {
                dialog.Filter = "PDF File|*.pdf";
            }

            bool? result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                fileLocation.Text = dialog.FileName;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            fileLocation.Text = "";
            //pdfViewer.DisposeCurrent();
        }

        private void btnGeneratePDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenSpanWPFWindow window = Application.Current.MainWindow as OpenSpanWPFWindow;

                if (window != null)
                {
                    Project proj = window.processInfo.ProjectObject;

                    if (this.fileLocation.Text.ToLower().EndsWith(".pdf"))
                    {
                        PDFDocument.CreateDocument(
                            "OpenSpan Process Discovery Document", proj.ProcessName, proj.Description, proj.Author, proj.ProcessImage,
                            window.screenCapture._screenImages.ToList<ScreenImage>(), this.fileLocation.Text, modules.IsChecked.Value);
                    }
                    else
                    {
                        WordDocument.CreateDocument(
                            "OpenSpan Process Discovery Document", proj.ProcessName, proj.Description, proj.Author, proj.ProcessImage,
                            window.screenCapture._screenImages.ToList<ScreenImage>(), this.fileLocation.Text, modules.IsChecked.Value, Word2003Installed);
                    }
                    //pdfViewer.Loadfile(fileLocation.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                System.IO.File.WriteAllText("C:\\error.log", ex.ToString());
            }
        }

        public void DisposeChildren()
        {
            //pdfViewer.Dispose();
        }

        
    }

    public class ObjectToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                return value.ToString().Length != 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
