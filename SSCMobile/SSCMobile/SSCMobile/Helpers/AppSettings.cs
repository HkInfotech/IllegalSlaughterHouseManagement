using MvvmHelpers;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using SSCMobile.Helpers;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using System.ComponentModel;

[assembly: Xamarin.Forms.Dependency(typeof(AppSettings))]

namespace SSCMobile.Helpers
{
    public class AppSettings : BaseViewModel, IAppSettings, INotifyPropertyChanged
    {
        private ISettings AppSetting
        {
            get { return CrossSettings.Current; }
        }

        public string ApplicationName
        {
            get => AppSetting.GetValueOrDefault(nameof(ApplicationName), string.Empty);
            set => AppSetting.AddOrUpdateValue(nameof(ApplicationName), value);
        }

        public string BaseUrl
        {
            get => AppSetting.GetValueOrDefault(nameof(BaseUrl), "http://ssc.resquark.com");
            set => AppSetting.AddOrUpdateValue(nameof(BaseUrl), value);
        }

        public string Token
        {
            get => AppSetting.GetValueOrDefault(nameof(Token), string.Empty);
            set => AppSetting.AddOrUpdateValue(nameof(Token), value);
        }
        public string UserRole
        {
            get => AppSetting.GetValueOrDefault(nameof(UserRole), string.Empty);
            set => AppSetting.AddOrUpdateValue(nameof(UserRole), value);
        }

        public bool IsLogin
        {
            get => AppSetting.GetValueOrDefault(nameof(IsLogin), false);
            set => AppSetting.AddOrUpdateValue(nameof(IsLogin), value);
        }

        public bool IsLogin2
        {
            get => AppSetting.GetValueOrDefault(nameof(IsLogin2), false);
            set => AppSetting.AddOrUpdateValue(nameof(IsLogin2), value);
        }


        public bool IsOnline
        {
            get => AppSetting.GetValueOrDefault(nameof(IsOnline), false);
            set => AppSetting.AddOrUpdateValue(nameof(IsOnline), value);
        }

        public string UserName
        {
            get => AppSetting.GetValueOrDefault(nameof(UserName), string.Empty);
            set => AppSetting.AddOrUpdateValue(nameof(UserName), value);
        }
        public string UserCity
        {
            get => AppSetting.GetValueOrDefault(nameof(UserCity), string.Empty);
            set => AppSetting.AddOrUpdateValue(nameof(UserCity), value);
        }

        public int UserCityId
        {
            get => AppSetting.GetValueOrDefault(nameof(UserCityId), 0);
            set => AppSetting.AddOrUpdateValue(nameof(UserCityId), value);
        }

        public string UserId
        {
            get => AppSetting.GetValueOrDefault(nameof(UserId), string.Empty);
            set => AppSetting.AddOrUpdateValue(nameof(UserId), value);
        }

        public string Email
        {
            get => AppSetting.GetValueOrDefault(nameof(Email), string.Empty);
            set => AppSetting.AddOrUpdateValue(nameof(Email), value);
        }
        public string PhoneNumber
        {
            get => AppSetting.GetValueOrDefault(nameof(PhoneNumber), string.Empty);
            set => AppSetting.AddOrUpdateValue(nameof(PhoneNumber), value);
        }
        public string Name
        {
            get => AppSetting.GetValueOrDefault(nameof(Name), string.Empty);
            set => AppSetting.AddOrUpdateValue(nameof(Name), value);
        }
    }
}