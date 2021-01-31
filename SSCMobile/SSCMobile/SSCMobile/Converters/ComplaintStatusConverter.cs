using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SSCMobile.Converters
{
    public class ComplaintStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value) 
            {
               
                case (int)ComplaintStatusEnum.Registered:
                    {
                        return "Registered";
                      
                    }
                case (int)ComplaintStatusEnum.Rejected:
                    {
                        return "Rejected";
                        
                    }
                case (int)ComplaintStatusEnum.Verified:
                    {
                        return "Verified";
                       
                    }
                case (int)ComplaintStatusEnum.Pending:
                default:
                
                    {
                        return "Pending";
                      
                    }
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
