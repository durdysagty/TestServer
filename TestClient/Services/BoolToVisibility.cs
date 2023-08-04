using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TestClient.Services
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invertConversion = false;
            if (parameter != null)
            {
                _ = bool.TryParse(parameter.ToString(), out invertConversion);
            }

            bool booleanValue = value is bool boolean && boolean;
            if (invertConversion)
            {
                return booleanValue ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return booleanValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }
}
