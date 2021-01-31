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
    public class MainAuthPageViewModel : ViewModelBase
    {
        #region Services

        private readonly IAccountService _accountService;

        #endregion Services

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

        public MainAuthPageViewModel(INavigationService NavigationService, IAppSettings setting, IPageDialogService pageDialogService) : base(NavigationService, setting, pageDialogService)
        {
            IsPrivacyPolicyCheck = new ValidatableObject<bool>();
            IsPrivacyPolicyCheck.Value = false;
        }

        #region Methods

        private async Task OnLoginClickCommandExecute()
        {
            await NavigationService.NavigateAsync(AppPages.Login.LoginPage);
        }

        private async Task OnRegisterClickCommandExecute()
        {
            await NavigationService.NavigateAsync(AppPages.Login.RegistrationsPage);
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