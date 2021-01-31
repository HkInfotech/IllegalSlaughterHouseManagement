using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO
{
    public class AppRolesDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AppUsersDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string UserRole { get; set; }
        public string RoleId { get; set; }
    }

}
