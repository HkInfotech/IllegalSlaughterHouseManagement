using System.Collections.Generic;

namespace SSCMobile.Models
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string City { get; set; }
        public int Cityid { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }

    }

    public class MobileRequest
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public int Id { get; set; }
        public List<int> Ids { get; set; }
        //public DateTime LastSyncDateTime { get; set; }
    }

    public class UpdateUserInfoRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public int CityId { get;set; }
        public string UserId { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string OTP { get; set; }
    }
    public class UpdateUserRoleRequest
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }


    }

    public class ChnageCityMailRequest
    {
        public string MCEmail { get; set; }
        public string FCCIEmail { get; set; }
        public int Cityid { get; set; }
    }
}