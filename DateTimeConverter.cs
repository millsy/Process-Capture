using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;
using ProcessCapture.Screenshot;

namespace OpenSpanWPF
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? selectedDate = value as DateTime?;

            if (selectedDate != null)
            {
                string dateTimeFormat = parameter as string;
                return selectedDate.Value.ToString(dateTimeFormat);
            }

            return "Select Date";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {

                var valor = value as string;
                if (!string.IsNullOrEmpty(valor))
                {
                    var retorno = DateTime.Parse(valor);
                    return retorno;
                }

                return null;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }

    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<ScreenImage> coll = (Application.Current.MainWindow as OpenSpanWPFWindow).screenCapture._screenImages;
            return coll.IndexOf(value as ScreenImage) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CanMoveUpConerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<ScreenImage> coll = (Application.Current.MainWindow as OpenSpanWPFWindow).screenCapture._screenImages;

            if (coll.IndexOf(value as ScreenImage) == 0 || coll.Count == 1)
            {
                //first record
                return false;// Visibility.Hidden;
            }

            return true;// Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CanMoveDownConerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<ScreenImage> coll = (Application.Current.MainWindow as OpenSpanWPFWindow).screenCapture._screenImages;

            if ((coll.IndexOf(value as ScreenImage) == coll.Count - 1) || coll.Count == 1)
            {
                //last record
                return false;// Visibility.Hidden;
            }

            return true;// Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
