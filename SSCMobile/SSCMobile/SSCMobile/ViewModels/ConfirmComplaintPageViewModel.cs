using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSCMobile.ViewModels
{
    public class ConfirmComplaintPageViewModel : ViewModelBase
    {
        #region Services

        private IUserDialogs _userDialogs;
        private IComplaintService _complaintService;

        #endregion Services

        private UrlWebViewSource _urlWebViewSource;

        public UrlWebViewSource UrlWebViewSourceObj
        {
            get { return _urlWebViewSource; }
            set { SetProperty(ref _urlWebViewSource, value); }
        }

        

        private bool _isRegisterButtonShow = false;

        public bool IsRegisterButtonShow
        {
            get
            {
                return _isRegisterButtonShow;
            }
            set
            {
                SetProperty(ref _isRegisterButtonShow, value);
            }
        }

        private bool _isEmailSend = false;

        public bool IsEmailSend
        {
            get
            {
                return _isEmailSend;
            }
            set
            {
                SetProperty(ref _isEmailSend, value);
            }
        } 
        private int _externalId;

        public int ExternalId
        {
            get
            {
                return _externalId;
            }
            set
            {
                SetProperty(ref _externalId, value);
            }
        }
        public ConfirmComplaintPageViewModel(IUserDialogs userDialogs, IComplaintService complaintService, IAppSettings appSettings, IPageDialogService pageDialogService, INavigationService navigationService) : base(navigationService, appSettings, pageDialogService)
        {
            _userDialogs = userDialogs;
            _complaintService = complaintService;
            //UrlWebViewSourceObj = new UrlWebViewSource();
        }
        private DelegateCommand _submitCommand;

        public DelegateCommand SubmitCommand =>
            _submitCommand ?? (_submitCommand = new DelegateCommand(async () => await ExecuteSubmitCommandAsync()));

        private async Task ExecuteSubmitCommandAsync()
        {
            if (_settings.IsOnline)
            {
                
                var IsConfirm = await _userDialogs.ConfirmAsync("Are you sure you want to register official complaint to authority?",null, "Yes", "No");
                if (IsConfirm == true)
                {
                    IsBusy = true;
                    
                    var responce = await _complaintService.SendMail(ExternalId);
                    if (responce == true)
                    {
                       await PageDialogService.DisplayAlertAsync(null, "Your complaint has been registered successfully. Thank you.", "OK");

                        IsBusy = false;
                        await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.DashBoard.ComplaintStatusPage}", System.UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        IsBusy = false;
                        await PageDialogService.DisplayAlertAsync(null, "Sorry, there seems some technical error in registering your complaint. Please try again later.", "OK");

                    }
                }
            }
            else
            {
                IsEmpty = true;
                EmptyStateTitle = AppAlertMessage.NoInternetConnections;
                IsBusy = false;
            }


        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (_settings.IsOnline)
            {
                IsBusy = true;
               
                IsEmailSend= parameters.GetValue<bool>("IsEmailSend");
                ExternalId = parameters.GetValue<int>("ExternalId");
                IsRegisterButtonShow = (IsEmailSend == false) ? true : false;
              
                UrlWebViewSourceObj = new UrlWebViewSource { Url = $"http://ssc.resquark.com/home/ComplaintEmail/{ExternalId}" };
                IsBusy = false;
            }
            else
            {
                IsEmpty = true;
                EmptyStateTitle = AppAlertMessage.NoInternetConnections;
                IsBusy = false;
            }

        }
        public async override void Initialize(INavigationParameters parameters)
        {

        }


    }
}
