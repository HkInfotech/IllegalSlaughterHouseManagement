using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
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

namespace SSCMobile.ViewModels
{
    public class LowsPageViewModel : ViewModelBase
    {
        #region Services

        private IUserDialogs _userDialogs;
        private IComplaintService _complaintService;

        #endregion Services
        private ObservableCollection<LowsModel> _lowsList;
        public ObservableCollection<LowsModel> LowsList
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
        public LowsPageViewModel(IUserDialogs userDialogs, IComplaintService complaintService, IAppSettings appSettings, IPageDialogService pageDialogService, INavigationService navigationService) : base(navigationService, appSettings, pageDialogService)
        {
            LowsList = new ObservableCollection<LowsModel>();
            _complaintService = complaintService;

            IsBusy = true;
            IsBusy = false;
        }
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {

            IsBusy = true;
            await GetLows();
            IsBusy = false;
        }

         public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
           
        }

        async public Task GetLows()
        {
            try
            {

                IsBusy = true;
                var listLows = await _complaintService.GetAllLows(_settings.UserId, _settings.UserCityId);
                if (listLows.AnyExtended())
                {
                    IsEmpty = false;
                    IsBusy = false;
                    int i = 1;
                    foreach (var item in listLows)
                    {
                        item.LowsTitile = item.LowsTitile;
                        item.NumberText = Convert.ToString(i);
                        item.LowsDescriptions = item.LowsDescriptions;
                        LowsList.Add(item);
                        i++;
                    }
                    IsBusy = false;
                }
                else
                {
                    IsEmpty = true;
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                IsEmpty = true;
                IsBusy = false;
            }
        }
    }

}
