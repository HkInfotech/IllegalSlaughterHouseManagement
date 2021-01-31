using SSCMobile.ViewModels.ComplaintRegister;
using Xamarin.Forms;

namespace SSCMobile.Views.ComplaintRegister
{
    public partial class ComplaintOverviewPage : ContentPage
    {

        ComplaintOverviewPageViewModel vm;
        public ComplaintOverviewPage()
        {
            InitializeComponent();
            vm = (ComplaintOverviewPageViewModel)BindingContext;
        }

        private void HkFileLayout_FileClicked(object sender, SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO.ComplaintImagesDTO e)
        {
            vm.ImageTappedClickCommand?.Execute(e);
        }
    }
}