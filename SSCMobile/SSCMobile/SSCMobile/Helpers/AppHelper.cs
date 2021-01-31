namespace SSCMobile.Helpers
{
    public static class AppPages
    {
        public const string MainPage = "MainPage";
        public const string NavigationPage = "NavigationPage";
        public const string BaseUrl = "http://ssc.resquark.com/";
        public const string CommonBrowserPage = "CommonBrowserPage";
        public const string TermsAndCondtionURL = "http://ssc.resquark.com/home/Termsandcondition";

        internal static class Login
        {
            public const string LoginPage = "LoginPage";
            public const string RegistrationsPage = "RegistrationsPage";
            public const string MainAuthPage = "MainAuthPage";
            public const string ThankYouPage = "ThankYouPage";
            public const string ForgotPasswordPage = "ForgotPasswordPage";
            public const string ChangePasswordPage = "ChangePasswordPage";
            public const string ResetPasswordPage = "ResetPasswordPage";
        }

        internal static class SSCMaster
        {
            public const string SSCMasterPage = "SSCMasterPage";
        }

        internal static class DashBoard
        {
            public const string DashBoardTabbedPage = "DashBoardTabbedPage";
            public const string HomePage = "HomePage";
            public const string SearchPage = "SearchPage";
            public const string CameraPage = "CameraPage";
            public const string SettigPage = "SettingPage";
            public const string ComplaintStatusPage = "ComplaintStatusPage";
            public const string ChangePasswordPage = "ChangePasswordPage";
            public const string SavedImageGalleryPage = "SavedImageGalleryPage";
            public const string LowsPage = "LowsPage";
            public const string ConfirmComplaintPage = "ConfirmComplaintPage";

            public const string UserProfilePage = "UserProfilePage";
            public const string EditUserProfilePage = "EditUserProfilePage";
            public const string ComplaintLocationPage = "ComplaintLocationPage";
            public const string UpdateCityMail = "UpdateCityMail";

            public const string SetUserRolePage = "SetUserRolePage";
            public const string AssignUserRolePage = "AssignUserRolePage";
            public const string CityAdminPage = "CityAdminPage";
            public const string SaveCityPage = "SaveCityPage";
            public const string CreateCityPage = "CreateCityPage";
          
        }

        internal static class Walkthrough
        {
            public const string WalkthroughPage = "WalkthroughPage";
        }

        internal static class ComplaintRegister
        {
            public const string ComplaintRegisterPage = "ComplaintRegisterPage";
            public const string SelectLawsPage = "SelectLawsPage";
            public const string CompalintImageUploadPage = "CompalintImageUploadPage";
            public const string ComplaintOverviewPage = "ComplaintOverviewPage";
            public const string ImageViewPage = "ImageViewPage";
        }
    }

    public static class EndPoint
    {
        internal static class Login
        {
            public const string Token = "/Token";
        }
        public static class Account
        {
            public const string Register = "/api/Account/Register";
            public const string UserInfo = "/api/Account/UserInfo";
            public const string UpdateUserInfo = "/api/Account/UpdateUserInfo";
            public const string ChangePassword = "/api/Account/ChangePassword";
            public const string ForgotPassword = "/api/Account/ForgotPassword";
            public const string GetRoles = "/api/Account/GetRoles";
            public const string GetUsersList = "/api/Account/GetUsersList";
            public const string GetUsersListByCity = "/api/Account/GetUsersListByCity";
            public const string UpdateUserRole = "/api/Account/UpdateUserRole";
        }

        public static class Complaint
        {
            public const string GetCities = "/api/City/GetCities";
            public const string GetAllCities = "/api/City/GetAllCities";
            public const string GetAllLows = "/api/Low/GetAllLows";
            public const string GetCityLows = "/api/City/GetCitieLows";
            public const string GetLowsOfCity = "/api/Low/GetCityLows";
            public const string GetAllSpecies = "/api/Species/GetAllSpecies";
            public const string SaveComplaints = "/api/Complaint/SaveComplaint";
            public const string GetComplaints = "/api/Complaint/GetComplaints";
            public const string SendMail = "/api/Complaint/SendMail";
            public const string UpdateCityMail = "/api/City/UpdateCityMail";
            public const string UpsertCity = "/api/City/UpsertCity";
            public const string CreateCity = "/api/City/CreateCity";
            public const string UpdateCity = "/api/City/UpdateCity";
        }
    }

    public static class AppAlertMessage 
    {
        public const string NoRecordFound = "No Record Found";
        public const string NoComplaintFound = "There is no complaint to show";
        public const string NoInternetConnections = "There is no INTERNET connection";
        public const string TechnicalError = "Sorry, there seems some technical error . Please try again later.";
    }
}