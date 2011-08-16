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
using ProcessCapture.Screenshot;

namespace ProcessCapture.UserControls
{
    /// <summary>
    /// Interaction logic for ImageLayout.xaml
    /// </summary>
    public partial class ImageLayout : UserControl
    {      
        public ImageLayout()
        {
            InitializeComponent();
        }

        public void Setup(string path, string title)
        {
            Image img = (Image)(((Grid)((Border)this.Content).Child).Children[0]);
            TextBlock tb = (TextBlock)(((Grid)((Border)this.Content).Child).Children[1]);

            tb.Text = title;

            Image i = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(path);
            src.EndInit();

            img.Source = src;

            this.UpdateLayout();
        }

        public void ShowTitle()
        {
            TextBlock tb = (TextBlock)(((Grid)((Border)this.Content).Child).Children[1]);
            tb.Visibility = Visibility.Visible;
        }
    }
}
