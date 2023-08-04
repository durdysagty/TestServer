using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TestClient.Models;
using TestModels;

namespace TestClient.Services
{
    public class TestImageToVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is TestImage buttonContent && values[1] is ObservableCollection<TestImage> answers)
            {
                return !answers.Select(a => a.Order).Contains(buttonContent.Order);
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
