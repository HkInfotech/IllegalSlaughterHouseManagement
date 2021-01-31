using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace SSCMobile.Converters
{
    public class ShowRegisterButtonConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var settings = DependencyService.Get<IAppSettings>();
            var Controller = EnumExtensionMethods.GetEnumDescription(UserRole.Controller);
            var Admin = EnumExtensionMethods.GetEnumDescription(UserRole.Admin);
            var SiteAdmin = EnumExtensionMethods.GetEnumDescription(UserRole.SiteAdmin);

            ComplaintModel complaint = (ComplaintModel)value;
            if (complaint.ComplainStatus == (int)ComplaintStatusEnum.Verified && complaint.CreatedBy.Equals(settings.UserId))
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