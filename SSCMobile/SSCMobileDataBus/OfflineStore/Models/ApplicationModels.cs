using SSCMobileServiceBus.OfflineSync;

namespace SSCMobileDataBus.OfflineStore.Models
{
    public class UserProfile : ModelBase
    {
        public string UserId { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string FCMToken { get; set; }
        public string Location { get; set; }
    }
}