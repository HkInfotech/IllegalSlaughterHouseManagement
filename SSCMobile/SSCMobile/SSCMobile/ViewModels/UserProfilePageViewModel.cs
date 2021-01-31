using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Implementations;
using SSCMobile.Services.Interfaces;
using SSCMobile.Validations;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class UserProfilePageViewModel : ViewModelBase
    {

        #region Services

        private IAccountService _accountService;
        private IUserDialogs _userDialogs;
        private IComplaintService _complaintService;


        #endregion Services
        #region Properties
        private ValidatableObject<string> _userEmail;

        public ValidatableObject<string> UserEmail
        {
            get
            {
                return _userEmail;
            }
            set
            {
                _userEmail = value;
                RaisePropertyChanged(() => UserEmail);
            }
        }

        private List<CitysModel> _citiesList;

        public List<CitysModel> CityList
        {
            get { return _citiesList; }
            set { SetProperty(ref _citiesList, value); }
        }


        private ValidatableObject<string> _userMobile;

        public ValidatableObject<string> UserMobile
        {
            get
            {
                return _userMobile;
            }
            set
            {
                _userMobile = value;
                RaisePropertyChanged(() => UserMobile);
            }
        }

        private ValidatableObject<string> _userCity;

        public ValidatableObject<string> UserCity
        {
            get
            {
                return _userCity;
            }
            set
            {
                _userCity = value;
                RaisePropertyChanged(() => UserCity);
            }
        }
        private ValidatableObject<string> _userName;

        public ValidatableObject<string> UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }
        #endregion


        private DelegateCommand _editUserprofileCommand;

        public DelegateCommand EditUserCommad =>
            _editUserprofileCommand ?? (_editUserprofileCommand = new DelegateCommand(async () => await ExecuteEditUserPropeCommandAsync()));

        private DelegateCommand _changepasswordCommand;

        public DelegateCommand ChangepasswordCommand =>
            _changepasswordCommand ?? (_changepasswordCommand = new DelegateCommand(async () =>
            {
                IsBusy = true;
                var responce = await _accountService.ForgotPassword(_settings.Email.Trim());
                if (responce)
                {
                    IsBusy = false;
                    await NavigationService.NavigateAsync(AppPages.DashBoard.ChangePasswordPage);
                }
                IsBusy = false;
            }));


        private async Task ExecuteEditUserPropeCommandAsync()
        {
            await NavigationService.NavigateAsync(AppPages.DashBoard.EditUserProfilePage);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsBusy = false;
        }

        public UserProfilePageViewModel(INavigationService navigationService, IAppSettings settings, IPageDialogService pageDialogService) : base(navigationService, settings, pageDialogService)
        {
            UserEmail = new ValidatableObject<string>();
            UserName = new ValidatableObject<string>();
            UserCity = new ValidatableObject<string>();
            UserMobile = new ValidatableObject<string>();
            UserEmail.Value = _settings.Email;
            UserName.Value = _settings.Name;
            UserCity.Value = _settings.UserCity;
            UserMobile.Value = _settings.PhoneNumber;
            _accountService = new AccountService();
        }

    }
}
