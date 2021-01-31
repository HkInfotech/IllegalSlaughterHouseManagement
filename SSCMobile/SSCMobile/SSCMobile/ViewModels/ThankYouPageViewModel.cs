using Prism.Commands;
using Prism.Navigation;
using SSCMobile.Helpers;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class ThankYouPageViewModel : ViewModelBase
    {
        private DelegateCommand _loginCommand;

        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(async () => await ExecuteLoginCommandAsync()));

        private async Task ExecuteLoginCommandAsync()
        {
            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.NavigationPage}/{AppPages.Walkthrough.WalkthroughPage}", System.UriKind.Absolute));
        }

        public ThankYouPageViewModel(INavigationService navigationservice) : base(navigationservice)
        {
        }

        public async override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }
    }
}