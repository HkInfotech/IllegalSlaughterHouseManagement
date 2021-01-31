using Acr.UserDialogs;
using PanCardView.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Models;
using SSCMobile.Services.Implementations;
using SSCMobile.Services.Interfaces;
using SSCMobile.Views.Dashboard;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SSCMobile.ViewModels
{
    public class SSCMasterPageViewModel : ViewModelBase
    {
        private IComplaintService _complaintService;
        private IAccountService _accountService;
        private IUserDialogs _userDialogs;
        public ObservableCollection<MyMenuItem> MenuItems { get; set; }

        private string _userName = "";

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _userPhoneNumber = "";

        public string UserPhoneNumber
        {
            get { return _userPhoneNumber; }
            set { SetProperty(ref _userPhoneNumber, value); }
        }

        private string _userEmail = "";

        public string UserEmail
        {
            get { return _userEmail; }
            set { SetProperty(ref _userEmail, value); }
        }

        private MyMenuItem selectedMenuItem;

        public MyMenuItem SelectedMenuItem
        {
            get => selectedMenuItem;
            set => SetProperty(ref selectedMenuItem, value);
        }
        private DelegateCommand _navigateCommand;

        public DelegateCommand NavigateCommand =>
            _navigateCommand ?? (_navigateCommand = new DelegateCommand(async () => await Navigate()));

        private DelegateCommand _navigateUserprofile;

        public DelegateCommand NavigateUserProfile =>
            _navigateUserprofile ?? (_navigateUserprofile = new DelegateCommand(async () => await NavigateUserProfileMethod()));

        private async Task NavigateUserProfileMethod()
        {
            await NavigationService.NavigateAsync(AppPages.NavigationPage + "/" + AppPages.DashBoard.UserProfilePage);
        }

        public SSCMasterPageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IComplaintService complaintService) : base(navigationservice, settings, pageDialogService)
        {
            _complaintService = complaintService;
            _userDialogs = userDialogs;
            UserEmail = _settings.Email;
            UserName = _settings.Name;
            _accountService = new AccountService();
            MenuItems = new ObservableCollection<MyMenuItem>();
            MenuItems.Add(new MyMenuItem()
            {
                Icon = IconFonts.Home,
                PageName = AppPages.DashBoard.HomePage,
                Title = "Home",
                IsNavigate = true,
                SubTitle = "Dashboard"
            });

            MenuItems.Add(new MyMenuItem()
            {
                Icon = IconFonts.User,
                PageName = AppPages.DashBoard.UserProfilePage,
                Title = "Profile",
                IsNavigate = true,
                SubTitle = "User profile"
            });
            MenuItems.Add(new MyMenuItem()
            {
                Icon = IconFonts.FileAlt,
                PageName = AppPages.DashBoard.ComplaintStatusPage,
                Title = "Complaint Status ",
                IsNavigate = true,
                SubTitle = "View Complaints"

            });

            MenuItems.Add(new MyMenuItem()
            {
                Icon = IconFonts.Info,
                PageName = "KnowFiapo",
                Title = "About Us",
                IsNavigate = false,
                SubTitle = "Know about FIAPO"
            });

            MenuItems.Add(new MyMenuItem()
            {
                Icon = IconFonts.HandsHelping,
                PageName = "JoinFIAPO",
                Title = "Join FIAPO",
                IsNavigate = false,
                SubTitle = "Become a FIAPO Activist"
            });

            MenuItems.Add(new MyMenuItem()
            {
                Icon = IconFonts.Donate,
                PageName = "Donate",
                Title = "Donate",
                IsNavigate = false,
                SubTitle = "Donate to the Cause"
            });
            //MenuItems.Add(new MyMenuItem()
            //{
            //    Icon = IconFonts.MapPin,
            //    PageName = AppPages.DashBoard.ComplaintLocationPage,
            //    Title = "Location",
            //    IsNavigate = true,
            //    SubTitle = "Location"
            //});
            //MenuItems.Add(new MyMenuItem()
            //{
            //    Icon = "",
            //    PageName = AppPages.DashBoard.HomePage,
            //    Title = "Refer App"
            //});
            //MenuItems.Add(new MyMenuItem()
            //{
            //    Icon = "",
            //    PageName = nameof(DashBoardTabbedPage),
            //    Title = "Help"
            //});
            //MenuItems.Add(new MyMenuItem()
            //{
            //    Icon = IconFonts.Key,
            //    PageName = AppPages.DashBoard.ChangePasswordPage,
            //    Title = "Change Password"
            //});
            if (IsAdmin() || IsSiteAdmin())
            {
                MenuItems.Add(new MyMenuItem()
                {
                    Icon = IconFonts.UsersCog,
                    PageName = AppPages.DashBoard.SetUserRolePage,
                    IsNavigate = true,
                    Title = "Set User Role",
                    SubTitle = "User Role"
                });

                MenuItems.Add(new MyMenuItem()
                {
                    Icon = IconFonts.Cogs,
                    PageName = AppPages.DashBoard.UpdateCityMail,
                    IsNavigate = true,
                    Title = "Settings",
                    SubTitle = "Settings"
                });


            }
            if (IsSiteAdmin())
            {
                MenuItems.Add(new MyMenuItem()
                {
                    Icon = IconFonts.City,
                    PageName = AppPages.DashBoard.CityAdminPage,
                    IsNavigate = true,
                    Title = "City Admin",
                    SubTitle = "Cities"
                });
            }
            //MenuItems.Add(new MyMenuItem()
            //{
            //    Icon = IconFonts.Key,
            //    PageName = AppPages.DashBoard.ChangePasswordPage,
            //    IsNavigate = true,
            //    Title = "Change password",
            //    SubTitle = "Change password"
            //});
            MenuItems.Add(new MyMenuItem()
            {
                Icon = IconFonts.SignOutAlt,
                PageName = "IsLogout",
                Title = "Log out",
                SubTitle = "Log out"
            });


        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            IsBusy = false;
        }

        private async Task Navigate()
        {
            if (SelectedMenuItem.IsNavigate)
            {
                IsBusy = true;

                //if (AppPages.DashBoard.ChangePasswordPage == SelectedMenuItem.PageName)
                //{
                //    IsBusy = false;
                //    var responce = await _accountService.ForgotPassword(_settings.Email.Trim());
                //    if (responce) 
                //    {
                //        await NavigationService.NavigateAsync(AppPages.NavigationPage + "/" + SelectedMenuItem.PageName);
                //    }
                //}
                //else
                //{
                //    IsBusy = false;
                //    await NavigationService.NavigateAsync(AppPages.NavigationPage + "/" + SelectedMenuItem.PageName);
                //}
              
                await NavigationService.NavigateAsync(AppPages.NavigationPage + "/" + SelectedMenuItem.PageName);
                IsBusy = false;
            }
            else
            {

                if (SelectedMenuItem.PageName == "Share")
                {

                }
                else if (SelectedMenuItem.PageName == "IsLogout")
                {
                    var result = await _userDialogs.ConfirmAsync("Are you sure you want to logout?", "Log Out", "Ok", "Cancel", null);
                    if (result == true)
                    {
                        _settings.IsLogin = false;
                        _settings.IsLogin2 = false;
                        _complaintService.DropTabels();
                        _settings.Token = "";
                        await NavigationService.NavigateAsync(new System.Uri($"/{AppPages.NavigationPage}/{AppPages.Walkthrough.WalkthroughPage}", System.UriKind.Absolute));
                    }


                }
                else if (SelectedMenuItem.PageName == "Donate")
                {
                    await Browser.OpenAsync(" http://www.fiapo.org/donation/", BrowserLaunchMode.SystemPreferred);

                }
                else if (SelectedMenuItem.PageName == "KnowFiapo")
                {


                    await Browser.OpenAsync("http://www.fiapo.org/fiaporg/about-us/", BrowserLaunchMode.SystemPreferred);
                }
                else if (SelectedMenuItem.PageName == "JoinFIAPO")
                {


                    await Browser.OpenAsync(" http://www.fiapo.org/fiaporg/volunteer/", BrowserLaunchMode.SystemPreferred);
                }
            }
        }


    }
}