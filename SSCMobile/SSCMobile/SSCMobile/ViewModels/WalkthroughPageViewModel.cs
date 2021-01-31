using ImTools;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class WalkthroughPageViewModel : ViewModelBase
    {
        #region Properties

        private DelegateCommand onSkipCommandClick;

        public DelegateCommand OnSkipCommandClick =>
            onSkipCommandClick ?? (onSkipCommandClick = new DelegateCommand(async () => await ExecuteOnSkipCommandClickAsync()));

        private bool CanExecuteOnSkipCommandClick()
        {
            return true;
        }

        private List<string> cardimages;

        public List<string> CardImages
        {
            get { return cardimages; }
            set
            {
                cardimages = value;
                RaisePropertyChanged(() => cardimages);
            }
        }

        #endregion Properties

        #region Constructor

        public WalkthroughPageViewModel(INavigationService navigationService, IAppSettings settings, IPageDialogService pageDialogService) : base(navigationService, settings, pageDialogService)
        {
            CardImages = new List<string>()
            {
               "bg2.jpg", "bg2.jpg", "bg2.jpg"
            };
        }

        #endregion Constructor

        #region Methods

        private async Task ExecuteOnSkipCommandClickAsync()
        {
            if (_settings.IsLogin == false && string.IsNullOrEmpty(_settings.Token))
            {
                await NavigationService.NavigateAsync(AppPages.Login.MainAuthPage);
            }
            else
            {
                await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}", System.UriKind.Absolute));
            }
        }

        #endregion Methods
    }
}