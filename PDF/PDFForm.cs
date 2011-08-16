using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProcessCapture.PDF
{
    public partial class PDFForm : UserControl
    {
        public PDFForm(string filename)
        {
            InitializeComponent();

            axAcroPDF1.LoadFile(filename);
        }

        public void Dispose()
        {
            axAcroPDF1.Dispose();

            base.Dispose();
        }
    }
}
