using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobile.Validations;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class ResetPasswordPageViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
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


        private DelegateCommand _validateUserEmailCommand;
        public DelegateCommand ValidateUserEmail =>
           _validateUserEmailCommand ?? (_validateUserEmailCommand = new DelegateCommand(() => ExecuteValidateUserEmailCommand()));

        private ValidatableObject<string> _otp;

        public ValidatableObject<string> OTP
        {
            get
            {
                return _otp;
            }
            set
            {
                _otp = value;
                RaisePropertyChanged(() => _otp);
            }
        }
        private ValidatableObject<string> _userPassword;

        public ValidatableObject<string> UserPassword
        {
            get
            {
                return _userPassword;
            }
            set
            {
                _userPassword = value;
                RaisePropertyChanged(() => UserPassword);
            }
        }
        private ValidatableObject<string> _userConfrmPassword;

        public ValidatableObject<string> UserConfirmPassword
        {
            get
            {
                return _userConfrmPassword;
            }
            set
            {
                _userConfrmPassword = value;
                RaisePropertyChanged(() => UserConfirmPassword);
            }
        }

        private DelegateCommand _validatePasswordCommand;

        public DelegateCommand ValidatePasswordCommand =>
            _validatePasswordCommand ?? (_validatePasswordCommand = new DelegateCommand(() => ExecuteValidateUserPasswordCommand()));

        private DelegateCommand _validateConfirmPasswordCommand;

        public DelegateCommand ValidateConfirmPasswordCommand =>
            _validateConfirmPasswordCommand ?? (_validateConfirmPasswordCommand = new DelegateCommand(() => ExecuteValidateConfirmPasswordCommand()));

        private DelegateCommand _validateOTPCommand;
        public DelegateCommand ValidateOTPCommand =>
           _validateOTPCommand ?? (_validateOTPCommand = new DelegateCommand(() => ExecuteValidateOTPCommand()));


        private DelegateCommand _submitCommand;

        public DelegateCommand SubmitCommand =>
            _submitCommand ?? (_submitCommand = new DelegateCommand(async () => await ExecuteSubmitCommandAsync()));
        public ResetPasswordPageViewModel(IUserDialogs userDialogs, IAppSettings appSettings, IPageDialogService pageDialogService, INavigationService navigationService, IAccountService accountService) : base(navigationService, appSettings, pageDialogService)
        {
            UserEmail = new ValidatableObject<string>();
            UserPassword = new ValidatableObject<string>();
            UserConfirmPassword = new ValidatableObject<string>();
            OTP = new ValidatableObject<string>();

            UserEmail.Value = string.Empty;
            UserPassword.Value = string.Empty;
            UserConfirmPassword.Value = string.Empty;
            
            OTP.Value = string.Empty;
            _accountService = accountService;
           
          
            AddValidations();
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            string Email = parameters.GetValue<string>("Email");
            if (!string.IsNullOrEmpty(Email))
                UserEmail.Value = Email;
            else
                await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.Login.LoginPage}/{AppPages.Login.ForgotPasswordPage}", System.UriKind.Absolute));

            IsBusy = false;
        }
        private void AddValidations()
        {
            _userEmail.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Email is required"
            });
            _userEmail.Validations.Add(new EmailRule<string>
            {
                ValidationMessage = "Enter valid email address"
            });
            _otp.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "OTP is Required"
            });
            _userPassword.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Password is required"
            });
            _userPassword.Validations.Add(new PasswordRule<string>
            {
                ValidationMessage = "Password  should contain at least 8 characters, 1 numeric, 1 lowercase, 1 uppercase, 1 special character"
            });
            _userConfrmPassword.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Confirm password is required"
            });
            _userConfrmPassword.Validations.Add(new CompareRule<string>() { CompareFunction = () => _userPassword.Value, ValidationMessage = "Confirm password is not match" });
        }
        private bool ExecuteValidateUserEmailCommand()
        {
            return _userEmail.Validate();
        }
        private bool ExecuteValidateOTPCommand()
        {
            return _otp.Validate();
        }

        private async Task ExecuteSubmitCommandAsync()
        {
            try
            {
                IsBusy = true;
                ValidateFields();
                if (_userEmail.IsValid && _userPassword.IsValid && _userPassword.IsValid && _userConfrmPassword.IsValid)
                {
                    var responce = await _accountService.ChangePassword(UserEmail.Value, OTP.Value, UserPassword.Value);
                    if (responce)
                    {
                        IsBusy = false;
                        await PageDialogService.DisplayAlertAsync(null, "Password has been reset.", "Ok");

                        await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.NavigationPage}/{AppPages.Login.LoginPage}", System.UriKind.Absolute));
                    }
                    else
                    {
                        await PageDialogService.DisplayAlertAsync(null, "OTP Is InValid", "Ok");
                    }
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                
                IsBusy = false;
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
            }

        }
        private bool ExecuteValidateUserPasswordCommand()
        {
            return _userPassword.Validate();
        }
        private bool ExecuteValidateConfirmPasswordCommand()
        {
            return _userConfrmPassword.Validate();
        }
        public void ValidateFields()
        {

            ValidateUserEmail.Execute();
            ValidateOTPCommand.Execute();
            ValidatePasswordCommand.Execute();
            ValidateConfirmPasswordCommand.Execute();

        }
    }
}
