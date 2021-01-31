using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SSCMobile.Converters
{
    public class ComplaintStatusButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value) 
            {
               
                case (int)ComplaintStatusEnum.Registered:
                    {
                        return "VIEW";
                      
                    }
                case (int)ComplaintStatusEnum.Rejected:
                    {
                        return "REVIEW & EDIT";
                        
                    }
                case (int)ComplaintStatusEnum.Verified:
                    {
                        return "REGISTER";
                       
                    }
                case (int)ComplaintStatusEnum.Pending:
                default:
                
                    {
                        return "REVIEW & EDIT";
                      
                    }
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
