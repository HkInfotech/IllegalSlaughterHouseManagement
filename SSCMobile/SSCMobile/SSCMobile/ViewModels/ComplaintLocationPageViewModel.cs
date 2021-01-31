using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace SSCMobile.ViewModels
{
    public class ComplaintLocationPageViewModel : ViewModelBase
    {
        #region Services

        public IComplaintService _complaintService;
        private IUserDialogs _userDialogs;

        #endregion

        #region Properties 
        private ObservableCollection<Pin> _pinCollection = new ObservableCollection<Pin>();
        public ObservableCollection<Pin> PinCollection
        {
            get
            {
                return _pinCollection;
            }
            set
            {
                SetProperty(ref _pinCollection, value);
            }
        }
        private DelegateCommand _saveLocationsubmitCommand;

        public DelegateCommand SaveLocationSubmitCommand =>
            _saveLocationsubmitCommand ?? (_saveLocationsubmitCommand = new DelegateCommand(async () => await ExecuteSubmitCommandAsync()));



        private Position _myPosition = new Position(20.5937, 78.9629);
        public Position MyPosition { get { return _myPosition; } set { _myPosition = value; OnPropertyChanged(); } }
        #endregion
        public ComplaintLocationPageViewModel(INavigationService navigationservice, IAppSettings settings, IUserDialogs userDialogs, IPageDialogService pageDialogService, IComplaintService complaintService) : base(navigationservice, settings, pageDialogService)
        {
            _complaintService = complaintService;
            _userDialogs = userDialogs;
            ComplaintModelObj = new ComplaintModel();
        }

        public override async void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            ComplaintModelObj = _complaintService.GetNonComplaint();
        }


        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            ComplaintModelObj = _complaintService.GetNonComplaint();

            try
            {
                if (_settings.IsOnline == true)
                {
                    if (string.IsNullOrEmpty(ComplaintModelObj.GpsLocations))
                    {

                        IsBusy = true;
                        var position = await Plugin.Geolocator.CrossGeolocator.Current.GetPositionAsync();
                        var placemarks = await Geocoding.GetPlacemarksAsync(position.Latitude, position.Longitude);
                        var placemark = placemarks?.FirstOrDefault();
                        string Locationstring = "";
                        if (placemark != null)
                        {
                            Locationstring = $"{placemark.SubAdminArea}  {placemark.AdminArea}, { placemark.SubLocality} {placemark.Locality} ,  {placemark.PostalCode} {placemark.CountryName}";
                        }

                        MyPosition = new Position(position.Latitude, position.Longitude);
                        //PinCollection.Add(new Pin() { Position = MyPosition, Type = PinType.Generic/*, Label = "Location", Address = string.IsNullOrEmpty(Locationstring) ? null : Locationstring */,Effects });
                        PinCollection.Add(new Pin() { Position = MyPosition, Type = PinType.Generic, Label = "Slaughter House Location" });
                        IsBusy = false;


                    }
                    else
                    {
                        string[] locationsplict = ComplaintModelObj.GpsLocations.Split(',');
                        MyPosition = new Position(Convert.ToDouble(locationsplict[0]),Convert.ToDouble(locationsplict[1]));
                        //PinCollection.Add(new Pin() { Position = MyPosition, Type = PinType.Generic/*, Label = "Location", Address = string.IsNullOrEmpty(Locationstring) ? null : Locationstring */,Effects });
                        PinCollection.Add(new Pin() { Position = MyPosition, Type = PinType.Generic, Label = "Slaughter House Location" });
                        IsBusy = false;
                    }
                }
                else
                {
                    IsEmpty = true;
                    IsBusy = false;
                    EmptyStateTitle = AppAlertMessage.NoInternetConnections;
                }
                

            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
                IsBusy = false;
            }



        }

        private async Task ExecuteSubmitCommandAsync()
        {
            try
            {
                var IsSaveLocation = await _userDialogs.ConfirmAsync("Are you sure you want to save this location as the location of Slaughter House", "Slaughter House Location", "Yes", "No");
                if (IsSaveLocation == true)
                {
                    ComplaintModelObj = await _complaintService.GetNonComplaintModel();
                    if (ComplaintModelObj != null)
                    {
                        ComplaintModelObj.GpsLocations = Convert.ToString($"{MyPosition.Latitude}, {MyPosition.Longitude}");
                        await _complaintService.SaveComplaint(ComplaintModelObj);
                        await NavigationService.GoBackAsync();
                    }



                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync(null, AppAlertMessage.TechnicalError, "OK");
                IsBusy = false;
            }


        }
    }
}
