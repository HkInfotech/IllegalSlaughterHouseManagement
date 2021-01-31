using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobileServiceBus.OfflineSync.Models.ComplaintModel
{
    public class AppRolesModel : ModelBase
    {
        public string ExtId { get; set; }
        public string Name { get; set; }
        [Ignore]
        public bool IsCheck { get; set; }
    }

    public class AppUsersModel : ModelBase
    {
        public string ExtId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
       

    }
}
