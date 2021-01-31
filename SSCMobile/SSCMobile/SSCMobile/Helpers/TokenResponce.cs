namespace SSCMobile.Helpers
{
    public class TokenResponce
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
        public string userName { get; set; }
    }
}