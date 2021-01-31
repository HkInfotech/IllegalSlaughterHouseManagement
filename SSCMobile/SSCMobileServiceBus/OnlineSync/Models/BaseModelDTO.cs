using System;

namespace SSCMobileServiceBus.OnlineSync.Models
{
    public class BaseModelDTO
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsEmptyModel { get; set; }
    }
}