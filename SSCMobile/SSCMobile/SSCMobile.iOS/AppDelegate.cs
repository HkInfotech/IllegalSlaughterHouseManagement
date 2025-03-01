﻿using Acr.UserDialogs;
using CarouselView.FormsPlugin.iOS;
using DLToolkit.Forms.Controls;
using Foundation;
using Octane.Xamarin.Forms.VideoPlayer.iOS;
using PanCardView.iOS;
using Prism;
using Prism.Ioc;
using SSCMobile.iOS.Platform_Specific;
using SSCMobile.Services.Interfaces;
using SSCMobileServiceBus.Platform_Specific_Services;
using UIKit;
using Xamarin;
using Xamarin.Forms;

namespace SSCMobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Forms.SetFlags("CollectionView_Experimental");
            FormsMaps.Init();
            CardsViewRenderer.Preserve();
            CarouselViewRenderer.Init();
            FlowListView.Init();
            FormsVideoPlayer.Init();
            
            global::Xamarin.Forms.FormsMaterial.Init();
           
            LoadApplication(new App(new iOSInitializer()));

            return base.FinishedLaunching(app, options);
        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.Register<IBaseUrl, BaseUrl>();
            containerRegistry.Register<IMediaService, MediaService>();
        }
    }
}