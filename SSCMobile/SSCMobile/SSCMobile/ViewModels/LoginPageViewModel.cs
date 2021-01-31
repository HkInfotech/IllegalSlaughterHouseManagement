using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Models;
using SSCMobile.Services.Implementations;
using SSCMobile.Services.Interfaces;
using SSCMobile.Validations;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using System;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        #region Services

        private IAccountService _accountService;
        private IUserDialogs _userDialogs;
        private IComplaintService _complaintService;

        #endregion Services

        #region Properties

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

        private ValidatableObject<string> _password;

        public ValidatableObject<string> Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        #endregion Properties

        #region Commands

        private DelegateCommand _loginCommand;

        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(async () => await ExecuteLoginCommandAsync()));

        private DelegateCommand _validateUserNameCommand;

        public DelegateCommand ValidateUserNameCommand =>
            _validateUserNameCommand ?? (_validateUserNameCommand = new DelegateCommand(() => ExecuteValidateUserNameCommand()));

        private DelegateCommand _validateUserPassword;

        public DelegateCommand ValidateUserPasswordCommand =>
            _validateUserPassword ?? (_validateUserPassword = new DelegateCommand(() => ExecuteValidateUserPasswordCommand()));

        #endregion Commands

        #region Constructor

        public LoginPageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IComplaintService complaintService) : base(navigationservice, settings, pageDialogService)
        {
            UserName = new ValidatableObject<string>();
            Password = new ValidatableObject<string>();
            _complaintService = complaintService;
            AddValidations();
            _userDialogs = userDialogs;
        }

        #endregion Constructor

        #region Methods

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        private void AddValidations()
        {
            _userName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Email is required"
            });
            _userName.Validations.Add(new EmailRule<string>
            {
                ValidationMessage = "Please enter valid email address"
            });

            _userName.Validations.Add(new EmailRule<string>
            {
                ValidationMessage = "Please enter password"
            });

            //_password.Validations.Add(new PasswordRule<string>
            //{
            //    ValidationMessage = "Password  should contain at least 8 characters, 1 numeric, 1 lowercase, 1 uppercase, 1 special character"
            //});
        }

        private async Task<bool> ExecuteLoginCommandAsync()
        {
            try
            {
                ValidateFields();
                if (_userName.IsValid && _password.IsValid)
                {
                    _userDialogs.ShowLoading("Loading", null);
                    LoginRequest loginRequest = new LoginRequest();
                    loginRequest.Email = UserName.Value;
                    loginRequest.Password = Password.Value;
                    _accountService = new AccountService();
                    _complaintService.DeleteCity();
                    var responce = await _accountService.RequestToken(loginRequest);

                    if (responce == true)
                    {
                        var UserInfo = await _accountService.GetUserInfo();


                        _settings.UserId = UserInfo.UserId;
                        _settings.UserName = UserInfo.UserId;
                        _settings.UserCity = UserInfo.City;
                        _settings.Email = UserInfo.Email;
                        _settings.PhoneNumber = UserInfo.PhoneNumber;
                        _settings.Name = UserInfo.Name;
                        _settings.UserRole = UserInfo.UserRole;
                        _settings.IsLogin = true;
                       await _complaintService.GetCities("");
                        var CityModel = await _complaintService.GetCityByName(UserInfo.City);
                        if (CityModel == null)
                        {
                            _userDialogs.HideLoading();
                            await PageDialogService.DisplayAlertAsync(null, "Sorry, for your City this APP is not active right now.", "OK");
                            _settings.IsLogin2 = false;

                            return true;
                        }
                        _settings.UserCityId = CityModel.ExternalId;
                        var Lows = await _complaintService.GetAllLows(_settings.UserId, CityModel.ExternalId);
                        _settings.UserCityId = CityModel.ExternalId;
                        _userDialogs.HideLoading();
                        await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}", System.UriKind.Absolute));
                    }
                    else
                    {
                        _userDialogs.HideLoading();
                        await PageDialogService.DisplayAlertAsync("Login", "Invalid login credentials", "OK");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _userDialogs.HideLoading();
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
                return true;
            }
        }

        public void ValidateFields()
        {
            ValidateUserPasswordCommand.Execute();
            ValidateUserNameCommand.Execute();
        }

        private bool ExecuteValidateUserNameCommand()
        {
            return _userName.Validate();
        }

        private bool ExecuteValidateUserPasswordCommand()
        {
            return _password.Validate();
        }

        #endregion Methods
    }
}