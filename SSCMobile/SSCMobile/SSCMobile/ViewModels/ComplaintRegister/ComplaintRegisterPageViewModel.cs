using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Services.Interfaces;
using SSCMobile.Validations;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace SSCMobile.ViewModels.ComplaintRegister
{
    public class ComplaintRegisterPageViewModel : ViewModelBase
    {
        #region Services

        private IUserDialogs _userDialogs;
        private IComplaintService _complaintService;

        #endregion Services

        #region Properties

        private bool _hasFiles;

        public bool HasFiles
        {
            get
            {
                return _hasFiles;
            }
            set
            {
                SetProperty(ref _hasFiles, value);
            }
        }

       

        private ObservableCollection<ComplaintImagesDTO> _complaintImagesDTOs;

        public ObservableCollection<ComplaintImagesDTO> ComplaintImagesDTOs
        {
            get
            {
                return _complaintImagesDTOs;
            }
            set
            {
                SetProperty(ref _complaintImagesDTOs, value);
            }
        }

        #region ShopName

        private ValidatableObject<string> _shopName;

        public ValidatableObject<string> ShopName
        {
            get
            {
                return _shopName;
            }
            set
            {
                _shopName = value;
                RaisePropertyChanged(() => ShopName);
            }
        }

        private DelegateCommand _validateShopNameCommand;

        public DelegateCommand ValidateShopNameCommand =>
            _validateShopNameCommand ?? (_validateShopNameCommand = new DelegateCommand(() => ExecuteShopNameCommand()));

        private bool ExecuteShopNameCommand()
        {
            return _shopName.Validate();
        }

        #endregion ShopName

        #region ShopAddress

        private ValidatableObject<string> _shopAddress;

        public ValidatableObject<string> ShopAddress
        {
            get
            {
                return _shopAddress;
            }
            set
            {
                _shopAddress = value;
                RaisePropertyChanged(() => ShopAddress);
            }
        }

        private DelegateCommand _validateShopAddressCommand;

        public DelegateCommand ValidateShopAddressCommand =>
            _validateShopAddressCommand ?? (_validateShopAddressCommand = new DelegateCommand(() => ExecuteShopAddressCommand()));

        #endregion ShopAddress

        #region Species

        private ValidatableObject<string> _species;

        public ValidatableObject<string> Species
        {
            get
            {
                return _species;
            }
            set
            {
                _species = value;
                RaisePropertyChanged(() => Species);
            }
        }

        private DelegateCommand _validateSpeciesCommand;

        public DelegateCommand ValidateSpeciesCommand =>
            _validateSpeciesCommand ?? (_validateSpeciesCommand = new DelegateCommand(async () => await ExecuteSpeciesCommandAsync()));

        private async Task<bool> ExecuteSpeciesCommandAsync()
        {
            var buttons = new List<IActionSheetButton>()
            {
                ActionSheetButton.CreateButton("---Select species---",WriteLine,"Select option"),

                ActionSheetButton.CreateCancelButton("Cancel", WriteLine, "Cancel")
            };
            var Species = await _complaintService.GetAllSpecies(_settings.UserId);
            foreach (var item in Species)
            {
                buttons.Add(ActionSheetButton.CreateButton(item.SpeciesName, WriteLine, item.SpeciesName));
            }
            await PageDialogService.DisplayActionSheetAsync("Select species", buttons.ToArray());
            return _species.Validate();
        }

        private void WriteLine(string message)
        {
            if (message == "Cancel")
            {
            }
            else
            {
                _species.Value = message;
            }
        }

        #endregion Species

        #region DateOfInspection

        private ValidatableObject<DateTime> _dateOfInspection;

        public ValidatableObject<DateTime> DateOfInspection
        {
            get
            {
                return _dateOfInspection;
            }
            set
            {
                _dateOfInspection = value;
                RaisePropertyChanged(() => DateOfInspection);
            }
        }

        private DelegateCommand _validateDateOfInspectionCommand;

        public DelegateCommand ValidateDateOfInspectionCommand =>
            _validateDateOfInspectionCommand ?? (_validateDateOfInspectionCommand = new DelegateCommand(async () => await ExecuteDateOfInspectionCommandAsync()));

        #endregion DateOfInspection

        #region Comment

        private ValidatableObject<string> _comment;

        public ValidatableObject<string> Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                RaisePropertyChanged(() => Comment);
            }
        }

        private DelegateCommand _validateCommentCommand;

        public DelegateCommand ValidateCommentCommand =>
            _validateCommentCommand ?? (_validateCommentCommand = new DelegateCommand(async () => await ExecuteCommentCommandAsync()));

        #endregion Comment

        private DelegateCommand _submitCommand;

        public DelegateCommand SubmitCommand =>
            _submitCommand ?? (_submitCommand = new DelegateCommand(async () => await ExecuteSubmitCommandAsync()));

        private DelegateCommand _pickComplaintLocationPage;

        public DelegateCommand PickComplaintLocationPage =>
            _pickComplaintLocationPage ?? (_pickComplaintLocationPage = new DelegateCommand(async () => await NavigateComplaintLocationPage()));

        private async Task NavigateComplaintLocationPage()
        {
            NavigationParameters parameters = new NavigationParameters();
            ComplaintModelObj = await _complaintService.GetNonComplaintModel();
            parameters.Add("complaint", ComplaintModelObj);
            await NavigationService.NavigateAsync(AppPages.DashBoard.ComplaintLocationPage);
        }

        private DelegateCommand _uploadimagecommand;

        public DelegateCommand Uploadimagecommand =>
            _uploadimagecommand ?? (_uploadimagecommand = new DelegateCommand(async () => await ExecuteUploadimagecommandAsync()));

        private DelegateCommand<ComplaintImagesDTO> _imageTappedClickCommand;

        public DelegateCommand<ComplaintImagesDTO> ImageTappedClickCommand =>
            _imageTappedClickCommand ?? (_imageTappedClickCommand = new DelegateCommand<ComplaintImagesDTO>(async (a) => await ExecuteImageTappedClickCommandAsync(a)));

        #endregion Properties

        public ComplaintRegisterPageViewModel(IUserDialogs userDialogs, IComplaintService complaintService, IAppSettings appSettings, IPageDialogService pageDialogService, INavigationService navigationService) : base(navigationService, appSettings, pageDialogService)
        {
            ShopName = new ValidatableObject<string>();
            ShopAddress = new ValidatableObject<string>();
            Species = new ValidatableObject<string>();
            Species.Value = "---Select species---";
            DateOfInspection = new ValidatableObject<DateTime>();
            DateOfInspection.Value = DateTime.Now;
            Comment = new ValidatableObject<string>();
            AddValidations();
            _userDialogs = userDialogs;
            _complaintService = complaintService;
            ComplaintModelObj = _complaintService.GetNonComplaint();
            ComplaintImagesDTOs = new ObservableCollection<ComplaintImagesDTO>();
            
        }

        private void AddValidations()
        {
            _shopName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Shop name is required"
            });
            _shopAddress.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Shop address is required"
            });
            _species.Validations.Add(new SpeciesSelectRule<string>
            {
                ValidationMessage = "Species is required"
            });
        }
        private Position _myPosition = new Position(20.5937, 78.9629);
        public Position MyPosition { get { return _myPosition; } set { _myPosition = value; OnPropertyChanged(); } }
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
             ComplaintImagesDTOs = new ObservableCollection<ComplaintImagesDTO>();


            await _complaintService.GetCities(_settings.UserId);
            await _complaintService.GetAllSpecies(_settings.UserId);
            HasFiles = false;
          
            if (ComplaintModelObj != null)
            {
                if (!string.IsNullOrWhiteSpace(ComplaintModelObj.ShopName))
                {
                    ShopName.Value = ComplaintModelObj.ShopName;
                }

                if (!string.IsNullOrWhiteSpace(ComplaintModelObj.ShopAddress))
                {
                    ShopAddress.Value = ComplaintModelObj.ShopAddress;
                }
                if (!string.IsNullOrWhiteSpace(ComplaintModelObj.SpeciesName))
                {
                    Species.Value = ComplaintModelObj.SpeciesName;
                }
                if (!string.IsNullOrWhiteSpace(ComplaintModelObj.Comments))
                {
                    Comment.Value = ComplaintModelObj.Comments;
                }

                DateOfInspection.Value = ComplaintModelObj.DateOfInspection ?? DateTime.Now;

                if (!string.IsNullOrEmpty(ComplaintModelObj.Files))
                {
                    List<string> SplitFilePaths = new List<string>();
                    SplitFilePaths = ComplaintModelObj.Files.Split(',')?.ToList();
                    if (SplitFilePaths.AnyExtended())
                    {
                        foreach (var item in SplitFilePaths)
                        {
                            ComplaintImagesDTOs.Add(new ComplaintImagesDTO()
                            {
                                ComplaintId = 0,
                                FileType = "Image",
                                FileImage = item
                            });
                        }
                        CheckHashFile();
                    }
                }
            }

        }
        async public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            //if (_settings.IsOnline)
            //{
            //    var GetPosition = parameters.GetValue<Position>("MyPosition");
            //    if (GetPosition.Latitude != 0)
            //    {
            //        MyPosition = GetPosition;
            //    }
                
            //}
        }

        public async Task ValidateFields()
        {
            if ((_species.Value == "---Select species---"))
            {
                _species.Validate();
            }
            ValidateShopNameCommand.Execute();
            ValidateShopAddressCommand.Execute();
            ValidateDateOfInspectionCommand.Execute();
            ValidateCommentCommand.Execute();
        }

        private async Task ExecuteSubmitCommandAsync()
        {


            await ValidateFields();
            if ((_shopAddress.IsValid && _shopName.IsValid) && (_species.IsValid && _dateOfInspection.IsValid))
            {
                //var IsRegister = await _userDialogs.ConfirmAsync("Are you sure want to register complaint ?", "Register complaint.", "Yes", "No");
                if (1 == 1)
                {
                    var CheckTempComplaintCreated = await _complaintService.GetNonComplaintModel();
                    var complaint = CheckTempComplaintCreated ?? new ComplaintModel();
                    complaint.ShopAddress = ShopAddress.Value;
                    complaint.ShopName = ShopName.Value;
                    complaint.DateOfInspection = DateOfInspection.Value;
                    int Speceid = await _complaintService.GetSpeciesByName(Species.Value);
                    complaint.SpeciesId = Speceid;
                    complaint.ShopAddress = ShopAddress.Value;
                    complaint.Comments = Comment.Value;
                    complaint.Operation = (int)Operations.Non;
                    complaint.ApplicationType = 1;
                    complaint.IsEmptyModel = false;
                    complaint.Violations = "";
                    complaint.Cityid = _settings.UserCityId;                   
                    complaint.IsActive = true;
                    complaint.IsDelete = false;
                    complaint.SpeciesName = Species.Value;

                    if (ComplaintImagesDTOs.AnyExtended())
                    {
                        List<string> images = new List<string>();
                        foreach (var item in ComplaintImagesDTOs)
                        {
                            images.Add(item.FileImage);
                        }
                        complaint.Files = string.Join(",", images.ToArray());
                    }
                    else
                    {
                        complaint.Files = "";
                    }

                    await _complaintService.SaveComplaint(complaint);
                    ComplaintModelObj = complaint;
                    await NavigationService.NavigateAsync(AppPages.ComplaintRegister.SelectLawsPage);
                }

            }
        }

        private async Task<bool> ExecuteCommentCommandAsync()
        {
            return _comment.Validate();
        }

        private async Task<bool> ExecuteDateOfInspectionCommandAsync()
        {
            return _dateOfInspection.Validate();
        }

        private bool ExecuteShopAddressCommand()
        {
            return _shopAddress.Validate();
        }

        private async Task ExecuteUploadimagecommandAsync()
        {
            try
            {
                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<MediaLibraryPermission>();

                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.MediaLibrary))
                    {
                        await PageDialogService.DisplayAlertAsync(null, "App needs permission to access the media library.", "OK");
                    }
                    status = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                }
                if (status == PermissionStatus.Granted)
                {
                }
                if (cameraStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await PageDialogService.DisplayAlertAsync(null, "App needs permission to access the camera.", "OK");
                    }


                    cameraStatus = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                }

                if (storageStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await PageDialogService.DisplayAlertAsync(null, "App needs permission to access the phone's local storage.", "OK");
                    }

                    storageStatus = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
                }

                if (status == PermissionStatus.Granted && cameraStatus == PermissionStatus.Granted)
                {
                    List<IActionSheetButton> buttons = new List<IActionSheetButton>()
                    {
                        ActionSheetButton.CreateButton("Gallery", new Action(async () =>
                            {
                                await UplaodImageFromGallery();
                            })),
                         ActionSheetButton.CreateButton("Camera", new Action(async () =>
                            {
                                await UplaodImageFromCamera();
                            })),
                            ActionSheetButton.CreateCancelButton("Cancel", new Action(() => { }))
                    };
                    await PageDialogService.DisplayActionSheetAsync("Select Option", buttons.ToArray());
                }
                else
                {
                    await PageDialogService.DisplayAlertAsync(null, "Sorry, permission is not granted to use the media library.", "OK");
                }
            }
            catch (Exception ex)
            {
                string st = ex.ToString();
            }
        }

        private async Task UplaodImageFromCamera()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await PageDialogService.DisplayAlertAsync(null, "Sorry, permission is not granted to use the camera.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Test",
                SaveToAlbum = true,
                CompressionQuality = 75,
                CustomPhotoSize = 50,
                PhotoSize = PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2000,
                DefaultCamera = CameraDevice.Front
            });

            if (file == null)
                return;

            //var source = FileExtensions.ReadFully(file.GetStream()).GetAwaiter().GetResult();
            //file.Dispose();
            var FilePath = file.Path;
            ComplaintImagesDTOs.Add(new ComplaintImagesDTO()
            {
                ComplaintId = 0,
                FileType = "Image",
                FileImage = FilePath
            });
            file.Dispose();
            CheckHashFile();
        }

        public void CheckHashFile()
        {
            if (ComplaintImagesDTOs.AnyExtended())
            {
                HasFiles = true;
            }
            else
            {
                HasFiles = false;
            }
        }

        private async Task UplaodImageFromGallery()
        {
            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
            });

            //var source = FileExtensions.ReadFully(file.GetStream()).GetAwaiter().GetResult();
            //var FilePath = FileExtensions.SaveAttachmentInLocalFolder(source, "Attachment", "jpeg");
            if (file == null)
                return;

            ComplaintImagesDTOs.Add(new ComplaintImagesDTO()
            {
                ComplaintId = 0,
                FileType = "Image",
                FileImage = file.Path
            });
            file.Dispose();
            CheckHashFile();
        }

        private async Task ExecuteImageTappedClickCommandAsync(ComplaintImagesDTO complaintImagesDTO)
        {
            List<IActionSheetButton> buttons = new List<IActionSheetButton>()
                    {
                        ActionSheetButton.CreateButton("View file", new Action(async () =>
                            {
                                await ViewFile(complaintImagesDTO);
                            })),
                         ActionSheetButton.CreateButton("Delete file", new Action(async () =>
                            {
                                await DeleteFile(complaintImagesDTO);
                            })),
                            ActionSheetButton.CreateCancelButton("Cancel", new Action(() => { }))
                    };
            await PageDialogService.DisplayActionSheetAsync("Select Option", buttons.ToArray());
        }

        private async Task DeleteFile(ComplaintImagesDTO complaintImagesDTO)
        {
            ComplaintImagesDTOs.Remove(complaintImagesDTO);
            CheckHashFile();
        }

        private async Task ViewFile(ComplaintImagesDTO complaintImagesDTO)
        {
            var Navigationparameters = new NavigationParameters();
            if (ComplaintImagesDTOs.AnyExtended())
            {
                List<string> images = new List<string>();
                foreach (var item in ComplaintImagesDTOs)
                {
                    images.Add(item.FileImage);
                }
                ComplaintModelObj.Files = string.Join(",", images.ToArray());
                await _complaintService.SaveComplaint(ComplaintModelObj);
            }
            Navigationparameters.Add("ComplaintImageView", complaintImagesDTO);
            await NavigationService.NavigateAsync(AppPages.ComplaintRegister.ImageViewPage, Navigationparameters);
        }
    }
}