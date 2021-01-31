using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private int _isActive;
        private IComplaintService _complaintService;
        public int IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        private DelegateCommand _followuponPreviousComplaintCommnad;

        public DelegateCommand FollowuponPreviousComplaintCommnad =>
          _followuponPreviousComplaintCommnad ?? (_followuponPreviousComplaintCommnad = new DelegateCommand(async () => await FollowuponPreviousComplaintMethod()));

        private DelegateCommand _lowsViewCommnad;

        public DelegateCommand LowsViewCommand =>
          _lowsViewCommnad ?? (_lowsViewCommnad = new DelegateCommand(async () => await LowsViewCommandMethod()));





        private DelegateCommand _onComplainRegisterClickCommnad;

        public DelegateCommand OnComplainRegisterClickCommnad =>
          _onComplainRegisterClickCommnad ?? (_onComplainRegisterClickCommnad = new DelegateCommand(async () => await ComplainRegisterClickCommand()));

        private async Task ComplainRegisterClickCommand()
        {
            _complaintService.DeleteComplaint();
            ComplaintModelObj = await _complaintService.GetNonComplaintModel() ?? new ComplaintModel();
            ComplaintModelObj.ShopAddress = "";
            ComplaintModelObj.ShopName = "";
            ComplaintModelObj.Files = "";
            ComplaintModelObj.ExternalId = 0;
            ComplaintModelObj.Operation = (int)Operations.Non;
            ComplaintModelObj.LowsId = "";
            ComplaintModelObj.ApplicationType = 1;
            ComplaintModelObj.IsEmptyModel = false;
            ComplaintModelObj.SpeciesName = "";
            ComplaintModelObj.SpeciesId = 0;
            ComplaintModelObj.Violations = "";
            ComplaintModelObj.Cityid = _settings.UserCityId;
            ComplaintModelObj.IsActive = true;
            ComplaintModelObj.IsDelete = false;
            ComplaintModelObj.Comments = "";
            ComplaintModelObj.DateOfInspection = DateTime.Now;
            ComplaintModelObj.LowsId = "";
            ComplaintModelObj.CreatedBy = _settings.UserId;
            ComplaintModelObj.ModifiedBy = _settings.UserId;
            ComplaintModelObj.CreatedDate = DateTime.Now;
            ComplaintModelObj.ModifiedDate = DateTime.Now;
            ComplaintModelObj.Files = "";
            ComplaintModelObj.IsEmailSend = false;
            ComplaintModelObj.ComplainStatus = 1;
            ComplaintModelObj.IsRejecet = false;
            ComplaintModelObj.CommentForRejection = "";
            ComplaintModelObj.IsRegister = false;
            ComplaintModelObj.CreatedUserName = _settings.Name;
            ComplaintModelObj.UserMobileNumber = _settings.PhoneNumber;
            ComplaintModelObj.CityName = _settings.UserCity;
            ComplaintModelObj.IsUpdate = true;

            await _complaintService.SaveComplaint(ComplaintModelObj);
            await NavigationService.NavigateAsync(AppPages.ComplaintRegister.ComplaintRegisterPage);
        }

        public HomePageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IComplaintService complaintService) : base(navigationservice, settings, pageDialogService)
        {
            _complaintService = complaintService;
        }

        public event EventHandler IsActiveChanged;
        private async Task LowsViewCommandMethod()
        {

            await NavigationService.NavigateAsync(AppPages.DashBoard.LowsPage);
        }
        private async Task FollowuponPreviousComplaintMethod()
        {
            await NavigationService.NavigateAsync(AppPages.DashBoard.ComplaintStatusPage);
        }
    }
}