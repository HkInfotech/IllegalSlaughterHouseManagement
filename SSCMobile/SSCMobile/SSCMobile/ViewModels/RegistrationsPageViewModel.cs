using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobile.Validations;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSCMobile.ViewModels
{
    public class RegistrationsPageViewModel : ViewModelBase
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

        private List<CitysModel> _citiesList;

        public List<CitysModel> CityList
        {
            get { return _citiesList; }
            set { SetProperty(ref _citiesList, value); }
        }


        private ValidatableObject<string> _userConfrmPassword;

        public ValidatableObject<string> ConfirmPassword
        {
            get
            {
                return _userConfrmPassword;
            }
            set
            {
                _userConfrmPassword = value;
                RaisePropertyChanged(() => ConfirmPassword);
            }
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
        private ValidatableObject<bool> _isPrivacyPolicyCheck;

        public ValidatableObject<bool> IsPrivacyPolicyCheck
        {
            get { return _isPrivacyPolicyCheck; }
            set
            {
                _isPrivacyPolicyCheck = value;
                RaisePropertyChanged(() => IsPrivacyPolicyCheck);
            }
        }

        #endregion Properties

        #region Commands

        private DelegateCommand _registerCommand;

        public DelegateCommand RegisterCommand =>
            _registerCommand ?? (_registerCommand = new DelegateCommand(async () => await ExecuteRegisterCommandAsync()));


        private DelegateCommand _termandconditionclick;

        public DelegateCommand TermsandContionClick =>
            _termandconditionclick ?? (_termandconditionclick = new DelegateCommand(async () => await ExecuteTermsandContionClickAsync()));

        private async Task ExecuteTermsandContionClickAsync()
        {
        
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("BrowserUrl", AppPages.TermsAndCondtionURL);
            await NavigationService.NavigateAsync(AppPages.CommonBrowserPage, navigationParameters);
        }

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

        private DelegateCommand _validatePasswordCommand;

        public DelegateCommand ValidatePasswordCommand =>
            _validatePasswordCommand ?? (_validatePasswordCommand = new DelegateCommand(() => ExecuteValidateUserPasswordCommand()));

        private DelegateCommand _validateConfirmPasswordCommand;

        public DelegateCommand ValidateConfirmPasswordCommand =>
            _validateConfirmPasswordCommand ?? (_validateConfirmPasswordCommand = new DelegateCommand(() => ExecuteValidateConfirmPasswordCommand()));
        private DelegateCommand _onPrivacyPolicyChangeCommand;

        public DelegateCommand OnPrivacyPolicyChangeCommand =>
            _onPrivacyPolicyChangeCommand ?? (_onPrivacyPolicyChangeCommand = new DelegateCommand(() => PrivacyPolicyChangeCommandExecute()));


        #endregion Commands

        #region Constructor

        public RegistrationsPageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IAccountService accountService, IComplaintService complaintService) : base(navigationservice, settings, pageDialogService)
        {
            UserEmail = new ValidatableObject<string>();
            UserPassword = new ValidatableObject<string>();
            ConfirmPassword = new ValidatableObject<string>();
            UserMobile = new ValidatableObject<string>();
            UserCity = new ValidatableObject<string>();
            UserName = new ValidatableObject<string>();
            IsPrivacyPolicyCheck = new ValidatableObject<bool>();
            IsPrivacyPolicyCheck.Value = false;
            UserCity.Value = "---Select city---";
            _accountService = accountService;
            _userDialogs = userDialogs;
            _complaintService = complaintService;
            CurrentCity = new CitysModel();
            AddValidations();
        }

        #endregion Constructor

        #region Methods

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            await GetCity();
        }

        public async Task GetCity()
        {
            CityList=await _complaintService.GetCities("");
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

            _userCity.Validations.Add(new CityValidate<string>
            {
                ValidationMessage = "City is required"
            });

            _userConfrmPassword.Validations.Add(new CompareRule<string>() { CompareFunction = () => _userPassword.Value, ValidationMessage = "Confirm password is not match" });
        }

        private async Task ExecuteRegisterCommandAsync()
        {
            try
            {
                ValidateFields();
                if ((_userCity.IsValid == true && _userName.IsValid == true) && (_userEmail.IsValid == true && _userMobile.IsValid == true) && (_userPassword.IsValid == true && _userConfrmPassword.IsValid == true))
                {
                    _userDialogs.ShowLoading("Loading", MaskType.Gradient);
                    var responce = await _accountService.Register(_userName.Value, _userEmail.Value, _userCity.Value, _userMobile.Value, _userPassword.Value, _userConfrmPassword.Value, CurrentCity.Id);
                    _userDialogs.HideLoading();
                    if (!responce.Success)
                    {
                        if (responce.Message.Contains("is already taken")) 
                        {
                            await PageDialogService.DisplayAlertAsync(null, "This email address is already registered with us. Try to login with an existing account.", "Ok");
                        }
                        else 
                        {
                            await PageDialogService.DisplayAlertAsync(null, responce.Message, "Ok");
                        }
                        
                    }
                    else
                    {
                        await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.NavigationPage}/{AppPages.Login.ThankYouPage}", System.UriKind.Absolute));
                    }
                }
            }
            catch (Exception ex)
            {
                
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
                Debug.WriteLine($"REGISTRATION : {ex.Message}");
            }
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
            ValidatePasswordCommand.Execute();
            ValidateConfirmPasswordCommand.Execute();
            CityList = new List<CitysModel>();
        }

        private bool ExecuteValidateUserNameCommand()
        {
            return _userName.Validate();
        }

        private bool ExecuteValidateUserPasswordCommand()
        {
            return _userPassword.Validate();
        }

        private bool ExecuteValidateUserMobileCommand()
        {
            return _userMobile.Validate();
        }

        private bool ExecuteValidateUserEmailCommand()
        {
            return _userEmail.Validate();
        }

        private bool ExecuteValidateConfirmPasswordCommand()
        {
            return _userConfrmPassword.Validate();
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

            //var buttons = new List<IActionSheetButton>
            //{
            //    ActionSheetButton.CreateButton("---Select city---", WriteLine,"---Select city---"),
            //    ActionSheetButton.CreateCancelButton("Cancel", WriteLine, "Cancel")
            //};

            //foreach (var item in CityList)
            //{
            //    buttons.Add(ActionSheetButton.CreateButton(item.CityName, WriteLine, item.CityName));
            //}
            //await PageDialogService.DisplayActionSheetAsync(null, buttons.ToArray());
            //if (!(UserCity.Value == "---Select city---"))
            //{
            //    return _userCity.Validate();
            //}
            //else
            //{
            //    return _userCity.Validate();
            //}
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
        private void PrivacyPolicyChangeCommandExecute()
        {
            if (IsPrivacyPolicyCheck.Value)
            {
                IsPrivacyPolicyCheck.Value = false;
            }
            else
            {
                IsPrivacyPolicyCheck.Value = true;
            }
        }
        #endregion Methods
    }
}