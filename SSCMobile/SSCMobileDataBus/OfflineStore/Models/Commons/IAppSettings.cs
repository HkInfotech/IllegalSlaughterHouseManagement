namespace SSCMobileDataBus.OfflineStore.Models.Commons
{
    public interface IAppSettings
    {
        string ApplicationName { get; set; }
        bool IsLogin { get; set; }
        bool IsOnline { get; set; }
        bool IsLogin2 { get; set; }

        string BaseUrl { get; set; }

        string Token { get; set; }
        string UserName { get; set; }
        string UserId { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
        string Name { get; set; }
        string UserRole { get; set; }

        string UserCity { get; set; }
        int UserCityId { get; set; }
      
    }
}