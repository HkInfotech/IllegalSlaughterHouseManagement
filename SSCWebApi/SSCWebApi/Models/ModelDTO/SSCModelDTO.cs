using System;
using System.Collections.Generic;

namespace SSCWebApi.Models.ModelDTO
{
    public class BaseDTO
    {
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsEmptyModel { get; set; }
        public bool IsActive { get; set; }

    }

    public class CityDTO : BaseDTO
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string MCEmail { get; set; }
        public string FcciEmail { get; set; }
        public string AdminEmail { get; set; }
        public string MobileNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsCreate { get; set; }

    }

    public class CityLowsDTO 
    {
        public long Id { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> LowId { get; set; }
    }


    public class SpeciesDTO : BaseDTO
    {
        public int Id { get; set; }
        public string SpeciesName { get; set; }
        public string Icon { get; set; }
    }

    public class LowDTO : BaseDTO
    {
        public int Id { get; set; }
        public string LowsTitile { get; set; }
        public string LowsDescriptions { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }

    public class ComplaintsDTO
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
        public string SpeciesName { get; set; }
        public List<int> LowIds { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsEmptyModel { get; set; }
        public string UserId { get;  set; }
        public int? ComplainStatus { get;  set; }
        public string ComplaintStatusValue { get; set; }
        public string GroupName { get;  set; }
        public bool? IsActive { get;  set; }
        public bool? IsDelete { get;  set; }

        public Nullable<bool> IsRejecet { get; set; }
        public string CommentForRejection { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<bool> IsRegister { get; set; }
        public Nullable<bool> IsEmailSend { get; set; }
        public string CreatedUserName { get; set; }
        public string UserMobileNumber { get; set; }

        public string ComplaintLowsId { get; set; }
        public string ComplaintFiles { get; set; }
        public string CityName { get; set; }
        public List<LowDTO> ComplaintLows { get; set; }
        public List<ComplaintImagesDTO> ComplaintImages { get; set; }

    }
    public class ComplaintImagesDTO : BaseDTO
    {
        public long Id { get; set; }
        public Nullable<long> ComplaintId { get; set; }
        public string ImageUrl { get; set; }
        public string Imagetype { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }

    public class ComplaintsLowsDTO : BaseDTO 
    {
        public int Id { get; set; }
        public Nullable<long> ComplaintId { get; set; }
        public Nullable<int> LowId { get; set; }
    }
}