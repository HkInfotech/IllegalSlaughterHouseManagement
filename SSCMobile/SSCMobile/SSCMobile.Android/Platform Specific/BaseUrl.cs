using SSCMobile.Droid.Platform_Specific;
using SSCMobileServiceBus.Platform_Specific_Services;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]

namespace SSCMobile.Droid.Platform_Specific
{
    public class BaseUrl : IBaseUrl
    {
        public string GetDatabasePath()
        {
            var dbpath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(dbpath, "SSCMobile.db3");
            return path;
        }
    }
}