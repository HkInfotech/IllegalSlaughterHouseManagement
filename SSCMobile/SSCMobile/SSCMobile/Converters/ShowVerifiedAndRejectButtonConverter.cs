using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Globalization;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;

namespace SSCMobile.Converters
{
    public class ShowVerifiedAndRejectButtonConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var settings = DependencyService.Get<IAppSettings>();
            var Controller = EnumExtensionMethods.GetEnumDescription(UserRole.Controller);
            var Admin = EnumExtensionMethods.GetEnumDescription(UserRole.Admin);
            var SiteAdmin = EnumExtensionMethods.GetEnumDescription(UserRole.SiteAdmin);

            ComplaintModel status = (ComplaintModel)value;

            if ((settings.UserRole.Equals(Controller)|| settings.UserRole.Equals(Admin)|| settings.UserRole.Equals(SiteAdmin)) && status.ComplainStatus != (int)ComplaintStatusEnum.Registered && status.IsEmailSend == false)
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
