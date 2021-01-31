using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class CityAdminPageViewModel : ViewModelBase
    {

        private readonly IComplaintService _complaintService;


        private CityDTO selectedCity;

        public CityDTO SelectedCityDTO
        {
            get { return selectedCity; }
            set { SetProperty(ref selectedCity, value); }
        }


        private List<CityDTO> _cityList = new List<CityDTO>();

        public List<CityDTO> CityList
        {
            get { return _cityList; }
            set { SetProperty(ref _cityList, value); }
        }

        private DelegateCommand _selectedItemCommand;

        public DelegateCommand SelectedItemCommand =>
            _selectedItemCommand ?? (_selectedItemCommand = new DelegateCommand(() => SelectedItemCommandExecute()));

        private DelegateCommand _addCommand;

        public DelegateCommand AddCommand =>
            _addCommand ?? (_addCommand = new DelegateCommand(async () =>
            {
                await NavigationService.NavigateAsync(AppPages.DashBoard.CreateCityPage);
            }));

        async private void SelectedItemCommandExecute()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("City", SelectedCityDTO);

            await NavigationService.NavigateAsync(AppPages.DashBoard.SaveCityPage, parameters);
        }

        public CityAdminPageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IComplaintService complaintService) : base(navigationservice, settings, pageDialogService)
        {
            _complaintService = complaintService;
            SelectedCityDTO = new CityDTO();
        }
        async public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsBusy = true;
            await GetCitys();
            IsBusy = false;
        }

        async private Task GetCitys()
        {
            try
            {
                CityList = new List<CityDTO>();
                CityList = await _complaintService.GetCitiesFromServer(_settings.UserId);

            }
            catch (Exception)
            {

            }
        }
    }
}
