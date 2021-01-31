﻿using Acr.UserDialogs;
using Prism.Commands;
using Prism.Common;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobile.Validations;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class SaveCityPageViewModel : ViewModelBase
    {
        private readonly IComplaintService _complaintService;
        private readonly IUserDialogs _userDialogs;

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
        private ValidatableObject<string> _cityName;

        public ValidatableObject<string> CityName
        {
            get
            {
                return _cityName;
            }
            set
            {
                _cityName = value;
                RaisePropertyChanged(() => CityName);
            }
        }


        private ValidatableObject<string> _mobilenumber;

        public ValidatableObject<string> MobileNumber
        {
            get
            {
                return _mobilenumber;
            }
            set
            {
                _mobilenumber = value;
                RaisePropertyChanged(() => MobileNumber);
            }
        }

        private ValidatableObject<bool> _isActive;

        public ValidatableObject<bool> IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                RaisePropertyChanged(() => IsActive);
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
        private ValidatableObject<string> _adminEmail;

        public ValidatableObject<string> AdminEmail
        {
            get
            {
                return _adminEmail;
            }
            set
            {
                _adminEmail = value;
                RaisePropertyChanged(() => _adminEmail);
            }
        }
        private DelegateCommand _validateMCEmailCommand;

        public DelegateCommand ValidateMCEmailCommand =>
            _validateMCEmailCommand ?? (_validateMCEmailCommand = new DelegateCommand(() => ExecuteValidateMCEmailCommand()));

        private DelegateCommand _validateMobileNumberCommand;

        public DelegateCommand ValidateMobileNumberCommand =>
            _validateMobileNumberCommand ?? (_validateMobileNumberCommand = new DelegateCommand(() => ExecuteValidateMobileNumberCommand()));



        private DelegateCommand _validateFCCIEmailCommand;

        public DelegateCommand ValidateFCCIEmailCommand =>
            _validateFCCIEmailCommand ?? (_validateFCCIEmailCommand = new DelegateCommand(() => ExecuteValidateFCCIEmailCommand()));


        private DelegateCommand _validateAdminEmailCommand;

        public DelegateCommand ValidateAdminEmailCommand =>
            _validateAdminEmailCommand ?? (_validateAdminEmailCommand = new DelegateCommand(() => ExecuteValidateAdminEmailCommand()));

        private DelegateCommand _validateCityNameCommand;

        public DelegateCommand ValidateCityNameCommand =>
            _validateCityNameCommand ?? (_validateCityNameCommand = new DelegateCommand(() => ExecuteValidateCityNameCommand()));

        private DelegateCommand saveCommand;

        public DelegateCommand SaveCommand =>
            saveCommand ?? (saveCommand = new DelegateCommand(async () => await SaveCommandExecute()));


        private CityDTO selectedCity;

        public CityDTO SelectedCityDTO
        {
            get { return selectedCity; }
            set { SetProperty(ref selectedCity, value); }
        }

        public SaveCityPageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IComplaintService complaintService) : base(navigationservice, settings, pageDialogService)
        {
            _complaintService = complaintService;
            _userDialogs = userDialogs;
            MCEmail = new ValidatableObject<string>();
            FCCIEmail = new ValidatableObject<string>();
            AdminEmail = new ValidatableObject<string>();
            CityName = new ValidatableObject<string>();
            IsActive = new ValidatableObject<bool>();
            MobileNumber = new ValidatableObject<string>();

            AddValidations();
        }
        async public override void OnNavigatedTo(INavigationParameters parameters)
        {

            //var cityDTO = parameters.GetValues<CityDTO>("city");

            if (parameters.TryGetValue("City", out CityDTO cityDTO))
            {
                IsBusy = true;
                SelectedCityDTO = new CityDTO();
                SelectedCityDTO = cityDTO;
                MCEmail.Value = cityDTO.MCEmail;
                FCCIEmail.Value = cityDTO.FcciEmail;
                AdminEmail.Value = cityDTO.AdminEmail;
                CityName.Value = cityDTO.CityName;
                IsActive.Value = cityDTO.IsActive ?? false;
                IsBusy = false;
            }
            else
            {
                await NavigationService.GoBackAsync();
            }
            base.OnNavigatedTo(parameters);
        }


        private async Task SaveCommandExecute()
        {
            try
            {
                ValidateFields();
                if (_settings.IsOnline == true)
                {

                    if ((CityName.IsValid && FCCIEmail.IsValid) && (MCEmail.IsValid && AdminEmail.IsValid))
                    {
                        CityDTO cityDTO = new CityDTO();
                        cityDTO.Id = SelectedCityDTO.Id;
                        cityDTO.CityName = CityName.Value.Trim();
                        cityDTO.CountryId = 1;
                        cityDTO.IsActive = IsActive.Value;
                        cityDTO.FcciEmail = FCCIEmail.Value.Trim();
                        cityDTO.MCEmail = MCEmail.Value.Trim();
                        cityDTO.AdminEmail = AdminEmail.Value.Trim();
                        cityDTO.CreatedBy = SelectedCityDTO.CreatedBy.Trim();
                        cityDTO.ModifiedBy = SelectedCityDTO.ModifiedBy.Trim();
                        cityDTO.CreatedDate = SelectedCityDTO.CreatedDate;
                        cityDTO.ModifiedDate = SelectedCityDTO.ModifiedDate;
                        cityDTO.MobileNo = SelectedCityDTO.MobileNo ?? "";
                        cityDTO.IsCreate = false;
                        cityDTO.IsEmptyModel = false;
                        var result = await _complaintService.UpdateCityOnServer(cityDTO);
                        if (result.ResponeContent == false)
                        {
                            await PageDialogService.DisplayAlertAsync(null, result.Message, "Ok");
                        }
                        else
                        {
                            await PageDialogService.DisplayAlertAsync(null, "City details has been updated successfully.", "Ok");
                            await NavigationService.GoBackAsync();
                        }
                    }
                }
                else
                {
                    await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.NoInternetConnections, "Ok");
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "Ok");

            }

        }
        private void AddValidations()
        {
            //_fCCIEmail.Validations.Add(new IsNotNullOrEmptyRule<string>
            //{
            //    ValidationMessage = "Email is required"
            //});
            //_fCCIEmail.Validations.Add(new CityEmailRule<string>
            //{
            //    ValidationMessage = "Please enter valid email address"
            //});

            //_mCEmail.Validations.Add(new IsNotNullOrEmptyRule<string>
            //{
            //    ValidationMessage = "Email is required"
            //});

            //_mCEmail.Validations.Add(new CityEmailRule<string>
            //{
            //    ValidationMessage = "Please enter valid email address"
            //});

            _adminEmail.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Email is required"
            });
            _adminEmail.Validations.Add(new EmailRule<string>
            {
                ValidationMessage = "Please enter valid email address"
            });

            _mobilenumber.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Mobile number is required"
            });
            _mobilenumber.Validations.Add(new MobilenumberRule<string>
            {
                ValidationMessage = "Please enter valid mobile address"
            });

            _cityName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "City name is required"
            });
        }

        public void ValidateFields()
        {
            ValidateFCCIEmailCommand.Execute();
            ValidateMCEmailCommand.Execute();
            ValidateAdminEmailCommand.Execute();
            ValidateCityNameCommand.Execute();
            ValidateMobileNumberCommand.Execute();
        }
        private bool ExecuteValidateMCEmailCommand()
        {
            return _mCEmail.Validate();
        }
        private bool ExecuteValidateFCCIEmailCommand()
        {
            return _fCCIEmail.Validate();
        }
        private bool ExecuteValidateAdminEmailCommand()
        {
            return _adminEmail.Validate();
        }
        private bool ExecuteValidateCityNameCommand()
        {
            return _cityName.Validate();
        }

        private bool ExecuteValidateMobileNumberCommand()
        {
            return _mobilenumber.Validate();
        }

    }
}
