using System;
using System.Collections.Generic;

namespace SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO
{
    public class CityDTO : BaseModelDTO
    {
        public string CityName { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string MCEmail { get; set; }
        public string FcciEmail { get; set; }
        public string AdminEmail { get; set; }
        public string MobileNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsCreate { get; set; }
      

        public Nullable<bool> IsActive { get; set; }
    }

    public class SpeciesDTO : BaseModelDTO
    {
        public string SpeciesName { get; set; }
        public string Icon { get; set; }
    }
    public class CityLowsDTO : BaseModelDTO
    {
      
       
        public int CityId { get; set; }
        public int LowId { get; set; }

    }
    public class LowDTO : BaseModelDTO
    {
        public string LowsTitile { get; set; }
        public string LowsDescriptions { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }

    public class ComplaintsDTO : BaseModelDTO
    {
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
        public string UserId { get; set;  }
        public string ComplaintStatusValue { get; set; }
        public int ComplainStatus { get; set;  }
        public string GroupName { get; set;  }
        public bool? IsActive { get; set;  }
        public bool? IsDelete { get; set;  }
        public string ComplaintLowsId { get; set; }
        public string ComplaintFiles { get; set; }

        public Nullable<bool> IsRejecet { get; set; }
        public string CommentForRejection { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<bool> IsRegister { get; set; }
        public Nullable<bool> IsEmailSend { get; set; }
        public string CreatedUserName { get; set; }
        public string UserMobileNumber { get; set; }
        public string CityName { get; set; }
        public List<LowDTO> ComplaintLows { get; set; }
        public List<ComplaintImagesDTO> ComplaintImages { get; set; }
    }

    public class ComplaintImagesDTO : BaseModelDTO
    {
        public Nullable<long> ComplaintId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageLocalUrl { get; set; }
        public string Imagetype { get; set; }
        public string FileImage { get; set; }
        public string FileType { get; set; } = "Image";
        public Nullable<bool> IsDelete { get; set; }

    }
    public class ComplaintsLowsDTO : BaseModelDTO
    {       
        public Nullable<int> ComplaintId { get; set; }
        public Nullable<int> LowId { get; set; }
    }
}