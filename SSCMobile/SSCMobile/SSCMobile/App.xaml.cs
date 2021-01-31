using Acr.UserDialogs;
using DLToolkit.Forms.Controls;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.Settings.Abstractions;
using Prism;
using Prism.Ioc;
using SSCMobile.Helpers;
using SSCMobile.Services.Implementations;
using SSCMobile.Services.Interfaces;
using SSCMobile.ViewModels;
using SSCMobile.ViewModels.ComplaintRegister;
using SSCMobile.Views;
using SSCMobile.Views.ComplaintRegister;
using SSCMobile.Views.Dashboard;
using SSCMobile.Views.LoginViews;
using SSCMobile.Views.WalkthroughViews;
using SSCMobileDataBus.OfflineStore.Models;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using SSCMobileServiceBus.OfflineSync.Repository;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SSCMobile
{
    public partial class App
    {
        /*
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor.
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public const bool UsLocalService = false;
        public const string hostUrl = "http://ssc.resquark.com/";

        public const string LocalhostUrl = "http://192.168.225.32:57696";

        public App() : this(null)
        {
        }

        public App(IPlatformInitializer initializer) : base(initializer)
        {
          
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            FlowListView.Init();
            
#if DEBUG
            EnableDebugRainbows(false);
#endif
            await NavigationService.NavigateAsync($"{AppPages.NavigationPage}/{AppPages.Walkthrough.WalkthroughPage}");
        }

        protected override  void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>(AppPages.NavigationPage);
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>(AppPages.MainPage);

            //Login View
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>(AppPages.Login.LoginPage);
            containerRegistry.RegisterForNavigation<RegistrationsPage, RegistrationsPageViewModel>(AppPages.Login.RegistrationsPage);
            //containerRegistry.RegisterForNavigation<AuthMainPage, AuthMainPageViewModel>(AppPages.Login.AuthMainPage);
            containerRegistry.RegisterForNavigation<MainAuthPage, MainAuthPageViewModel>(AppPages.Login.MainAuthPage);
            containerRegistry.RegisterForNavigation<ForgotPasswordPage, ForgotPasswordPageViewModel>(AppPages.Login.ForgotPasswordPage);

            containerRegistry.RegisterForNavigation<ResetPasswordPage, ResetPasswordPageViewModel>();

            //Walk through View
            containerRegistry.RegisterForNavigation<WalkthroughPage, WalkthroughPageViewModel>(AppPages.Walkthrough.WalkthroughPage);

            //Master Detail Page
            containerRegistry.RegisterForNavigation<SSCMasterPage, SSCMasterPageViewModel>(AppPages.SSCMaster.SSCMasterPage);
            //TabbedPage
            containerRegistry.RegisterForNavigation<DashBoardTabbedPage, DashBoardTabbedPageViewModel>(AppPages.DashBoard.DashBoardTabbedPage);
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>(AppPages.DashBoard.HomePage);
            containerRegistry.RegisterForNavigation<SearchPage, SearchPageViewModel>(AppPages.DashBoard.SearchPage);
            containerRegistry.RegisterForNavigation<CameraPage, CameraPageViewModel>(AppPages.DashBoard.CameraPage);
            containerRegistry.RegisterForNavigation<SettingPage, SettingPageViewModel>(AppPages.DashBoard.SettigPage);
            containerRegistry.RegisterForNavigation<LowsPage, LowsPageViewModel>(AppPages.DashBoard.LowsPage);
            containerRegistry.RegisterForNavigation<ConfirmComplaintPage, ConfirmComplaintPageViewModel>(AppPages.DashBoard.ConfirmComplaintPage);
            containerRegistry.RegisterForNavigation<UserProfilePage, UserProfilePageViewModel>(AppPages.DashBoard.UserProfilePage);

            containerRegistry.RegisterForNavigation<SetUserRolePage, SetUserRolePageViewModel>(AppPages.DashBoard.SetUserRolePage);

            containerRegistry.RegisterForNavigation<AssignUserRolePage, AssignUserRolePageViewModel>(AppPages.DashBoard.AssignUserRolePage);

            containerRegistry.RegisterForNavigation<CityAdminPage, CityAdminPageViewModel>(AppPages.DashBoard.CityAdminPage);
            containerRegistry.RegisterForNavigation<SaveCityPage, SaveCityPageViewModel>(AppPages.DashBoard.SaveCityPage);
            containerRegistry.RegisterForNavigation<CreateCityPage, CreateCityPageViewModel>(AppPages.DashBoard.CreateCityPage);


            //ComplaintRegister
            containerRegistry.RegisterForNavigation<ComplaintRegisterPage, ComplaintRegisterPageViewModel>(AppPages.ComplaintRegister.ComplaintRegisterPage);
            containerRegistry.RegisterForNavigation<SelectLawsPage, SelectLawsPageViewModel>(AppPages.ComplaintRegister.SelectLawsPage);
            containerRegistry.RegisterForNavigation<CompalintImageUploadPage, CompalintImageUploadPageViewModel>(AppPages.ComplaintRegister.CompalintImageUploadPage);
            containerRegistry.RegisterForNavigation<ComplaintOverviewPage, ComplaintOverviewPageViewModel>(AppPages.ComplaintRegister.ComplaintOverviewPage);
            containerRegistry.RegisterForNavigation<ComplaintStatusPage, ComplaintStatusPageViewModel>(AppPages.DashBoard.ComplaintStatusPage);
            containerRegistry.RegisterForNavigation<SavedImageGalleryPage, SavedImageGalleryPageViewModel>(AppPages.DashBoard.SavedImageGalleryPage);
            containerRegistry.RegisterForNavigation<ChangePasswordPage, ChangePasswordPageViewModel>(AppPages.DashBoard.ChangePasswordPage);
            containerRegistry.RegisterForNavigation<ThankYouPage, ThankYouPageViewModel>(AppPages.Login.ThankYouPage);
            containerRegistry.RegisterForNavigation<ImageViewPage, ImageViewPageViewModel>(AppPages.ComplaintRegister.ImageViewPage);
            containerRegistry.RegisterForNavigation<UserProfilePage, UserProfilePageViewModel>(AppPages.DashBoard.UserProfilePage);
            containerRegistry.RegisterForNavigation<EditUserProfilePage, EditUserProfilePageViewModel>(AppPages.DashBoard.EditUserProfilePage);
            containerRegistry.RegisterForNavigation<ComplaintLocationPage, ComplaintLocationPageViewModel>(AppPages.DashBoard.ComplaintLocationPage);
            containerRegistry.RegisterForNavigation<UpdateCityMail, UpdateCityMailViewModel>(AppPages.DashBoard.UpdateCityMail);


            #region Register instance

            containerRegistry.RegisterInstance(typeof(IUserDialogs), UserDialogs.Instance);
            containerRegistry.RegisterInstance(typeof(IConnectivity), CrossConnectivity.Current);
            containerRegistry.RegisterInstance(typeof(ISettings), Plugin.Settings.CrossSettings.Current);
            containerRegistry.RegisterInstance<IAppSettings>(new AppSettings());

            #endregion Register instance
            #region Repository
            containerRegistry.Register<IRepository<CitysModel>, Repository<CitysModel>>();
            containerRegistry.Register<IRepository<SpeciesModel>, Repository<SpeciesModel>>();
            containerRegistry.Register<IRepository<LowsModel>, Repository<LowsModel>>();
            containerRegistry.Register<IRepository<CityLowsModel>, Repository<CityLowsModel>>(); 
            containerRegistry.Register<IRepository<ComplaintModel>, Repository<ComplaintModel>>(); 
            
            containerRegistry.Register<IRepository<UserProfile>, Repository<UserProfile>>();
            containerRegistry.Register<IRepository<AppRolesModel>, Repository<AppRolesModel>>();
            containerRegistry.Register<IRepository<AppUsersModel>, Repository<AppUsersModel>>();
       


            #endregion Repository


            #region Services

            containerRegistry.Register<IAccountService, AccountService>();
            containerRegistry.Register<IComplaintService, ComplaintService>();

            #endregion Services







            containerRegistry.RegisterForNavigation<CommonBrowserPage, CommonBrowserPageViewModel>(AppPages.CommonBrowserPage);


        }

        private void EnableDebugRainbows(bool shouldUseDebugRainbows)
        {
            Resources.Add(new Style(typeof(ContentPage))
            {
                ApplyToDerivedTypes = true,
                Setters = {
                new Setter
                {
                    Property = Xamarin.Forms.DebugRainbows.DebugRainbow.ShowColorsProperty,
                    Value = shouldUseDebugRainbows
                }
            }
            });
        }
    }
}