using System;
using System.Globalization;

using Xamarin.Forms;

namespace SSCMobile.Converters
{
    public class NullOrEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case string stringValue:
                    return !string.IsNullOrWhiteSpace(stringValue);

                default:
                    return value != null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}