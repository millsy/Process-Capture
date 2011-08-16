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

namespace ProcessCapture.PDF
{
	/// <summary>
	/// Interaction logic for PDFViewer.xaml
	/// </summary>
    public partial class PDFViewer : Grid
	{

        public PDFViewer()
        {
            this.InitializeComponent();
        }

        public void Loadfile(string file)
        {
            DisposeCurrent();

            PDFForm pdf = new PDFForm(file);
            //pdf.Width = (int) this.ActualWidth - 10;
            //pdf.Height = 500;
            host.Child = pdf;
        }

        public void DisposeCurrent()
        {
            if (host.Child != null)
            {
                PDFForm p = (PDFForm)host.Child;
                host.Child = null;
                p.Dispose();
            }
        }

        public void Dispose()
        {
            if (host.Child != null)
            {
                PDFForm p = (PDFForm)host.Child;
                host.Child = null;
                p.Dispose();
            }

            host.Dispose();
        }
	}
}