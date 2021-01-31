using Newtonsoft.Json;
using SSCMobile.Helpers;
using SSCMobile.Models;
using SSCMobile.Models.Common;
using SSCMobile.Services.Interfaces;

using SSCMobileDataBus.OfflineStore.Models;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using SSCMobileServiceBus.OfflineSync.Repository;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using SSCMobileServiceBus.Platform_Specific_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSCMobile.Services.Implementations
{
    public class AccountService : IAccountService
    {
        #region Services

        private readonly IRepository<UserProfile> _userProfileRepo;
        private readonly IRepository<AppRolesModel> _appRolesRepo;
        private readonly IRepository<AppUsersModel> _appUserRepo;

        private RestApiHelpers restapiHelper;

        #endregion Services

        #region Constructor

        public AccountService()
        {
            var baseUrl = DependencyService.Get<IBaseUrl>();
            _appRolesRepo = new Repository<AppRolesModel>(baseUrl);
            //appUserRepo = new Repository<AppUsersModel>(baseUrl);
            restapiHelper = new RestApiHelpers();
        }


        public async Task<bool> RequestToken(LoginRequest loginRequest)
        {
            bool result = await restapiHelper.RequestTokenAsync(loginRequest.Email.Trim(), loginRequest.Password.Trim());
            return result;
        }

        public async Task<Responce<RegistrationDTO>> Register(string Name, string Email, string City, string Mobile, string Password, string ConfirmPassword, int CityId)
        {
            try
            {
                RegisterRequest registerRequest = new RegisterRequest();
                registerRequest.City = City;
                registerRequest.Name = Name.Trim();
                registerRequest.Email = Email.Trim();
                registerRequest.PhoneNumber = Mobile;
                registerRequest.Password = Password.Trim();
                registerRequest.Cityid = CityId;
                registerRequest.ConfirmPassword = ConfirmPassword.Trim();
                string Json = JsonConvert.SerializeObject(registerRequest);
                var result = await restapiHelper.PosyAsync<Responce<RegistrationDTO>>(EndPoint.Account.Register, Json);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Responce<UserinfoResponce>> UpdateUserInfo(string Name, string Email, string City, string Mobile, string UserId,int CityId)
        {
            try
            {
                UpdateUserInfoRequest updateUserInfoRequest = new UpdateUserInfoRequest();
                updateUserInfoRequest.City = City.Trim();
                updateUserInfoRequest.Name = Name.Trim();
                updateUserInfoRequest.Email = Email.Trim();
                updateUserInfoRequest.PhoneNumber = Mobile.Trim();
                updateUserInfoRequest.UserId = UserId.Trim();
                updateUserInfoRequest.CityId = CityId;

                string Json = JsonConvert.SerializeObject(updateUserInfoRequest);
                var result = await restapiHelper.PosyAsync<Responce<UserinfoResponce>>(EndPoint.Account.UpdateUserInfo, Json);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UserinfoResponce> GetUserInfo()
        {
            try
            {

                var result = await restapiHelper.GetAsync<UserinfoResponce>(EndPoint.Account.UserInfo);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> ForgotPassword(string userEmail)
        {
            try
            {
                ForgotPasswordRequest forgotPasswordReques = new ForgotPasswordRequest();
                forgotPasswordReques.Email = userEmail.Trim();
                string Json = JsonConvert.SerializeObject(forgotPasswordReques);
                var result = await restapiHelper.PosyAsync<Responce<bool>>(EndPoint.Account.ForgotPassword, Json);
                return result.ResponeContent;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> ChangePassword(string userEmail, string OTP, string Password)
        {
            try
            {
                ForgotPasswordRequest forgotPasswordReques = new ForgotPasswordRequest();
                forgotPasswordReques.Email = userEmail.Trim();
                forgotPasswordReques.OTP = OTP.Trim();
                forgotPasswordReques.Password = Password.Trim();
                string Json = JsonConvert.SerializeObject(forgotPasswordReques);
                var result = await restapiHelper.PosyAsync<Responce<bool>>(EndPoint.Account.ChangePassword, Json);
                return result.ResponeContent;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<List<AppRolesModel>> GetRoles()
        {
            try
            {
                var approles = _appRolesRepo.GetItems() ?? new List<AppRolesModel>();
                if (!approles.AnyExtended())
                {
                    var result = await restapiHelper.PosyAsync<Responce<List<AppRolesDTO>>>(EndPoint.Account.GetRoles, "");
                    if (result.Success)
                    {
                        _appRolesRepo.InsertOrReplaceAllWithChildren(result.ResponeContent.Select(a => new AppRolesModel() { ExtId = a.Id, Name = a.Name }));
                        approles = _appRolesRepo.GetItems();
                    }
                }

                return approles;
            }
            catch (Exception ex)
            {
                return new List<AppRolesModel>();
            }

        }

        public async Task<List<AppUsersDTO>> GetUsersList()
        {
            List<AppUsersDTO> appUsers = new List<AppUsersDTO>();

            try
            {
                var result = await restapiHelper.PosyAsync<Responce<List<AppUsersDTO>>>(EndPoint.Account.GetUsersList, "");
                if (result.Success)
                {
                    return result.ResponeContent ?? new List<AppUsersDTO>();
                }
                return appUsers;
            }
            catch (Exception ex)
            {
                return appUsers;
            }
        }

        public async Task<List<AppUsersDTO>> GetUsersListByCity()
        {
            List<AppUsersDTO> appUsers = new List<AppUsersDTO>();

            try
            {
                var settings = DependencyService.Get<IAppSettings>();

                MobileRequest mobileRequest = new MobileRequest();
                mobileRequest.UserId = settings.UserId;
                mobileRequest.Username = settings.UserId;
                string Json = JsonConvert.SerializeObject(mobileRequest);

                var result = await restapiHelper.PosyAsync<Responce<List<AppUsersDTO>>>(EndPoint.Account.GetUsersListByCity, Json);
                if (result.Success)
                {
                    return result.ResponeContent ?? new List<AppUsersDTO>();
                }
                return appUsers;
            }
            catch (Exception ex)
            {
                return appUsers;
            }
        }

        public async Task<bool> UpdateUserRole(string UserId, string RoleId)
        {
            try
            {
                UpdateUserRoleRequest updateUserRoleRequest = new UpdateUserRoleRequest();
                updateUserRoleRequest.Email = "";
                updateUserRoleRequest.UserId = UserId;
                updateUserRoleRequest.RoleId = RoleId;

                string Json = JsonConvert.SerializeObject(updateUserRoleRequest);
                var result = await restapiHelper.PosyAsync<Responce<bool>>(EndPoint.Account.UpdateUserRole, Json);
                return result.ResponeContent;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }

    #endregion Constructor

}