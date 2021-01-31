using Prism.Commands;
using Prism.Common;
using Prism.Mvvm;
using Prism.Navigation;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSCMobile.ViewModels.ComplaintRegister
{
    public class ImageViewPageViewModel : ViewModelBase
    {
        #region Properties
        private ComplaintImagesDTO _complaintImage;

        public ComplaintImagesDTO ComplaintImage
        {
            get { return _complaintImage; }
            set { SetProperty(ref _complaintImage, value); }
        }

        #endregion
        public ImageViewPageViewModel()
        {
            ComplaintImage = new ComplaintImagesDTO();
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var ImageRequest = parameters.GetValue<ComplaintImagesDTO>("ComplaintImageView");
            if (ImageRequest!=null) 
            {
                ComplaintImage = ImageRequest;
            }
           
        }
    }
}
