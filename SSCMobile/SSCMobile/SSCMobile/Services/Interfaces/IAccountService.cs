using SSCMobile.Helpers;
using SSCMobile.Models;
using SSCMobile.Models.Common;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMobile.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RequestToken(LoginRequest loginRequest);

        Task<Responce<RegistrationDTO>> Register(string Name, string Email, string City, string Mobile, string Password, string ConfirmPassword, int CityId);
        Task<UserinfoResponce> GetUserInfo();
        Task<Responce<UserinfoResponce>> UpdateUserInfo(string Name, string Email, string City, string Mobile, string UserId, int CityId);
        Task<bool> ForgotPassword(string userEmail);
        Task<bool> ChangePassword(string userEmail, string OTP, string Password);
        Task<List<AppRolesModel>> GetRoles();
        Task<List<AppUsersDTO>> GetUsersList();
        Task<List<AppUsersDTO>> GetUsersListByCity();
        Task<bool> UpdateUserRole(string UserId, string RoleId);
    }
}