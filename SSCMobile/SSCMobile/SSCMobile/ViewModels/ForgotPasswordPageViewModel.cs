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
    public class ForgotPasswordPageViewModel : ViewModelBase
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

        private DelegateCommand _submitCommand;

        public DelegateCommand SubmitCommand =>
            _submitCommand ?? (_submitCommand = new DelegateCommand(async () => await ExecuteSubmitCommandAsync()));

        public ForgotPasswordPageViewModel(IUserDialogs userDialogs, IAppSettings appSettings, IPageDialogService pageDialogService, INavigationService navigationService, IAccountService accountService) : base(navigationService, appSettings, pageDialogService)
        {
            UserEmail = new ValidatableObject<string>();
            UserEmail.Value = string.Empty;
            _accountService = accountService;
            AddValidations();
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
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
        }

        private bool ExecuteValidateUserEmailCommand()
        {
            return _userEmail.Validate();
        }
        private async Task ExecuteSubmitCommandAsync()
        {
            try
            {
                ValidateFields();
                if (_userEmail.IsValid)
                {
                    IsBusy = true;
                    var responce = await _accountService.ForgotPassword(_userEmail.Value);
                    if (responce)
                    {
                        IsBusy = false;
                        NavigationParameters navigationParameters = new NavigationParameters();
                        navigationParameters.Add("Email", UserEmail.Value);
                        await NavigationService.NavigateAsync(AppPages.Login.ResetPasswordPage, navigationParameters);
                    }
                    else
                    {
                        IsBusy = false;
                        await PageDialogService.DisplayAlertAsync(null, "Please enter valid email address", "OK");
                    }
                    IsBusy = false;
                }
               
            }
            catch (Exception ex)
            {
             
                IsBusy = false;
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
            }

        }
        public void ValidateFields()
        {

            ValidateUserEmail.Execute();

        }
    }
}
