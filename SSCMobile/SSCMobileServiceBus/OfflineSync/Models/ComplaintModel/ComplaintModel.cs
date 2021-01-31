using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace SSCMobileServiceBus.OfflineSync.Models.ComplaintModel
{
    public class LowsModel : ModelBase
    {
        public string LowsTitile { get; set; }
        public string LowsDescriptions { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        [Ignore]
        public bool IsCheck { get; set; }

        [Ignore]
        public string NumberText { get; set; }
    }

    public class CitysModel : ModelBase
    {
        public string CityName { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string MCEmail { get; set; }
        public string FcciEmail { get; set; }
        public string AdminEmail { get; set; }
        public Nullable<bool> IsActive { get; set; }


    }

    public class CityLowsModel : ModelBase
    {
        [ForeignKey(typeof(CitysModel))]
        public int CityId { get; set; }

        [ForeignKey(typeof(LowsModel))]
        public int LowId { get; set; }

    }

    public class SpeciesModel : ModelBase
    {
        public string SpeciesName { get; set; }
        public string Icon { get; set; }

    }

    public class ComplaintModel : ModelBase
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
        public int ComplainStatus { get;  set; }
        public string GroupName { get;  set; }
        public bool? IsActive { get;  set; }
        public bool? IsDelete { get;  set; }     
        public string UserId { get;  set; }
        public string LowsId { get; set; }
        public string Files { get; set; }
        public Nullable<bool> IsRejecet { get; set; }
        public string CommentForRejection { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<bool> IsRegister { get; set; }
        public Nullable<bool> IsEmailSend { get; set; }
        public string CreatedUserName { get; set; }
        public string UserMobileNumber { get; set; }
        public string CityName { get; set; }
        public bool IsUpdate { get; set; }

        [Ignore]
        public string ComplaintLowsId { get; set; }
        [Ignore]
        public string ComplaintFiles { get; set; }

    }

    public class ComplaintImagesModel : ModelBase
    {

        public Nullable<long> ComplaintId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageLocalUrl { get; set; }
        public string Imagetype { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
    public class ComplaintsLowsModel : ModelBase
    {
        public Nullable<long> ComplaintId { get; set; }
        public Nullable<int> LowId { get; set; }
    }


}