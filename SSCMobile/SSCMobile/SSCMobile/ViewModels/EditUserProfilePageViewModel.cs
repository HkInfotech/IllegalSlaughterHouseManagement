using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
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
    public class EditUserProfilePageViewModel : ViewModelBase
    {

        #region Services

        private IAccountService _accountService;
        private IUserDialogs _userDialogs;
        private IComplaintService _complaintService;


        #endregion Services

        #region Properties

        private CitysModel _currentCity;

        public CitysModel CurrentCity
        {
            get { return _currentCity; }
            set { SetProperty(ref _currentCity, value); }
        }

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

        #region Commands
        private DelegateCommand _editUserprofileCommand;

        public DelegateCommand UpdateUserCommadExecute =>
            _editUserprofileCommand ?? (_editUserprofileCommand = new DelegateCommand(async () => await ExecuteEditUserPropeCommandAsync()));



        private DelegateCommand _validateUserNameCommand;

        public DelegateCommand ValidateUserNameCommand =>
            _validateUserNameCommand ?? (_validateUserNameCommand = new DelegateCommand(() => ExecuteValidateUserNameCommand()));

        private DelegateCommand _validateUserEmailCommand;

        public DelegateCommand ValidateUserEmail =>
            _validateUserEmailCommand ?? (_validateUserEmailCommand = new DelegateCommand(() => ExecuteValidateUserEmailCommand()));

        private DelegateCommand _validateUserMobileCommand;

        public DelegateCommand ValidateUserMobile =>
            _validateUserMobileCommand ?? (_validateUserMobileCommand = new DelegateCommand(() => ExecuteValidateUserMobileCommand()));

        private DelegateCommand _validateUserCityCommand;

        public DelegateCommand ValidateUserCity =>
            _validateUserCityCommand ?? (_validateUserCityCommand = new DelegateCommand(async () => await ExecuteValidateUserCityCommand()));



        #endregion
        public EditUserProfilePageViewModel(INavigationService navigationService, IAppSettings settings, IPageDialogService pageDialogService, IComplaintService complaintService, IAccountService accountService, IUserDialogs userDialogs) : base(navigationService, settings, pageDialogService)
        {
            UserEmail = new ValidatableObject<string>();
            UserName = new ValidatableObject<string>();
            UserCity = new ValidatableObject<string>();
            UserMobile = new ValidatableObject<string>();
            UserEmail.Value = _settings.Email;
            UserName.Value = _settings.Name;
            UserCity.Value = _settings.UserCity;
            UserMobile.Value = _settings.PhoneNumber;
            _complaintService = complaintService;
            _accountService = accountService;
            _userDialogs = userDialogs;
            AddValidations();
            CurrentCity = new CitysModel();
        }
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            await GetCity();
        }
        public async Task GetCity()
        {
            CityList = await _complaintService.GetCities("");
        }

        private void AddValidations()
        {
            _userName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "User name is required"
            });
            _userEmail.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Email is required"
            });
            _userEmail.Validations.Add(new EmailRule<string>
            {
                ValidationMessage = "Enter valid email address"
            });

            _userMobile.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Mobile number is required"
            });

            _userMobile.Validations.Add(new MobilenumberRule<string>
            {
                ValidationMessage = "Enter valid mobile number"
            });







            _userCity.Validations.Add(new CityValidate<string>
            {
                ValidationMessage = "City is required"
            });


        }
        public void ValidateFields()
        {
            if ((UserCity.Value == "---Select city---"))
            {
                _userCity.Validate();
            }
            ValidateUserNameCommand.Execute();
            ValidateUserMobile.Execute();
            ValidateUserEmail.Execute();

            CityList = new List<CitysModel>();
        }
        private bool ExecuteValidateUserNameCommand()
        {
            return _userName.Validate();
        }

        private bool ExecuteValidateUserMobileCommand()
        {
            return _userMobile.Validate();
        }
        private bool ExecuteValidateUserEmailCommand()
        {
            return _userEmail.Validate();
        }


        private async Task<bool> ExecuteValidateUserCityCommand()
        {
            var buttons = new List<IActionSheetButton>
            {
                ActionSheetButton.CreateButton("---Select city---", ()=>{
                    UserCity.Value="---Select city---";
                }),
                ActionSheetButton.CreateCancelButton("Cancel", ()=>
                {
                     UserCity.Value="---Select city---";
                })
            };

            foreach (var item in CityList)
            {
                buttons.Add(ActionSheetButton.CreateButton(item.CityName, () =>
                {
                    UserCity.Value = item.CityName;
                    CurrentCity = item;
                }));
            }
            await PageDialogService.DisplayActionSheetAsync(null, buttons.ToArray());
            if (!(UserCity.Value == "---Select city---"))
            {
                return _userCity.Validate();
            }
            else
            {
                return _userCity.Validate();
            }
        }

        private void WriteLine(string message)
        {
            if (message == "Cancel")
            {
            }
            else
            {
                UserCity.Value = message;
            }
        }
        private async Task ExecuteEditUserPropeCommandAsync()
        {
            try
            {


                ValidateFields();
                if (_settings.IsOnline)
                {
                    if ((_userName.IsValid == true && _userMobile.IsValid == true))
                    {
                        IsBusy = true;
                        var responce = await _accountService.UpdateUserInfo(_userName.Value, _settings.Email, _settings.UserCity, _userMobile.Value, _settings.UserId, _settings.UserCityId);
                        IsBusy = false;
                        if (responce != null)
                        {

                            if (responce.Success)
                            {
                               // _settings.Email = responce.ResponeContent.Email;
                                _settings.Name = responce.ResponeContent.Name;
                                _settings.UserCity = responce.ResponeContent.City;
                                _settings.PhoneNumber = responce.ResponeContent.PhoneNumber;
                                //var CityModel = await _complaintService.GetCityByName(responce.ResponeContent.City);
                                 _settings.UserCityId = CurrentCity.ExternalId;
                                await PageDialogService.DisplayAlertAsync(null, "User details are updated successfully.", "Ok");
                             
                                await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.UserProfilePage}", System.UriKind.Absolute));
                            }
                        }
                        else
                        {
                            await PageDialogService.DisplayAlertAsync(null, "Sorry, there seems some technical error in edit your profile details. Please try again later.", "OK");
                        }
                    }

                }
                else
                {
                    IsBusy = false;
                    _userDialogs.Toast("There is no INTERNET connection", new TimeSpan(20));
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
            }
        }


    }




}
