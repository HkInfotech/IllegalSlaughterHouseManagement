using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Globalization;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using SSCMobileDataBus.OfflineStore.Models.Commons;

namespace SSCMobile.Converters
{
    public class ShowMoreButtonConverter : IValueConverter
    {
        int ExternalId = 0;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var settings = DependencyService.Get<IAppSettings>();
            var Controller = EnumExtensionMethods.GetEnumDescription(UserRole.Controller);
            var Volunteer = EnumExtensionMethods.GetEnumDescription(UserRole.Volunteer);
            var Admin = EnumExtensionMethods.GetEnumDescription(UserRole.Admin);

            switch ((int)value)
            {
                

                case (int)ComplaintStatusEnum.Registered:
                    {
                        return false;
                    }
                case (int)ComplaintStatusEnum.Rejected:
                    {
                        return true;
                        

                    }
                case (int)ComplaintStatusEnum.Verified:
                    {
                        if (settings.UserRole.Equals(Controller)|| settings.UserRole.Equals(Admin)) 
                        {
                            return false;
                        }
                        return "Register";

                    }
                case (int)ComplaintStatusEnum.Pending:
                default:

                    {
                        return true;

                    }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
