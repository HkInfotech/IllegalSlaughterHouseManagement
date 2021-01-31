using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Models;
using SSCMobile.Services.Interfaces;
using SSCMobile.Validations;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Services

        private IAccountService _accountService;

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

        #endregion Properties

        #region Commands

        private DelegateCommand _loginCommand;

        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(async () => await ExecuteLoginCommandAsync()));

        private DelegateCommand _validateUserNameCommand;

        public DelegateCommand ValidateUserNameCommand =>
            _validateUserNameCommand ?? (_validateUserNameCommand = new DelegateCommand(() => ExecuteValidateUserNameCommand()));

        #endregion Commands

        #region Constructor

        public MainPageViewModel(INavigationService navigationService, IAppSettings settings, IPageDialogService pageDialogService, IAccountService accountServic)
            : base(navigationService, settings, pageDialogService)
        {
            _accountService = accountServic;
            _userName = new ValidatableObject<string>();
            AddValidations();
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
                ValidationMessage = "User name is required"
            });
        }

        private async Task ExecuteLoginCommandAsync()
        {
            LoginRequest loginRequest = new LoginRequest();
            await _accountService.RequestToken(loginRequest);
        }

        private bool ExecuteValidateUserNameCommand()
        {
            return _userName.Validate();
        }

        #endregion Methods
    }
}