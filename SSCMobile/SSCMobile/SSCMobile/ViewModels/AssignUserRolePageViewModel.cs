using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels
{
    public class AssignUserRolePageViewModel : ViewModelBase
    {
        private readonly IAccountService _accountService;
        private IUserDialogs _userDialogs;

        private AppUsersDTO selectedResult = new AppUsersDTO();
        public AppUsersDTO SelectedResult
        {
            get { return selectedResult; }
            set { SetProperty(ref selectedResult, value); }
        }


        private AppRolesModel selectedAppRole = new AppRolesModel();
        public AppRolesModel SelectedAppRole
        {
            get { return selectedAppRole; }
            set { SetProperty(ref selectedAppRole, value); }
        }


        private List<AppRolesModel> appRolesResult = new List<AppRolesModel>();
        public List<AppRolesModel> AppRolesResult
        {
            get { return appRolesResult; }
            set { SetProperty(ref appRolesResult, value); }
        }

        private DelegateCommand _selectRoleCommand;

        public DelegateCommand SelectRoleCommand =>
            _selectRoleCommand ?? (_selectRoleCommand = new DelegateCommand(async () => await SelectRoleCommandExecuteAsync()));

        private DelegateCommand _saveRoleCommand;

        public DelegateCommand SaveRoleCommand =>
            _saveRoleCommand ?? (_saveRoleCommand = new DelegateCommand(async () => await SaveRoleCommandExecuteAsync()));



        public AssignUserRolePageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IAccountService accountService) : base(navigationservice, settings, pageDialogService)
        {
            _accountService = accountService;
            SelectedResult = new AppUsersDTO();
            AppRolesResult = new List<AppRolesModel>();
            _userDialogs = userDialogs;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            SelectedResult = parameters.GetValue<AppUsersDTO>("SelectedAppUser") ?? new AppUsersDTO();
            IsBusy = true;
            await GetAppRoles();
            IsBusy = false;
        }

        public async Task GetAppRoles()
        {
            try
            {

            }
            catch (Exception) 
            {
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
                IsBusy = false;
            }
            AppRolesResult = await _accountService.GetRoles();

            AppRolesResult = AppRolesResult.Where(a => a.Name.ToLower()!= "site admin")?.ToList();
            SelectedAppRole = AppRolesResult?.Where(a => a.ExtId.Equals(SelectedResult.RoleId))?.FirstOrDefault();
        }
        private async Task SelectRoleCommandExecuteAsync()
        {

            List<IActionSheetButton> buttons = new List<IActionSheetButton>();
            foreach (var item in AppRolesResult)
            {
                buttons.Add(ActionSheetButton.CreateButton(item.Name, new Action(() =>
                {
                    SelectedAppRole = item;
                })));

            }
            await PageDialogService.DisplayActionSheetAsync("Select Option", buttons.ToArray());
        }

        private async Task SaveRoleCommandExecuteAsync()
        {
            try
            {
                if (_settings.IsLogin)
                {
                    var VerifiedResponce = await _userDialogs.ConfirmAsync("Are you sure you want to change user role ", null, "Yes", "No", null);
                    if (VerifiedResponce == true)
                    {
                        IsBusy = true;
                        var result = await _accountService.UpdateUserRole(SelectedResult.Id, SelectedAppRole.ExtId);
                        if (result)
                        {
                            IsBusy = false;
                            await PageDialogService.DisplayAlertAsync(null, "User role change successfully.", "Ok");
                            await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.SSCMaster.SSCMasterPage}/{AppPages.NavigationPage}/{AppPages.DashBoard.SetUserRolePage}", System.UriKind.Absolute));
                        }
                        IsBusy = false;

                    }

                }

            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
                IsBusy = false;
            }
        }
    }
}
