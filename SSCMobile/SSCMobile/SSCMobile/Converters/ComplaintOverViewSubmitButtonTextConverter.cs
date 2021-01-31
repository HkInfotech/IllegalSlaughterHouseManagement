using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Globalization;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
namespace SSCMobile.Converters
{
   public class ComplaintOverViewSubmitButtonTextConverter:IValueConverter  
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {

                case (int)ComplaintStatusEnum.Registered:
                    {
                        return "View";

                    }
                case (int)ComplaintStatusEnum.Rejected:
                    {
                        return "Submit";

                    }
                case (int)ComplaintStatusEnum.Verified:
                    {
                        return "Register";

                    }
                case (int)ComplaintStatusEnum.Pending:
                default:

                    {
                        return "Confirm";

                    }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
