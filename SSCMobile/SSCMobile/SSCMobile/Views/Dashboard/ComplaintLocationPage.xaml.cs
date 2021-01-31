using SSCMobile.Commonfiles;
using SSCMobile.Helpers;
using SSCMobile.ViewModels;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace SSCMobile.Views.Dashboard
{
    public partial class ComplaintLocationPage : ContentPage
    {
        ComplaintLocationPageViewModel vm;
        public ComplaintLocationPage()
        {
            InitializeComponent();
            vm = (ComplaintLocationPageViewModel)BindingContext;
        }

        async private void BindableMap_MapClicked(object sender, Xamarin.Forms.Maps.MapClickedEventArgs e)
        {
            try
            {
                var _setting = DependencyService.Get<IAppSettings>();
                if (_setting.IsLogin && _setting.IsOnline)
                {
                    vm.MyPosition = new Position(e.Position.Latitude, e.Position.Longitude);
                    vm.IsBusy = true;
                    var placemarks = await Geocoding.GetPlacemarksAsync(e.Position.Latitude, e.Position.Longitude);
                    var placemark = placemarks?.FirstOrDefault();
                    string Locationstring = "";
                    if (placemark != null)
                    {
                        Locationstring = $"{placemark.SubAdminArea}  {placemark.AdminArea}, { placemark.SubLocality} {placemark.Locality} ,  {placemark.PostalCode} {placemark.CountryName}";


                    }
                    vm.PinCollection = new System.Collections.ObjectModel.ObservableCollection<Pin>();
                    vm.PinCollection.Add(new Pin() { Position = vm.MyPosition, Type = PinType.Generic, Label = $"Slaughter House Location" });
                   
                  
                    vm.IsBusy = false;
                }
                else
                {
                    vm.IsEmpty = true;
                    vm.EmptyStateTitle = AppAlertMessage.NoInternetConnections;
                }

            }
            catch (Exception ex)
            {
                vm.IsBusy = false;
            }

        }
    }
}
