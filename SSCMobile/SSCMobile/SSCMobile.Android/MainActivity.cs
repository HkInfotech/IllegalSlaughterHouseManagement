using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using CarouselView.FormsPlugin.Android;
using DLToolkit.Forms.Controls;
using FFImageLoading.Forms.Platform;
using Octane.Xamarin.Forms.VideoPlayer.Android;
using PanCardView.Droid;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Prism;
using Prism.Ioc;
using SSCMobile.Droid.Platform_Specific;
using SSCMobile.Services.Interfaces;
using SSCMobileServiceBus.Platform_Specific_Services;
using Xamarin.Forms;

namespace SSCMobile.Droid
{
    [Activity(Label = "Living Free", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            UserDialogs.Init(this);

            base.OnCreate(bundle);
            CrossCurrentActivity.Current.Init(this, bundle);
            Forms.SetFlags("CollectionView_Experimental");

            global::Xamarin.Forms.Forms.Init(this, bundle);
            CardsViewRenderer.Preserve();
            global::Xamarin.Forms.FormsMaterial.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            CarouselViewRenderer.Init();
            FormsVideoPlayer.Init();
            FlowListView.Init();

            CachedImageRenderer.Init(true);
            CachedImageRenderer.InitImageViewHandler();
            Android.Glide.Forms.Init(this);
            LoadApplication(new App(new AndroidInitializer()));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
          
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.Register<IBaseUrl, BaseUrl>();
            //containerRegistry.Register<IMediaService, MediaService>();
        }
    }
}