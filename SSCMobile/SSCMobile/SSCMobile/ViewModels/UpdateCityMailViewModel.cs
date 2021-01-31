using Acr.UserDialogs;
using ImTools;
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
    public class UpdateCityMailViewModel : ViewModelBase
    {
        private readonly IComplaintService _complaintService;


        private ValidatableObject<string> _mCEmail;

        public ValidatableObject<string> MCEmail
        {
            get
            {
                return _mCEmail;
            }
            set
            {
                _mCEmail = value;
                RaisePropertyChanged(() => MCEmail);
            }
        }
        private ValidatableObject<string> _fCCIEmail;

        public ValidatableObject<string> FCCIEmail
        {
            get
            {
                return _fCCIEmail;
            }
            set
            {
                _fCCIEmail = value;
                RaisePropertyChanged(() => FCCIEmail);
            }
        }



        private DelegateCommand _validateMCEmailCommand;

        public DelegateCommand ValidateMCEmailCommand =>
            _validateMCEmailCommand ?? (_validateMCEmailCommand = new DelegateCommand(() => ExecuteValidateMCEmailCommand()));


        private DelegateCommand _validateFCCIEmailCommand;

        public DelegateCommand ValidateFCCIEmailCommand =>
            _validateFCCIEmailCommand ?? (_validateFCCIEmailCommand = new DelegateCommand(() => ExecuteValidateFCCIEmailCommand()));

        private DelegateCommand saveCommand;

        public DelegateCommand SaveCommand =>
            saveCommand ?? (saveCommand = new DelegateCommand(async () => await SaveCommandExecute()));

        private async Task SaveCommandExecute()
        {
            try
            {


                if (_settings.IsOnline)
                {
                    IsBusy = true;
                    ValidateFields();
                    if (_mCEmail.IsValid && _fCCIEmail.IsValid)
                    {
                        var response = await _complaintService.ConfigCityMail(MCEmail.Value.Trim(), FCCIEmail.Value.Trim(), _settings.UserCityId);
                        IsBusy = false;
                        if (response == true)
                        {

                            await PageDialogService.DisplayAlertAsync(null, "Your detail has been save successfully.", "OK");

                        }

                    }
                    else
                    {
                        IsBusy = false;
                        await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.NoInternetConnections, "OK");
                    }
                }
                else
                {
                    IsBusy = false;
                    await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");

                }
            }
            catch (Exception)
            {

                IsBusy = false;
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
            }

        }

        public UpdateCityMailViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IComplaintService complaintService) : base(navigationservice, settings, pageDialogService)
        {
            _complaintService = complaintService;
            var UserCity = _complaintService.GetUserCity();
            MCEmail = new ValidatableObject<string>();
            FCCIEmail = new ValidatableObject<string>();
            MCEmail.Value = UserCity.MCEmail;
            FCCIEmail.Value = UserCity.FcciEmail;
            IsBusy = false;
            AddValidations();
        }





        private void AddValidations()
        {
            _fCCIEmail.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Email is required"
            });
            //_fCCIEmail.Validations.Add(new EmailRule<string>
            //{
            //    ValidationMessage = "Please enter valid email address"
            //});

            _mCEmail.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Email is required"
            });
            //_mCEmail.Validations.Add(new EmailRule<string>
            //{
            //    ValidationMessage = "Please enter valid email address"
            //});
        }

        public void ValidateFields()
        {
            ValidateFCCIEmailCommand.Execute();
            ValidateMCEmailCommand.Execute();
        }
        private bool ExecuteValidateMCEmailCommand()
        {
            return _mCEmail.Validate();
        }
        private bool ExecuteValidateFCCIEmailCommand()
        {
            return _fCCIEmail.Validate();
        }

    }
}
