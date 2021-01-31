using SSCMobile.iOS.Platform_Specific;
using SSCMobileServiceBus.Platform_Specific_Services;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]

namespace SSCMobile.iOS.Platform_Specific
{
    public class BaseUrl : IBaseUrl
    {
        public string GetDatabasePath()
        {
            var libPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                         "..",
                         "Library",
                         "data");
            if (!Directory.Exists(libPath))
            {
                Directory.CreateDirectory(libPath);
            }

            var DbPath = Path.Combine(libPath + "..", "Library", "data", "SSCMobile.db3");
            return DbPath;
        }
    }
}