using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Models.Common;
using SSCMobile.Services.Implementations;
using SSCMobile.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SSCMobile.ViewModels.ComplaintRegister
{
    public class CompalintImageUploadPageViewModel : ViewModelBase
    {
        #region Services

        private IUserDialogs _userDialogs;
        private IComplaintService _complaintService;
        private IMediaService _mediaService;

        #endregion Services
        #region Properties
        private ObservableCollection<LowsModel> _lowsList;
       
        public string SearchText { get; set; }
   
        public ObservableCollection<MediaAsset> MediaAssets { get; set; }
        MediaAsset _mediaSelected;


       

        #endregion

        #region command
        private DelegateCommand _submitcommand;

        public DelegateCommand Submitcommand =>
           _submitcommand ?? (_submitcommand = new DelegateCommand(async () => await ExecuteSubmitCommand()));
        private DelegateCommand _skipcommand;

        public DelegateCommand SkipCommand =>
           _skipcommand ?? (_skipcommand = new DelegateCommand(async () => await ExecuteSkipCommand()));

       

        private DelegateCommand<MediaAsset> _itemTappedCommand;

        public DelegateCommand<MediaAsset> ItemTappedCommand =>
           _itemTappedCommand ?? (_itemTappedCommand = new DelegateCommand<MediaAsset>(async (a) => await ExecuteItemTappedCommand(a)));

        private DelegateCommand _uploadImageCommand;

        public DelegateCommand UploadImageCommand =>
           _uploadImageCommand ?? (_uploadImageCommand = new DelegateCommand(async () => await ExecuteUploadImageCommand()));

        
        #endregion
        public CompalintImageUploadPageViewModel(IUserDialogs userDialogs, IComplaintService complaintService, IAppSettings appSettings, IPageDialogService pageDialogService, INavigationService navigationService, IMediaService mediaService) : base(navigationService, appSettings, pageDialogService)
        {
            _complaintService = complaintService;
          //  MediaAssets = new ObservableCollection<MediaAsset>();
            //_mediaService = mediaService;
            //Xamarin.Forms.BindingBase.EnableCollectionSynchronization(MediaAssets, null, ObservableCollectionCallback);
            //_mediaService.OnMediaAssetLoaded += OnMediaAssetLoaded;

        }
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            var ComplaintsDetails = parameters.GetValue<ComplaintModel>("ComplaintRequest");
            ComplaintModelObj = ComplaintsDetails;
            await LoadMediaAssets();
        }

        public override async void OnNavigatedFrom(INavigationParameters parameters)
        {
          
            var navigationparameters = new NavigationParameters();
            navigationparameters.Add("ComplaintRequest", ComplaintModelObj);
          
            await NavigationService.GoBackAsync(navigationparameters);
        }
        private void OnMediaAssetLoaded(object sender, MediaEventArgs e)
        {
            MediaAssets.Add(e.Media);
        }
        void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            // `lock` ensures that only one thread access the collection at a time
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }
        private Task ExecuteSubmitCommand()
        {
            throw new NotImplementedException();
        }
        private Task ExecuteUploadImageCommand()
        {
            throw new NotImplementedException();
        }

        private async  Task ExecuteItemTappedCommand(MediaAsset mediaAsset)
        {
            //if (mediaAsset.Type == MediaAssetType.Video)
            //{
            //    await App.Current.MainPage.Navigation.PushAsync(new VideoDetailPage(mediaAsset.Path));
            //}
            //else
            //{
            //    await App.Current.MainPage.Navigation.PushAsync(new ImageDetailPage(mediaAsset.Path));
            //}
        }

        async  Task LoadMediaAssets()
        {
            try
            {
                await _mediaService.RetrieveMediaAssetsAsync();
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Task was cancelled");
            }
        }
        private async Task ExecuteSkipCommand()
        {

            await NavigationService.NavigateAsync(AppPages.ComplaintRegister.ComplaintOverviewPage);
        }
    }
}