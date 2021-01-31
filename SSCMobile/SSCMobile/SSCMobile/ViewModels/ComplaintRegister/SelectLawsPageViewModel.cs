using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels.ComplaintRegister
{
    public class SelectLawsPageViewModel : ViewModelBase
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

        private List<int> _saveLowsList;

        public List<int> SaveLowList
        {
            get { return _saveLowsList; }
            set { SetProperty(ref _saveLowsList, value); }
        }

        #endregion Properties

        #region command

        private DelegateCommand _selectLowSubmitCommand;

        public DelegateCommand SelectLowCommandExecute =>
           _selectLowSubmitCommand ?? (_selectLowSubmitCommand = new DelegateCommand(async () => await ExecuteSelectLowCommand()));

        #endregion command

        public SelectLawsPageViewModel(IUserDialogs userDialogs, IComplaintService complaintService, IAppSettings appSettings, IPageDialogService pageDialogService, INavigationService navigationService) : base(navigationService, appSettings, pageDialogService)
        {
            _complaintService = complaintService;
            ComplaintModelObj = new ComplaintModel();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            ComplaintModelObj = await _complaintService.GetNonComplaintModel();
            IsBusy = true;
            IsEmpty = false;
            await GetLows();
            IsBusy = false;
        }

        public override async void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        #region Method

        private async Task GetLows()
        {
            List<LowsModel> listLows = new List<LowsModel>();
            List<LowsModel> UpdateLowModel = new List<LowsModel>();

            listLows = await _complaintService.GetAllLows(_settings.UserId, _settings.UserCityId);

            if (listLows.Count > 0)
            {

                SaveLowList = new List<int>();
                if (!string.IsNullOrEmpty(ComplaintModelObj.LowsId))
                {
                    List<int> SplitLows = new List<int>();
                    SplitLows = ComplaintModelObj.LowsId.Split(',')?.Select(Int32.Parse).ToList();
                    foreach (var item in listLows)
                    {
                        if (SplitLows.AnyExtended())
                        {
                            if (SplitLows.Contains(item.ExternalId))
                            {
                                item.IsCheck = true;
                            }
                            else
                            {
                                item.IsCheck = false;
                            }
                            UpdateLowModel.Add(item);
                        }
                        else
                        {
                            UpdateLowModel.Add(item);
                        }
                    }
                    LowsList = UpdateLowModel;
                }
                else
                {
                    LowsList = listLows;
                }
            }
            else
            {
                IsBusy = false;
                IsEmpty = true;
            }
            IsBusy = false;
        }

        private async Task ExecuteSelectLowCommand()
        {
            try
            {

                var selectedLowList = LowsList;
                var IsLowsCheck = LowsList.Where(a => a.IsCheck == true)?.ToList();
                if (IsLowsCheck.AnyExtended())
                {
                    List<string> lows = new List<string>();
                    foreach (var item in IsLowsCheck)
                    {
                        if (item.IsCheck)
                        {
                            lows.Add(Convert.ToString(item.ExternalId));
                        }
                    }
                    ComplaintModelObj.LowsId = string.Join(",", lows.ToArray());

                    await _complaintService.SaveComplaint(ComplaintModelObj);
                    await NavigationService.NavigateAsync(AppPages.ComplaintRegister.ComplaintOverviewPage);
                }
                else
                {
                    await PageDialogService.DisplayAlertAsync(null, "Please check laws, which you find being violated", "OK");
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion Method
    }
}