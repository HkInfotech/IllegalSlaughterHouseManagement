using Prism;
using Prism.Ioc;
using SSCMobile.UWP.Platform_Specific;
using SSCMobileServiceBus.Platform_Specific_Services;

namespace SSCMobile.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new SSCMobile.App(new UwpInitializer()));
        }
    }

    public class UwpInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.Register<IBaseUrl, BaseUrl>();
        }
    }
}