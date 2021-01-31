using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class ComplaintStatusPageViewModel : ViewModelBase
    {
        #region Services

        private IComplaintService _complaintService;
        private IUserDialogs _userDialogs;

        #endregion Services

        #region Properties

        private List<ComplaintsDTO> _complaintList;

        public List<ComplaintsDTO> ComplaintList
        {
            get
            {
                return _complaintList;
            }
            set
            {
                SetProperty(ref _complaintList, value);
            }
        }

        private ComplaintsDTO _selectedComplaint = new ComplaintsDTO();

        public ComplaintsDTO SelectedComplaint
        {
            get
            {
                return _selectedComplaint;
            }
            set
            {
                SetProperty(ref _selectedComplaint, value);
            }
        }

        private DelegateCommand<ComplaintsDTO> _onCompalinStatusClickCommand;

        public DelegateCommand<ComplaintsDTO> OnCompalinStatusClickCommand =>
          _onCompalinStatusClickCommand ?? (_onCompalinStatusClickCommand = new DelegateCommand<ComplaintsDTO>(async (a) => await CompalinStatusMethod(a)));

        private async Task CompalinStatusMethod(ComplaintsDTO complaints)
        {
            try
            {
                IsBusy = true;

                _complaintService.DeleteComplaint();
                if (_settings.IsOnline)
                {
                    IsBusy = true;

                    ComplaintModelObj = await _complaintService.GetNonComplaintModel() ?? new ComplaintModel();
                    if (ComplaintModelObj != null)
                    {
                        ComplaintModelObj.ShopAddress = complaints.ShopAddress ?? "";
                        ComplaintModelObj.ShopName = complaints.ShopName ?? "";

                        ComplaintModelObj.Files = complaints.ComplaintFiles ?? "";
                        if (!string.IsNullOrEmpty(ComplaintModelObj.Files))
                        {
                            List<string> SplitFilePaths = new List<string>();
                            List<string> AddFilePaths = new List<string>();
                            SplitFilePaths = ComplaintModelObj.Files.Split(',')?.ToList();
                            if (SplitFilePaths.AnyExtended())
                            {
                                foreach (var item in SplitFilePaths)
                                {
                                    var localFilePath = await FileExtensions.DownloadImageFromUrlSaveLocal(item);
                                    AddFilePaths.Add(localFilePath);
                                }
                            }
                            ComplaintModelObj.Files = string.Join(",", AddFilePaths.ToArray());
                            ComplaintModelObj.ComplaintFiles = ComplaintModelObj.Files;

                        }
                        else
                        {
                            ComplaintModelObj.Files = "";
                        }

                        ComplaintModelObj.ExternalId = complaints.Id;
                        ComplaintModelObj.Operation = (int)Operations.Non;
                        ComplaintModelObj.LowsId = complaints.ComplaintLowsId;
                        ComplaintModelObj.ComplaintLowsId = complaints.ComplaintLowsId;
                        ComplaintModelObj.ApplicationType = 1;
                        ComplaintModelObj.IsEmptyModel = false;
                        ComplaintModelObj.SpeciesName = complaints.SpeciesName;
                        ComplaintModelObj.SpeciesId = complaints.SpeciesId;
                        ComplaintModelObj.Violations = complaints.Violations;
                        ComplaintModelObj.Cityid = _settings.UserCityId;
                        ComplaintModelObj.ComplainStatus = complaints.ComplainStatus;
                        ComplaintModelObj.IsActive = true;
                        ComplaintModelObj.GpsLocations = complaints.GpsLocations;
                        ComplaintModelObj.IsDelete = false;
                        ComplaintModelObj.Comments = complaints.Comments;
                        ComplaintModelObj.DateOfInspection = complaints.DateOfInspection;
                        ComplaintModelObj.IsRejecet = complaints.IsRejecet ?? false;
                        ComplaintModelObj.CommentForRejection = complaints.CommentForRejection;
                        ComplaintModelObj.RegistrationDate = complaints.RegistrationDate ?? DateTime.UtcNow;
                        ComplaintModelObj.IsRegister = complaints.IsRegister;
                        ComplaintModelObj.IsEmailSend = complaints.IsEmailSend;
                        ComplaintModelObj.CreatedBy = complaints.CreatedBy;
                        ComplaintModelObj.ModifiedBy = complaints.ModifiedBy;
                        ComplaintModelObj.CreatedDate = complaints.CreatedDate ?? DateTime.UtcNow;
                        ComplaintModelObj.ModifiedDate = complaints.ModifiedDate ?? DateTime.UtcNow;
                        ComplaintModelObj.CreatedUserName = complaints.CreatedUserName;
                        ComplaintModelObj.UserMobileNumber = complaints.UserMobileNumber;
                        ComplaintModelObj.ModifiedDate = complaints.ModifiedDate ?? DateTime.UtcNow;
                        ComplaintModelObj.CityName = complaints.CityName;
                        ComplaintModelObj.IsUpdate = false;
                        await _complaintService.SaveComplaint(ComplaintModelObj);


                        IsBusy = false;
                        NavigationParameters navigationparameters = new NavigationParameters();
                        navigationparameters.Add("complaintdto", complaints);
                        await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.DashBoard.ComplaintStatusPage}/{AppPages.ComplaintRegister.ComplaintOverviewPage}", System.UriKind.RelativeOrAbsolute), navigationparameters);

                    }
                    IsBusy = false;
                }
                else
                {
                    _userDialogs.Toast("There is no INTERNET connection", new TimeSpan(20));
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
                
            }
        }

        #endregion Properties

        public ComplaintStatusPageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IComplaintService complaintService) : base(navigationservice, settings, pageDialogService)
        {
            _complaintService = complaintService;
            _userDialogs = userDialogs;
            IsBusy = false;


        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {

            if (ComplaintList.AnyExtended())
            {
                IsBusy = false;
            }
            else
            {
                IsBusy = true;
            }
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            await GetComplaints();
        }

        public async Task GetComplaints()
        {

            IsBusy = true;
            try
            {

                if (_settings.IsLogin && _settings.IsOnline)
                {

                    var GetServerComplaintList = await _complaintService.GetComplaintsFromServer(0);
                    if (GetServerComplaintList.AnyExtended())
                    {
                        ComplaintList = GetServerComplaintList;
                        IsBusy = false;
                    }
                    else
                    {
                        IsBusy = false;
                        IsEmpty = true;
                        EmptyStateTitle = AppAlertMessage.NoComplaintFound;
                    }
                }
                else
                {
                    IsBusy = false;
                    IsEmpty = true;
                    EmptyStateTitle = AppAlertMessage.NoInternetConnections;

                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                IsEmpty = true;
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");

                //EmptyStateTitle = AppAlertMessage.NoComplaintFount;
            }

        }
    }
}