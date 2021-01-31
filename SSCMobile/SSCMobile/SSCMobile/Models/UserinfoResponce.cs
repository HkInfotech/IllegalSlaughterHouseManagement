using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobile.Models
{
    public class UserinfoResponce
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }

        public string UserId { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string UserRole { get; set; }

    }
}
