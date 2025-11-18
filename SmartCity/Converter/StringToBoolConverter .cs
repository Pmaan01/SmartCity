using System.Globalization;
using Microsoft.Maui.Controls;

namespace SmartCity.Converters
{
    public class HasValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return !string.IsNullOrWhiteSpace(str);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsGreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intVal)
            {
                return intVal > 0;
            }
            if (value is decimal decimalVal)
            {
                return decimalVal > 0;
            }
            if (value is double doubleVal)
            {
                return doubleVal > 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}