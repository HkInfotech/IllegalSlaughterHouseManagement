using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace SSCMobile.Converters
{
    public class ShowViewComplaintButtonConverter : IValueConverter
    {
        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var complaint = (ComplaintModel)value;

            if (complaint.ComplainStatus==(int)ComplaintStatusEnum.Registered)
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