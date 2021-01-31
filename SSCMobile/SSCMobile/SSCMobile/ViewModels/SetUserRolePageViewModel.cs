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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class SetUserRolePageViewModel : ViewModelBase
    {


        #region Properties
        public int CurrentPage { get; set; } = 1;

        private ObservableCollection<AppUsersDTO> tempResult = new ObservableCollection<AppUsersDTO>();
        public ObservableCollection<AppUsersDTO> TempResult
        {
            get { return tempResult; }
            set { SetProperty(ref tempResult, value); }
        }

        private ObservableCollection<AppUsersDTO> searchResults = new ObservableCollection<AppUsersDTO>();
        public ObservableCollection<AppUsersDTO> SearchResults
        {
            get { return searchResults; }
            set { SetProperty(ref searchResults, value); }
        }
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { SetProperty(ref searchText, value); }
        }

        private AppUsersDTO selectedResult;
        public AppUsersDTO SelectedResult
        {
            get { return selectedResult; }
            set { SetProperty(ref selectedResult, value); }
        }

        private bool isCommonSearching;
        public bool IsCommonSearching
        {
            get { return isCommonSearching; }
            set { SetProperty(ref isCommonSearching, value); }
        }


        private bool isUpToDate;
        public bool IsUpToDate
        {
            get { return isUpToDate; }
            set { SetProperty(ref isUpToDate, value); }
        }

        #endregion
        private DelegateCommand itemSelected;
        public DelegateCommand ItemSelectedCommand => itemSelected ?? (itemSelected = new DelegateCommand(OnItemSelectedCommandExecuted));

        private DelegateCommand search;
        public DelegateCommand SearchCommand => search ?? (search = new DelegateCommand(OnSearchCommandExecuted));


        private DelegateCommand<object> loadMore;
        public DelegateCommand<object> LoadMoreCommand => loadMore ?? (loadMore = new DelegateCommand<object>(OnLoadMoreCommandExecuted));

        #region Commands

        #endregion
        private readonly IAccountService _accountService;
        public SetUserRolePageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IAccountService accountService) : base(navigationservice, settings, pageDialogService)
        {
            _accountService = accountService;
            SearchResults = new ObservableCollection<AppUsersDTO>();
            TempResult = new ObservableCollection<AppUsersDTO>();
        }

        public SetUserRolePageViewModel() : base()
        {

        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsBusy = true;
            
            await GetUsers();
            await _accountService.GetRoles();
            IsBusy = false;

        }
        public async Task GetUsers()
        {
            try
            {
                if (_settings.IsOnline) 
                {
                    var GetUsers = await _accountService.GetUsersListByCity();
                    SearchResults = new ObservableCollection<AppUsersDTO>(GetUsers?.OrderBy(a => a.Email).ToList());
                     tempResult = new ObservableCollection<AppUsersDTO>(GetUsers?.OrderBy(a => a.Email).ToList());
                    if (!GetUsers.AnyExtended())
                    {
                        IsEmpty = true;
                        IsBusy = false;
                        EmptyStateTitle = AppAlertMessage.NoRecordFound;
                    }
                   
                }
                else
                {
                    IsEmpty = true;
                    IsBusy = false;
                    EmptyStateTitle = AppAlertMessage.NoInternetConnections;

                }
                
                
            }
            catch (Exception ex)
            {
                IsBusy = false;
                IsEmpty = true;
                
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
            }
        }
        private async void OnItemSelectedCommandExecuted()
        {
            NavigationParameters navigationParameters = new NavigationParameters();
            if (!(SelectedResult.UserRole.ToLower()== "site admin")) 
            {
                navigationParameters.Add("SelectedAppUser", SelectedResult);
                await NavigationService.NavigateAsync(AppPages.DashBoard.AssignUserRolePage, navigationParameters);
            }
            else
            {
                await PageDialogService.DisplayAlertAsync(null, $"{SelectedResult.Name} Has site admin role. you have no access to change user role", "Ok");
            }
           
        }
        private async void OnSearchCommandExecuted()
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(SearchText)) 
            {
                var searchUser = tempResult.Where(a => a.Email.ToLower().Contains(SearchText.ToLower())).ToList();
                if (searchUser.Count > 0)
                {
                    searchResults.ReplaceRange(searchUser);

                }
                else
                {

                    SearchText = string.Empty;
                    searchResults.ReplaceRange(searchUser);
                   await PageDialogService.DisplayAlertAsync(null, "No Record Found", "Ok");
                }
            }

            
            
            IsBusy = false;

        }
        private async void OnLoadMoreCommandExecuted(object obj)
        {

        }

    }
}
