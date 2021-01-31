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
    public class ShowEditAndConfirmButtonConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var settings = DependencyService.Get<IAppSettings>();
            var Controller = EnumExtensionMethods.GetEnumDescription(UserRole.Controller);
            var Admin = EnumExtensionMethods.GetEnumDescription(UserRole.Admin);
            var SiteAdmin = EnumExtensionMethods.GetEnumDescription(UserRole.SiteAdmin);


            ComplaintModel complaint = (ComplaintModel)value;

            if (complaint.ComplainStatus == (int)ComplaintStatusEnum.Pending || complaint.ComplainStatus == (int)ComplaintStatusEnum.Rejected)
            {
                if ((settings.UserRole.Equals(Controller)|| settings.UserRole.Equals(Admin) || settings.UserRole.Equals(SiteAdmin)) && complaint.CreatedBy.Equals(settings.UserId))
                {
                    return true;
                }
                else
                {
                    if (!settings.UserRole.Equals(Controller)|| settings.UserRole.Equals(Admin) || settings.UserRole.Equals(SiteAdmin))
                    {
                        return true;
                    }
                    return false;
                }
               
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
