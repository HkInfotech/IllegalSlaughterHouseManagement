using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobile.Validations;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class AuthMainPageViewModel : ViewModelBase
    {
        #region Services

        private readonly IAccountService _accountService;

        #endregion Services

        #region Properties

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

        private DelegateCommand _onPrivacyPolicyChangeCommand;

        public DelegateCommand OnPrivacyPolicyChangeCommand =>
            _onPrivacyPolicyChangeCommand ?? (_onPrivacyPolicyChangeCommand = new DelegateCommand(() => PrivacyPolicyChangeCommandExecute()));

        private DelegateCommand _onLoginClickCommand;

        public DelegateCommand OnLoginClickCommand =>
            _onLoginClickCommand ?? (_onLoginClickCommand = new DelegateCommand(async () => await OnLoginClickCommandExecute()));

        private DelegateCommand _onRegisterClickCommand;

        public DelegateCommand OnRegisterClickCommand =>
            _onRegisterClickCommand ?? (_onRegisterClickCommand = new DelegateCommand(async () => await OnRegisterClickCommandExecute()));

        #endregion Commands

        #region Constructor

        public AuthMainPageViewModel(INavigationService NavigationService, IAppSettings setting, IAccountService accountService, IPageDialogService pageDialogService) : base(NavigationService, setting, pageDialogService)
        {
            _accountService = accountService;
            IsPrivacyPolicyCheck = new ValidatableObject<bool>();
            IsPrivacyPolicyCheck.Value = false;
        }

        #endregion Constructor

        #region Methods

        private async Task OnLoginClickCommandExecute()
        {
            await NavigationService.NavigateAsync(AppPages.Login.LoginPage);
        }

        private async Task OnRegisterClickCommandExecute()
        {
            await NavigationService.NavigateAsync(AppPages.Login.RegistrationsPage);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsPrivacyPolicyCheck = new ValidatableObject<bool>();
            IsPrivacyPolicyCheck.Value = false;
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