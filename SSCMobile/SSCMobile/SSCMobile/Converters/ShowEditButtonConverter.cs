using SSCMobileServiceBus.OfflineSync.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace SSCMobile.Converters
{
    public class ShowEditButtonConverter : IValueConverter
    {
        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int status = (int)value;
            if (status == (int)ComplaintStatusEnum.Pending || status == (int)ComplaintStatusEnum.Non|| status == (int)ComplaintStatusEnum.Rejected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}