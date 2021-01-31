using SSCMobile.UWP.Platform_Specific;
using SSCMobileServiceBus.Platform_Specific_Services;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]

namespace SSCMobile.UWP.Platform_Specific
{
    public class BaseUrl : IBaseUrl
    {
        public string GetDatabasePath()
        {
            var DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            if (!Directory.Exists(DbPath))
            {
                Directory.CreateDirectory(DbPath);
            }
            return Path.Combine(DbPath, "SSCMobile.db3");
            //throw new NotImplementedException();
        }
    }
}