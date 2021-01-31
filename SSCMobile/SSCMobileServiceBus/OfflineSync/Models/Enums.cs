using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSCMobileServiceBus.OfflineSync.Models
{
   
    

    public enum MediaAssetType
    {
        Image, Video
    }

    public enum Operations
    {
        Non,Create, Update, Delete, Sync
    }

    public enum ComplaintStatusEnum { Non, Pending , Verified , Rejected , Registered }

    public enum UserRole
    {
        [Description("No Role")]
        None,
        [Description("Controller")]
        Controller,
        [Description("Site Admin")]
        SiteAdmin,
        [Description("Volunteer")]
        Volunteer,
        [Description("Admin")]
        Admin,
    }

    public enum SortTypes { None, Ascending, Descending }
}
