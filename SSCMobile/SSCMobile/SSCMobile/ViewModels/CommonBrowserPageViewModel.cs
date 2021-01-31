using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace SSCMobile.ViewModels
{
    public class CommonBrowserPageViewModel : ViewModelBase
    {
        private UrlWebViewSource _urlWebViewSource;

        public UrlWebViewSource UrlWebViewSourceObj
        {
            get { return _urlWebViewSource; }
            set { SetProperty(ref _urlWebViewSource, value); }
        }
        public CommonBrowserPageViewModel(INavigationService navigationservice, IAppSettings settings, IPageDialogService pageDialogService) : base(navigationservice, settings, pageDialogService)
        {
            IsEmpty = false;
            IsBusy = false;
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsEmpty = false;
            IsBusy = false;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            if (_settings.IsOnline)
            {
              
                var BrowserUrl = parameters.GetValue<string>("BrowserUrl");
              
                UrlWebViewSourceObj = new UrlWebViewSource { Url = $"{BrowserUrl}" };
               
            }
            else
            {
                IsEmpty = true;
                EmptyStateTitle = AppAlertMessage.NoInternetConnections;
                IsBusy = false;
            }
        }
    }
}
