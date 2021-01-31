using Prism.Navigation;
using Prism.Services;
using SSCMobile.ViewModels.Base;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using Xamarin.Essentials;

namespace SSCMobile.ViewModels
{
    public class ViewModelBase : ExtendedBindableObject, IInitialize, INavigationAware, IDestructible
    {
        #region Services

        protected  INavigationService NavigationService { get; private set; }

        protected  IAppSettings _settings;
       

        protected IPageDialogService PageDialogService { get; private set; }

        #endregion Services

        #region Properties

        private bool _IsEnable;

        public bool IsEnable
        {
            get { return _IsEnable; }
            set { SetProperty(ref _IsEnable, value); }
        }

        private ComplaintStatusEnum _isComplaintStatus = ComplaintStatusEnum.Pending;

        public ComplaintStatusEnum IsComplaintStatus
        {
            get { return _isComplaintStatus; }
            set { SetProperty(ref _isComplaintStatus, value); }
        }
        private ComplaintModel complaintModelObj;
        public ComplaintModel ComplaintModelObj
        {
            get { return complaintModelObj; }
            set { SetProperty(ref complaintModelObj, value); }
        }
        //private string _title;
        //public string Title
        //{
        //    get { return _title; }
        //    set { SetProperty(ref _title, value); }
        //}

        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            set { SetProperty(ref _isConnected, value); }
        }
        private string _emptyStateTitle;

        public string EmptyStateTitle
        {
            get { return _emptyStateTitle; }
            set { SetProperty(ref _emptyStateTitle, value); }
        }

        private string _emptyStateSubtitle;

        public string EmptyStateSubtitle
        {
            get { return _emptyStateSubtitle; }
            set { SetProperty(ref _emptyStateSubtitle, value); }
        }
        private bool isEmpty;

        public bool IsEmpty
        {
            get { return isEmpty; }
            set { SetProperty(ref isEmpty, value); }
        }
        #endregion Properties

        #region Constructor

        public ViewModelBase(INavigationService navigationService, IAppSettings settings, IPageDialogService pageDialogService)
        {
            NavigationService = navigationService;
            _settings = settings;
            PageDialogService = pageDialogService;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
            _settings.IsOnline = IsConnected;
            IsEmpty = false;
            EmptyStateSubtitle = "";
            EmptyStateSubtitle = "";
        }

        public ViewModelBase() : base()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            IsEmpty = false;
            EmptyStateSubtitle = "";
            EmptyStateSubtitle = "";
        }




        ~ViewModelBase()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        public ViewModelBase(INavigationService navigationservice)
        {
            NavigationService = navigationservice;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            IsEmpty = false;
            EmptyStateSubtitle = "";
            EmptyStateSubtitle = "";
        }

        #endregion Constructor

        #region Methods

        public virtual async void Initialize(INavigationParameters parameters)
        {
        }

        public virtual async void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual async void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public virtual async void Destroy()
        {
        }

        public bool IsController() 
        {
            var Controller = EnumExtensionMethods.GetEnumDescription(UserRole.Controller);
            var Volunteer = EnumExtensionMethods.GetEnumDescription(UserRole.Volunteer);
            var Admin = EnumExtensionMethods.GetEnumDescription(UserRole.Admin);
            var SiteAdmin = EnumExtensionMethods.GetEnumDescription(UserRole.SiteAdmin);

            if (_settings.UserRole.Equals(Controller)|| _settings.UserRole.Equals(Admin) || _settings.UserRole.Equals(SiteAdmin))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool IsAdmin()
        {
 
            var Admin = EnumExtensionMethods.GetEnumDescription(UserRole.Admin);
            var SiteAdmin = EnumExtensionMethods.GetEnumDescription(UserRole.SiteAdmin);

            if (_settings.UserRole.Equals(Admin) || _settings.UserRole.Equals(SiteAdmin))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool IsSiteAdmin()
        {

            var Admin = EnumExtensionMethods.GetEnumDescription(UserRole.SiteAdmin);
            if (_settings.UserRole.Equals(Admin))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsConnected = e.NetworkAccess == NetworkAccess.Internet;
            _settings.IsOnline = IsConnected;
        }

        public void Dispose()
        {
            Destroy();
        }

        #endregion Methods
    }
}