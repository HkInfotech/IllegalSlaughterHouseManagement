using SSCWebApi.Models.ModelDTO;
using System;
using System.Collections.Generic;

namespace SSCWebApi.Models
{
    public class MobileRequest
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public int Id { get; set; }
        public List<int> Ids { get; set; }
        //public DateTime LastSyncDateTime { get; set; }
    }

    public class ChnageCityMailRequest 
    {
        public string MCEmail { get; set; }
        public string FCCIEmail { get; set; }
        public int Cityid { get; set; }
    }

    public class ComplaintRequest
    {
        public long Id { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public Nullable<System.DateTime> DateOfInspection { get; set; }
        public string Comments { get; set; }
        public string Violations { get; set; }
        public string GpsLocations { get; set; }
        public int Cityid { get; set; }
        public int SpeciesId { get; set; }
        public List<byte[]> Images { get; set; }
        public List<int> LowIds { get; set; }
        public string UserId { get; set; }
        public int? ComplainStatus { get;  set; }
        public string GroupName { get;  set; }
        public bool? IsActive { get;  set; }
        public bool? IsDelete { get;  set; }
    }


    public class UpdateUserInfoRequest 
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string UserId { get; set; }
        public int CityId { get; set; }

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
}