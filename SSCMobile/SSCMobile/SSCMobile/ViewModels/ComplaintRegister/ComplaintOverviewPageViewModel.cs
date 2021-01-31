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

namespace SSCMobile.ViewModels.ComplaintRegister
{
    public class ComplaintOverviewPageViewModel : ViewModelBase
    {
        #region Services

        private IUserDialogs _userDialogs;
        private IComplaintService _complaintService;

        #endregion Services

        #region Properties

        private List<LowsModel> _lowsList;

        public List<LowsModel> LowsList
        {
            get
            {
                return _lowsList;
            }
            set
            {
                SetProperty(ref _lowsList, value);
            }
        }
        

        private ComplaintModel _complaint = new ComplaintModel();

        public ComplaintModel Complaint
        {
            get
            {
                return _complaint;
            }
            set
            {
                SetProperty(ref _complaint, value);
            }
        }


        private ObservableCollection<ComplaintImagesDTO> _complaintImagesDTOs;

        public ObservableCollection<ComplaintImagesDTO> ComplaintImagesDTOs
        {
            get
            {
                return _complaintImagesDTOs;
            }
            set
            {
                SetProperty(ref _complaintImagesDTOs, value);
            }
        }

        private bool _hasFiles;

        public bool HasFiles
        {
            get
            {
                return _hasFiles;
            }
            set
            {
                SetProperty(ref _hasFiles, value);
            }
        }

    

        private int _listViewHeight;

        public int ListViewHeight
        {
            get
            {
                return _listViewHeight;
            }
            set
            {
                SetProperty(ref _listViewHeight, value);
            }
        }

        private bool _showRejectionView = false;

        public bool ShowRejectionView
        {
            get
            {
                return _showRejectionView;
            }
            set
            {
                SetProperty(ref _showRejectionView, value);

            }
        }

        private bool _showbuttonscollection = true;

        public bool Showbuttonscollection
        {
            get
            {
                return _showbuttonscollection;
            }
            set
            {
                SetProperty(ref _showbuttonscollection, value);

            }
        }






        private bool _showSubmitButton;

        public bool ShowSubmitButton
        {
            get
            {
                return _showSubmitButton;
            }
            set
            {
                SetProperty(ref _showSubmitButton, value);


            }
        }

        private string _rejectTextArea = "";

        public string RejectTextArea
        {
            get
            {
                return _rejectTextArea;
            }
            set
            {
                SetProperty(ref _rejectTextArea, value);
            }
        }


        #endregion Properties

        #region Command
        private DelegateCommand _editcommand;

        public DelegateCommand EditCommand =>
            _editcommand ?? (_editcommand = new DelegateCommand(async () => await ExecuteEditCommandAsync()));

        private DelegateCommand _verifiedCommand;

        public DelegateCommand VerifiedCommand =>
            _verifiedCommand ?? (_verifiedCommand = new DelegateCommand(async () => await VerifiedComplaints()));


        private DelegateCommand _rejectCommand;

        public DelegateCommand RejectCommand =>
            _rejectCommand ?? (_rejectCommand = new DelegateCommand(async () => await RejectComplaints()));

        private DelegateCommand _confirmButton;

        public DelegateCommand ConfirmCommand =>
            _confirmButton ?? (_confirmButton = new DelegateCommand(async () => await ConfirmCommandExecute()));

        private DelegateCommand _cancelcommand;

        public DelegateCommand CancelCommand =>
            _cancelcommand ?? (_cancelcommand = new DelegateCommand(async () => await CancelCommandExecute()));

        

        private DelegateCommand _submitCommand;

        public DelegateCommand SubmitCommand =>
            _submitCommand ?? (_submitCommand = new DelegateCommand(async () => await ExecuteSubmitCommandAsync()));

        private DelegateCommand _registerCommand;

        public DelegateCommand RegisterCommand =>
            _registerCommand ?? (_registerCommand = new DelegateCommand(async () => await RegisterCommandExecuteasync()));

        private DelegateCommand _viewComplaint;

        public DelegateCommand ViewComplaint =>
            _viewComplaint ?? (_viewComplaint = new DelegateCommand(async () => await ViewComplaintCommandExecuteasync()));

       

        private DelegateCommand _savecomplaintCommad;

        public DelegateCommand SaveComplaintCommand =>
            _savecomplaintCommad ?? (_savecomplaintCommad = new DelegateCommand(async () => await SaveComplaintCommandExecute()));

        private DelegateCommand<ComplaintImagesDTO> _imageTappedClickCommand;

        public DelegateCommand<ComplaintImagesDTO> ImageTappedClickCommand =>
            _imageTappedClickCommand ?? (_imageTappedClickCommand = new DelegateCommand<ComplaintImagesDTO>(async (a) => await ExecuteImageTappedClickCommandAsync(a)));

        private async Task ExecuteEditCommandAsync()
        {
            ComplaintModelObj = await _complaintService.GetNonComplaintModel();
            ComplaintModelObj.IsUpdate = true;
            await _complaintService.SaveComplaint(ComplaintModelObj);

            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.ComplaintRegister.ComplaintRegisterPage}/", System.UriKind.RelativeOrAbsolute));
        }


        #endregion

        public ComplaintOverviewPageViewModel(IUserDialogs userDialogs, IComplaintService complaintService, IAppSettings appSettings, IPageDialogService pageDialogService, INavigationService navigationService) : base(navigationService, appSettings, pageDialogService)
        {
            _complaintService = complaintService;
            LowsList = new List<LowsModel>();
            ComplaintImagesDTOs = new ObservableCollection<ComplaintImagesDTO>();
            Complaint = _complaintService.GetNonComplaint();
            ComplaintModelObj = _complaintService.GetNonComplaint();
            _userDialogs = userDialogs;
            IsBusy = false;
            
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
         
            var complainttemp = _complaintService.GetNonComplaint();
            Complaint = _complaintService.GetNonComplaint();
            ComplaintModelObj = _complaintService.GetNonComplaint();
            ListViewHeight = 10;
            if (!string.IsNullOrEmpty(complainttemp.LowsId))
            {
                var SplitLows = complainttemp.LowsId.Split(',')?.Select(Int32.Parse).ToList();

                foreach (var item in SplitLows)
                {
                    ListViewHeight = ListViewHeight + 60;
                }
                
            }
            else
            {
                 ListViewHeight = 50;
            }

            


        }
        public async override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            IsBusy = true;
            await SetComplaintOverviewData();
            IsBusy = false;
        }

        public async Task SetComplaintOverviewData() 
        {
            try
            {
                List<LowsModel> listLows = new List<LowsModel>();
                listLows = await _complaintService.GetAllLows(_settings.UserId, _settings.UserCityId);
                RejectTextArea = ComplaintModelObj.CommentForRejection;
                Complaint = _complaintService.GetNonComplaint();
                IsBusy = true;
                ComplaintModelObj = await _complaintService.GetNonComplaintModel();
                var Controller = EnumExtensionMethods.GetEnumDescription(UserRole.Controller);
                var Admin = EnumExtensionMethods.GetEnumDescription(UserRole.Admin);

                if (ComplaintModelObj != null)
                {
                    if (!string.IsNullOrEmpty(ComplaintModelObj.LowsId))
                    {
                        List<int> SplitLows = new List<int>();
                        SplitLows = ComplaintModelObj.LowsId.Split(',')?.Select(Int32.Parse).ToList();
                        int i = 1;
                        foreach (var item in listLows)
                        {
                            if (SplitLows.AnyExtended())
                            {
                                if (SplitLows.Contains(item.ExternalId))
                                {
                                    item.LowsTitile = item.LowsTitile;
                                    item.NumberText = Convert.ToString(i);
                                    LowsList.Add(item);
                                    i++;
                                }
                            }
                        }
                    }

                    ComplaintImagesDTOs = new ObservableCollection<ComplaintImagesDTO>();
                    if (!string.IsNullOrEmpty(ComplaintModelObj.Files))
                    {
                        List<string> SplitFilePaths = new List<string>();

                        SplitFilePaths = ComplaintModelObj.Files.Split(',')?.ToList();
                        if (SplitFilePaths.AnyExtended())
                        {
                            foreach (var item in SplitFilePaths)
                            {

                                ComplaintImagesDTOs.Add(new ComplaintImagesDTO()
                                {
                                    ComplaintId = 0,
                                    FileType = "Image",
                                    FileImage = item
                                });
                            }
                            CheckHashFile();
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void CheckHashFile()
        {
            if (ComplaintImagesDTOs.AnyExtended())
            {
                HasFiles = true;
            }
            else
            {
                HasFiles = false;
            }
        }
        private async Task ExecuteSubmitCommandAsync()
        {
            try
            {
                if (_settings.IsOnline == true)
                {

                    IsBusy = true;

                    if (ComplaintModelObj.ComplainStatus == (int)ComplaintStatusEnum.Registered)
                    {

                        NavigationParameters keyValuePairs = new NavigationParameters();
                        
                        keyValuePairs.Add("ExternalId", Complaint.ExternalId);
                        keyValuePairs.Add("IsEmailSend", Complaint.IsEmailSend);
                       
                        await NavigationService.NavigateAsync(AppPages.DashBoard.ConfirmComplaintPage, keyValuePairs);
                        return;
                    }

                    switch ((ComplaintStatusEnum)ComplaintModelObj.ComplainStatus)
                    {
                        case ComplaintStatusEnum.Non:
                        case ComplaintStatusEnum.Pending:
                            {
                                ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Pending;
                                break;
                            }
                        case ComplaintStatusEnum.Verified:
                            {
                                if (ComplaintModelObj.CreatedBy.Equals(_settings.UserId))
                                {
                                    ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Registered;
                                    ComplaintModelObj.IsRegister = true;
                                    ComplaintModelObj.RegistrationDate = DateTime.UtcNow;
                                    ComplaintModelObj.IsRejecet = false;
                                }
                                else
                                {
                                    ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Verified;
                                    ComplaintModelObj.IsRejecet = false;
                                }
                                break;
                            }
                        case ComplaintStatusEnum.Rejected:
                            {
                                ComplaintModelObj.IsRejecet = true;
                                ComplaintModelObj.CommentForRejection = RejectTextArea;
                                if (IsController()) 
                                {
                                ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Rejected;

                                }
                                else 
                                {
                                ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Pending;

                                }
                                break;
                            }
                    }
                    ComplaintModelObj.CommentForRejection = RejectTextArea;
                    await _complaintService.SaveComplaint(ComplaintModelObj);
                    ComplaintModelObj = await _complaintService.GetNonComplaintModel();
                    var responce = await _complaintService.SaveOnServer(ComplaintModelObj);
                    if (responce == true)
                    {
                        IsBusy = false;
                        ComplaintModelObj.Operation = (int)Operations.Update;
                        ComplaintModelObj.IsEmailSend = false;
                        await _complaintService.SaveComplaint(ComplaintModelObj);
                        if (ComplaintModelObj.ComplainStatus == (int)ComplaintStatusEnum.Registered)
                        {
                            NavigationParameters keyValuePairs = new NavigationParameters();
                         
                          
                            keyValuePairs.Add("ExternalId", Complaint.ExternalId);
                            keyValuePairs.Add("IsEmailSend", Complaint.IsEmailSend);
                            await NavigationService.NavigateAsync(AppPages.DashBoard.ConfirmComplaintPage, keyValuePairs);
                            IsBusy = false;
                            return;
                        }
                        else
                        {
                            IsBusy = false;
                            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.DashBoard.ComplaintStatusPage}", System.UriKind.RelativeOrAbsolute));
                        }


                    }
                    IsBusy = false;

                }
                else
                {

                    IsBusy = false;
                    _userDialogs.Toast("There is no INTERNET connection", new TimeSpan(20));
                }

                
            }
            catch (Exception ex)
            {

                IsBusy = false;
            }


        }


        private async Task RejectComplaints()
        {
            ShowRejectionView = true;
            Showbuttonscollection = false;
            

        }
        private async Task VerifiedComplaints()
        {
            try
            {
                if (_settings.IsOnline) 
                {
                    var VerifiedResponce = await _userDialogs.ConfirmAsync("Are you sure you want to verify this complaint? ", null, "Yes", "No", null);

                    if (VerifiedResponce)
                    {
                        IsBusy = true;
                        ComplaintModelObj = await _complaintService.GetNonComplaintModel();
                        ComplaintModelObj.ComplainStatus =(int) ComplaintStatusEnum.Verified;
                        ComplaintModelObj.IsRejecet = false;
                        ComplaintModelObj.CommentForRejection = " ";
                        ComplaintModelObj.IsRegister = false;
                        var responce = await _complaintService.SaveOnServer(ComplaintModelObj);

                        if (responce == true)
                        {
                            IsBusy = false;
                            ComplaintModelObj.Operation = (int)Operations.Update;
                            ComplaintModelObj.IsEmailSend = false;
                            await _complaintService.SaveComplaint(ComplaintModelObj);
                            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.DashBoard.ComplaintStatusPage}", System.UriKind.RelativeOrAbsolute));
                        }
                        IsBusy = false;
                    }
                    else
                    {
                        IsBusy = false;
                    }
                }
                else
                {
                    IsBusy = false;
                    _userDialogs.Toast("There is no INTERNET connection", new TimeSpan(20));
                }
                
            }
            catch (Exception ex) 
            {
                IsBusy = false;
            }
            

        }

        private async Task ConfirmCommandExecute()
        {
            try
            {
                if (_settings.IsOnline == true)
                {
                    var VerifiedResponce = await _userDialogs.ConfirmAsync("Are you sure you want to save this complaint?", null, "Yes", "No",null);
                    if (VerifiedResponce==true) 
                    {
                        IsBusy = true;
                        ComplaintModelObj = await _complaintService.GetNonComplaintModel();

                        if (Complaint.ComplainStatus == (int)ComplaintStatusEnum.Non)
                        {
                            ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Pending;
                        }
                        if (Complaint.IsUpdate==true && Complaint.ComplainStatus == (int)ComplaintStatusEnum.Rejected) 
                        {
                            ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Pending;
                        }

                        //if (Complaint.ComplainStatus == (int)ComplaintStatusEnum.Rejected)
                        //{
                        //    ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Pending;
                        //    ComplaintModelObj.IsRejecet = false;
                        //    ComplaintModelObj.CommentForRejection = " ";
                        //}
                       await _complaintService.SaveComplaint(ComplaintModelObj);
                        var responce = await _complaintService.SaveOnServer(ComplaintModelObj);

                        if (responce == true)
                        {
                            IsBusy = false;
                            ComplaintModelObj.Operation = (int)Operations.Update;
                            ComplaintModelObj.IsEmailSend = false;
                            await _complaintService.SaveComplaint(ComplaintModelObj);
                            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.DashBoard.ComplaintStatusPage}", System.UriKind.RelativeOrAbsolute));
                        }
                        IsBusy = false;
                    }
                    else
                    {
                        IsBusy = true;
                    }
                    IsBusy = false;

                }
            }
            catch (Exception ex) 
            {
                IsBusy = false;
                _userDialogs.Toast("There is no INTERNET connection", new TimeSpan(20));
            }
           
        }
        private async Task CancelCommandExecute()
        {
            RejectTextArea = "";
            ShowRejectionView = false;
            Showbuttonscollection = true;
            
            
        }

        private async Task SaveComplaintCommandExecute()
        {
            try
            {
                if (string.IsNullOrEmpty(RejectTextArea)) 
                {
                    await PageDialogService.DisplayAlertAsync(null, "Rejection Reason is mandatory", "Ok", null);
                    return;
                }
                if (_settings.IsOnline == true)
                {

                    var VerifiedResponce = await _userDialogs.ConfirmAsync("Are you sure you want to reject this complaint?", null, "Yes", "No", null);
                    if (VerifiedResponce==true) 
                    {
                        IsBusy = true;
                        ComplaintModelObj = await _complaintService.GetNonComplaintModel();
                        ComplaintModelObj.IsRejecet = true;
                        ComplaintModelObj.CommentForRejection = RejectTextArea;
                        ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Rejected;
                        var responce = await _complaintService.SaveOnServer(ComplaintModelObj);

                        if (responce == true)
                        {
                            IsBusy = false;
                            ComplaintModelObj.Operation = (int)Operations.Update;
                            ComplaintModelObj.IsEmailSend = false;
                            await _complaintService.SaveComplaint(ComplaintModelObj);
                            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.DashBoard.ComplaintStatusPage}", System.UriKind.RelativeOrAbsolute));
                        }
                        IsBusy = false;
                    }
                    else
                    {
                        IsBusy = false;

                    }
                    
                }
                else
                {
                    IsBusy = false;
                    _userDialogs.Toast("There is no INTERNET connection", new TimeSpan(20));
                }
                IsBusy = false;
            }
            catch (Exception ex) 
            {
                IsBusy = false;
            }
        }

        private async Task RegisterCommandExecuteasync()
        {
            try
            {
                if (_settings.IsOnline)
                {
                    var VerifiedResponce = await _userDialogs.ConfirmAsync("Are you sure you want to register this complaint to authority?", null, "Yes", "No", null);

                    if (VerifiedResponce)
                    {
                        IsBusy = true;
                        ComplaintModelObj = await _complaintService.GetNonComplaintModel();
                        //ComplaintModelObj.ComplainStatus = (int)ComplaintStatusEnum.Registered;
                        ComplaintModelObj.IsRejecet = false;
                        ComplaintModelObj.CommentForRejection = " ";
                        ComplaintModelObj.RegistrationDate = DateTime.Now;
                        ComplaintModelObj.IsRegister = true;
                        var responce = await _complaintService.SaveOnServer(ComplaintModelObj);

                        if (responce == true)
                        {
                            IsBusy = false;
                            ComplaintModelObj.Operation = (int)Operations.Update;
                            ComplaintModelObj.IsEmailSend = false;
                            await _complaintService.SaveComplaint(ComplaintModelObj);

                            NavigationParameters navigationParameters = new NavigationParameters();
                            
                            navigationParameters.Add("ExternalId", Complaint.ExternalId);
                            navigationParameters.Add("IsEmailSend", Complaint.IsEmailSend);
                            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.DashBoard.ComplaintStatusPage}/{AppPages.DashBoard.ConfirmComplaintPage}", System.UriKind.RelativeOrAbsolute), navigationParameters);
                        }
                        IsBusy = false;
                    }
                    else
                    {
                        IsBusy = false;
                    }
                }
                else
                {
                    IsBusy = false;
                    _userDialogs.Toast("There is no INTERNET connection", new TimeSpan(20));
                }

                IsBusy = false;

            }
            catch (Exception ex)
            {
                IsBusy = false;
            }

        }

        private async Task ViewComplaintCommandExecuteasync()
        {
            NavigationParameters navigationParameters = new NavigationParameters();
            navigationParameters.Add("ExternalId", Complaint.ExternalId);
            navigationParameters.Add("IsEmailSend", Complaint.IsEmailSend);
            
            navigationParameters.Add("complaint", Complaint.IsEmailSend);
            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.DashBoard.ComplaintStatusPage}/{AppPages.DashBoard.ConfirmComplaintPage}", System.UriKind.RelativeOrAbsolute), navigationParameters);
        }
        private async Task EditComplaints()
        {
            //var Controller = EnumExtensionMethods.GetEnumDescription(UserRole.Controller);
            //var Volunteer = EnumExtensionMethods.GetEnumDescription(UserRole.Volunteer);
            ComplaintModelObj = await _complaintService.GetNonComplaintModel();         
            ComplaintModelObj.IsUpdate = true;
            await _complaintService.SaveComplaint(ComplaintModelObj);
            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.HomePage}/{AppPages.ComplaintRegister.ComplaintRegisterPage}", System.UriKind.Absolute));
        }
        private async Task ViewFile(ComplaintImagesDTO complaintImagesDTO)
        {
            var Navigationparameters = new NavigationParameters();
            
            Navigationparameters.Add("ComplaintImageView", complaintImagesDTO);
            await NavigationService.NavigateAsync(AppPages.ComplaintRegister.ImageViewPage, Navigationparameters);
        }

        private async Task ExecuteImageTappedClickCommandAsync(ComplaintImagesDTO complaintImagesDTO)
        {
            await ViewFile(complaintImagesDTO);
        }
    }
}