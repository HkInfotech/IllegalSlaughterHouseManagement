using SSCMobile.ViewModels.ComplaintRegister;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace SSCMobile.Views.ComplaintRegister
{
    public partial class ComplaintRegisterPage : ContentPage
    {
        ComplaintRegisterPageViewModel complaintRegisterPageViewModel;
        public ComplaintRegisterPage()
        {
            InitializeComponent();
            complaintRegisterPageViewModel =(ComplaintRegisterPageViewModel)BindingContext;
        }

        private void HkFileLayout_FileClicked(object sender, SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO.ComplaintImagesDTO e)
        {
            complaintRegisterPageViewModel.ImageTappedClickCommand?.Execute(e);
        }
    }
}