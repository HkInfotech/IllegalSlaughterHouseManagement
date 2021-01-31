using Acr.UserDialogs;
using Newtonsoft.Json;
using Prism.Navigation;
using Prism.Services;
using SSCMobile.Helpers;
using SSCMobile.Models;
using SSCMobile.Services.Interfaces;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OfflineSync.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using SSCMobileServiceBus.OfflineSync.Repository;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Threading.Tasks;

namespace SSCMobile.Services.Implementations
{
    public class ComplaintService : IComplaintService
    {
        #region Repository

        private IRepository<CitysModel> _cityRepo;
        private IRepository<SpeciesModel> _speciesRepo;
        private IRepository<LowsModel> _lowsRepo;
        private IRepository<CityLowsModel> _cityLowsRepo;
        private IRepository<ComplaintModel> _complaintRepo;
        private IAppSettings _settings;

        #endregion Repository

        #region Service

        public RestApiHelpers restapiHelper;



        #endregion Service

        #region Constructor

        public ComplaintService(IRepository<CitysModel> cityrepository, IAppSettings settings, IRepository<SpeciesModel> Speciesrepository, IRepository<LowsModel> lowsRepo, IRepository<CityLowsModel> cityLowsrepo, IRepository<ComplaintModel> complaintRepo)
        {
            _cityRepo = cityrepository;
            _lowsRepo = lowsRepo;
            _cityLowsRepo = cityLowsrepo;
            _settings = settings;
            _speciesRepo = Speciesrepository;
            _complaintRepo = complaintRepo;
            restapiHelper = new RestApiHelpers();
        }

        #endregion Constructor


        public void DeleteCity()
        {
            List<CitysModel> CityList = new List<CitysModel>();
            CityList = _cityRepo.GetItems();
            _cityRepo.DeleteAll(CityList);

        }
        public async Task<List<CitysModel>> GetCities(string UserId)
        {
            List<CitysModel> CityList = new List<CitysModel>();
            CityList = _cityRepo.GetItems();
            try
            {
                if (!CityList.AnyExtended() && _settings.IsOnline)
                {
                    var mobileRequesy = new MobileRequest()
                    {
                        UserId = UserId
                    };
                    string Json = JsonConvert.SerializeObject(mobileRequesy);
                    var responce = await restapiHelper.PosyAsync<Responce<List<CityDTO>>>(EndPoint.Complaint.GetCities, Json);
                    if (responce.Success == true)
                    {
                        CityList = responce.ResponeContent.Select(a => a.MapCityModel()).ToList();
                        _cityRepo.InsertOrReplaceAllWithChildren(CityList);
                    }
                    CityList = _cityRepo.GetItems();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR On GetCities Call:{ex}");
            }
            return CityList;
        }

        public async Task<List<CityDTO>> GetCitiesFromServer(string UserId)
        {
            List<CityDTO> CityList = new List<CityDTO>();
            try
            {
                var mobileRequesy = new MobileRequest()
                {
                    UserId = UserId
                };
                string Json = JsonConvert.SerializeObject(mobileRequesy);
                var responce = await restapiHelper.PosyAsync<Responce<List<CityDTO>>>(EndPoint.Complaint.GetAllCities, Json);
                if (responce.Success == true)
                {
                    CityList = responce.ResponeContent.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR On GetCities Call:{ex}");
            }
            return CityList;
        }

        public CitysModel GetUserCity()
        {
            int currentCityId = _settings.UserCityId;
            var CurrentCity = _cityRepo.GetItemById(currentCityId);

            //var CurrentCity = _cityRepo.GetItems().Where(a => a.Id == currentCityId).FirstOrDefault();

            return CurrentCity;
        }

        public async Task<List<SpeciesModel>> GetAllSpecies(string UserId)
        {
            List<SpeciesModel> speceiesList = new List<SpeciesModel>();
            speceiesList = _speciesRepo.GetItems();
            try
            {
                if (!speceiesList.AnyExtended() && _settings.IsOnline)
                {
                    var mobileRequesy = new MobileRequest()
                    {
                        UserId = UserId
                    };
                    string Json = JsonConvert.SerializeObject(mobileRequesy);
                    var responce = await restapiHelper.PosyAsync<Responce<List<SpeciesDTO>>>(EndPoint.Complaint.GetAllSpecies, Json);
                    if (responce.Success == true)
                    {
                        speceiesList = responce.ResponeContent.Select(a => a.MapSpeciesModel()).ToList();
                        _speciesRepo.InsertOrReplaceAllWithChildren(speceiesList);
                    }
                    speceiesList = _speciesRepo.GetItems();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR On GetAllSpecies Call:{ex}");
                Debug.WriteLine($"ERROR On REST API Call:{EndPoint.Complaint.GetAllSpecies}");
            }
            return speceiesList;
        }

        public async Task<List<CityLowsModel>> GetCityLows(int Cityid)
        {
            List<CityLowsModel> cityLowsModels = new List<CityLowsModel>();
            cityLowsModels = _cityLowsRepo.GetItems();
            try
            {
                if (!cityLowsModels.AnyExtended() && _settings.IsOnline)
                {
                    var mobileRequesy = new MobileRequest()
                    {
                        UserId = _settings.UserId,
                        Id = Cityid
                    };
                    string Json = JsonConvert.SerializeObject(mobileRequesy);
                    var responce = await restapiHelper.PosyAsync<Responce<List<CityLowsDTO>>>(EndPoint.Complaint.GetCityLows, Json);
                    if (responce.Success == true)
                    {
                        cityLowsModels = responce.ResponeContent.Select(a => a.MapCityLowsModel()).ToList();
                        _cityLowsRepo.InsertOrReplaceAllWithChildren(cityLowsModels);
                    }
                    cityLowsModels = _cityLowsRepo.GetItems().Where(a => a.CityId == _settings.UserCityId)?.ToList() ?? new List<CityLowsModel>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR On GetCityLows Call:{ex}");
                Debug.WriteLine($"ERROR On REST API Call:{EndPoint.Complaint.GetCityLows}");
            }
            return cityLowsModels;
        }

        public async Task<List<LowsModel>> GetAllLows(string UserId, int CityId)
        {
            List<LowsModel> lowsmodels = new List<LowsModel>();
            lowsmodels = _lowsRepo.GetItems();
            try
            {

                if (!lowsmodels.AnyExtended() && _settings.IsOnline)
                {

                    var mobileRequesy = new MobileRequest()
                    {
                        UserId = UserId,
                        Id = CityId
                    };
                    string Json = JsonConvert.SerializeObject(mobileRequesy);
                    var responce = await restapiHelper.PosyAsync<Responce<List<LowDTO>>>(EndPoint.Complaint.GetLowsOfCity, Json);
                    if (responce.Success == true)
                    {
                        lowsmodels = responce.ResponeContent.Select(a => a.MapLowsModel()).ToList();
                        _lowsRepo.InsertOrReplaceAllWithChildren(lowsmodels);
                    }
                    lowsmodels = _lowsRepo.GetItems();
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR On GetAllLows Call:{ex}");
                Debug.WriteLine($"ERROR On REST API Call:{EndPoint.Complaint.GetAllLows}");
            }
            return lowsmodels;
        }

        //public async Task<List<CityLowsModel>> GetCityLows(string UserId)
        //{
        //    List<CityLowsModel> citylowsModel = new List<CityLowsModel>();
        //    citylowsModel = _cityLowsrepo.GetItems();
        //    try
        //    {
        //        if (!citylowsModel.AnyExtended() && _settings.IsOnline)
        //        {
        //            var mobileRequesy = new MobileRequest()
        //            {
        //                UserId = UserId
        //            };
        //            string Json = JsonConvert.SerializeObject(mobileRequesy);
        //            var responce = await restapiHelper.PosyAsync<Responce<List<CityLowsDTO>>>(EndPoint.Complaint.GetCityLows, Json);
        //            if (responce.Success == true)
        //            {
        //                citylowsModel = responce.ResponeContent.Select(a => a.MapCityLowsModel()).ToList();
        //                _cityLowsrepo.InsertOrReplaceAllWithChildren(citylowsModel);
        //            }
        //            citylowsModel = _lowsRepo.GetItems();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"ERROR On GetCityLows Call:{ex}");
        //        Debug.WriteLine($"ERROR On REST API Call:{EndPoint.Complaint.GetCityLows}");
        //    }
        //    return citylowsModel;
        //}

        public async Task<int> GetSpeciesByName(string name)
        {
            return _speciesRepo.GetItemByQuery(a => a.SpeciesName == name).ExternalId;
        }

        public async Task<CitysModel> GetCityByName(string name)
        {
            return _cityRepo.GetItemByQuery(a => a.CityName.ToLower().Equals(name.ToLower()));
        }

        public async Task SaveComplaint(ComplaintModel model)
        {
            try
            {
                _complaintRepo.InsertOrReplaceWithChildren(model);
            }
            catch (Exception ex)
            {

            }

        }

        public async Task<List<ComplaintModel>> GetComplaintModel()
        {
            List<ComplaintModel> complaintModels = new List<ComplaintModel>();
            try
            {
                complaintModels = _complaintRepo.GetItems();
            }
            catch (Exception ex)
            {

            }
            return complaintModels;
        }


        public void DeleteComplaint()
        {
            try
            {
                List<ComplaintModel> complaintModels = new List<ComplaintModel>();
                complaintModels = _complaintRepo.GetItems();
                _complaintRepo.DeleteAll(complaintModels);

            }
            catch (Exception ex)
            {

            }
        }
        public async Task<ComplaintModel> GetNonComplaintModel()
        {
            try
            {
                return _complaintRepo.GetItemByQuery(a => a.Operation == (int)Operations.Non);
            }
            catch (Exception)
            {
                return new ComplaintModel();
            }
        }

        public ComplaintModel GetNonComplaint()
        {
            try
            {
                return _complaintRepo.GetItemByQuery(a => a.Operation == (int)Operations.Non);
            }
            catch (Exception)
            {
                return new ComplaintModel();
            }
        }

        public SpeciesModel GetSpeciesById(int Id)
        {
            try
            {
                return _speciesRepo.GetItemByQuery(a => a.ExternalId == Id);
            }
            catch (Exception ex)
            {
                return new SpeciesModel();
            }
        }

        public async Task<bool> SaveOnServer(ComplaintModel complaintModel)
        {
            try
            {
                List<KeyValuePair<string, string>> ListOfParameters = new List<KeyValuePair<string, string>>();
                ListOfParameters.Add(new KeyValuePair<string, string>("ShopName", complaintModel.ShopName));
                ListOfParameters.Add(new KeyValuePair<string, string>("Id", Convert.ToString(complaintModel.ExternalId)));
                ListOfParameters.Add(new KeyValuePair<string, string>("ShopAddress", complaintModel.ShopAddress));
                ListOfParameters.Add(new KeyValuePair<string, string>("DateOfInspection", complaintModel.DateOfInspection?.ToString("dd-MMM-yyyy")));
                ListOfParameters.Add(new KeyValuePair<string, string>("Comments", complaintModel.Comments ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("Violations", complaintModel.Violations ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("GpsLocations", complaintModel.GpsLocations ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("UserId", _settings.UserId ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("Cityid", Convert.ToString(_settings.UserCityId) ?? "0"));
                ListOfParameters.Add(new KeyValuePair<string, string>("SpeciesId", Convert.ToString(complaintModel.SpeciesId) ?? "0"));
                ListOfParameters.Add(new KeyValuePair<string, string>("ComplainStatus", Convert.ToString(complaintModel.ComplainStatus)));
                ListOfParameters.Add(new KeyValuePair<string, string>("GroupName", complaintModel.GroupName ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsDelete", "false"));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsActive", "true"));
                ListOfParameters.Add(new KeyValuePair<string, string>("LowIds", complaintModel.LowsId));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsRejecet", Convert.ToString(complaintModel.IsRejecet ?? false)));
                ListOfParameters.Add(new KeyValuePair<string, string>("CommentForRejection", Convert.ToString(complaintModel.CommentForRejection ?? " ")));
                ListOfParameters.Add(new KeyValuePair<string, string>("RegistrationDate", complaintModel.RegistrationDate?.ToString("dd-MMMM-yyyy hh:mm:ss") ?? DateTime.Now.ToString("dd-MMMM-yyyy hh:mm:ss")));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsRegister", Convert.ToString(complaintModel.IsRegister ?? false)));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsEmailSend", Convert.ToString(complaintModel.IsEmailSend ?? false)));
                List<byte[]> fileBlob = new List<byte[]>();
                if (!string.IsNullOrEmpty(complaintModel.Files))
                {
                    List<string> SplitFilePaths = new List<string>();

                    SplitFilePaths = complaintModel.Files.Split(',')?.ToList();
                    if (SplitFilePaths.AnyExtended())
                    {
                        foreach (var item in SplitFilePaths)
                        {
                            var ByteArray = await FileExtensions.LoadFileBytesAsync(item);
                            fileBlob.Add(ByteArray);
                        }

                    }

                }

                var responce = await restapiHelper.SaveComplaint(EndPoint.Complaint.SaveComplaints, ListOfParameters, fileBlob);
                return responce;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateComplaintOnServer(ComplaintsDTO complaintsDTO)
        {
            try
            {
                List<KeyValuePair<string, string>> ListOfParameters = new List<KeyValuePair<string, string>>();
                ListOfParameters.Add(new KeyValuePair<string, string>("ShopName", complaintsDTO.ShopName));
                ListOfParameters.Add(new KeyValuePair<string, string>("Id", Convert.ToString(complaintsDTO.Id)));
                ListOfParameters.Add(new KeyValuePair<string, string>("ShopAddress", complaintsDTO.ShopAddress));
                ListOfParameters.Add(new KeyValuePair<string, string>("DateOfInspection", complaintsDTO.DateOfInspection?.ToString("dd-MMM-yyyy")));
                ListOfParameters.Add(new KeyValuePair<string, string>("Comments", complaintsDTO.Comments ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("Violations", complaintsDTO.Violations ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("GpsLocations", complaintsDTO.GpsLocations ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("UserId", _settings.UserId ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("Cityid", Convert.ToString(_settings.UserCityId) ?? "0"));
                ListOfParameters.Add(new KeyValuePair<string, string>("SpeciesId", Convert.ToString(complaintsDTO.SpeciesId) ?? "0"));
                ListOfParameters.Add(new KeyValuePair<string, string>("ComplainStatus", Convert.ToString(complaintsDTO.ComplainStatus)));
                ListOfParameters.Add(new KeyValuePair<string, string>("GroupName", complaintsDTO.GroupName ?? " "));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsDelete", "false"));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsActive", "true"));
                ListOfParameters.Add(new KeyValuePair<string, string>("LowIds", complaintsDTO.ComplaintLowsId));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsRejecet", Convert.ToString(complaintsDTO.IsRejecet ?? false)));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsEmailSend", Convert.ToString(complaintsDTO.IsEmailSend ?? false)));
                ListOfParameters.Add(new KeyValuePair<string, string>("CommentForRejection", Convert.ToString(complaintsDTO.CommentForRejection ?? " ")));
                ListOfParameters.Add(new KeyValuePair<string, string>("RegistrationDate", complaintsDTO.RegistrationDate?.ToString("dd-MMMM-yyyy hh:mm:ss") ?? DateTime.Now.ToString("dd-MMMM-yyyy hh:mm:ss")));
                ListOfParameters.Add(new KeyValuePair<string, string>("IsEmailSend", Convert.ToString(complaintsDTO.IsEmailSend ?? false)));
                List<byte[]> fileBlob = new List<byte[]>();
                if (!string.IsNullOrEmpty(complaintsDTO.ComplaintFiles))
                {
                    List<string> SplitFilePaths = new List<string>();

                    SplitFilePaths = complaintsDTO.ComplaintFiles.Split(',')?.ToList();
                    if (SplitFilePaths.AnyExtended())
                    {

                        foreach (var item in SplitFilePaths)
                        {
                            using (var webClient = new System.Net.WebClient())
                            {
                                byte[] imageBytes = webClient.DownloadData(item);
                                fileBlob.Add(imageBytes);
                            }


                        }

                    }

                }

                var responce = await restapiHelper.SaveComplaint(EndPoint.Complaint.SaveComplaints, ListOfParameters, fileBlob);
                return responce;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<ComplaintsDTO>> GetComplaintsFromServer(int id = 0)
        {
            List<ComplaintsDTO> listcomplaintmodel = new List<ComplaintsDTO>();
            //listcomplaintmodel = _complaintRepo.GetItems();
            try
            {
                //if (!lowsmodels.AnyExtended() && _settings.IsOnline)
                //{
                var mobileRequesy = new MobileRequest()
                {
                    Username = _settings.UserId,
                    Id = id
                };
                string Json = JsonConvert.SerializeObject(mobileRequesy);
                var responce = await restapiHelper.PosyAsync<Responce<List<ComplaintsDTO>>>(EndPoint.Complaint.GetComplaints, Json);
                if (responce.Success == true)
                {
                    //listcomplaintmodel = responce.ResponeContent.Select(a => a.MapComplaintModel()).ToList();
                    listcomplaintmodel = responce.ResponeContent;
                    //_cityLowsRepo.InsertOrReplaceAllWithChildren(listcomplaintmodel);
                }
                // listcomplaintmodel = _complaintRepo.GetItems();
                //}

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR On GetComplaintsFromServer Call:{ex}");
                Debug.WriteLine($"ERROR On REST API Call:{EndPoint.Complaint.GetComplaints}");
            }
            return listcomplaintmodel;
        }

        public async Task<List<ComplaintModel>> GetComplaintSync(int id = 0, bool IsSync = false)
        {
            var listcomplaintmodel = _complaintRepo.GetItems() ?? new List<ComplaintModel>();
            try
            {
                if (IsSync == true || !(listcomplaintmodel.AnyExtended()))
                {
                    var mobileRequesy = new MobileRequest()
                    {
                        Username = _settings.UserName,
                        Id = id
                    };
                    string Json = JsonConvert.SerializeObject(mobileRequesy);
                    var responce = await restapiHelper.PosyAsync<Responce<List<ComplaintsDTO>>>(EndPoint.Complaint.GetComplaints, Json);
                    if (responce.Success == true)
                    {
                        listcomplaintmodel = responce.ResponeContent.Select(a => a.MapComplaintModel()).ToList();
                        _complaintRepo.InsertOrReplaceAllWithChildren(listcomplaintmodel);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return listcomplaintmodel;
        }


        public void DropTabels()
        {

            _cityRepo.DropTable();
            _lowsRepo.DropTable();
            _cityLowsRepo.DropTable();
            _speciesRepo.DropTable();
            _complaintRepo.DropTable();
        }

        public async Task<bool> SendMail(int Id)
        {
            var mobileRequest = new MobileRequest()
            {
                Username = _settings.UserId,
                Id = Id
            };
            string Json = JsonConvert.SerializeObject(mobileRequest);
            var responce = await restapiHelper.PosyAsync<Responce<bool>>(EndPoint.Complaint.SendMail, Json);
            if (responce.Success == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ConfigCityMail(string MCEmail, string FCCIEmail, int Cityid)
        {
            var chnageCityMailRequest = new ChnageCityMailRequest()
            {
                MCEmail = MCEmail,
                FCCIEmail = FCCIEmail,
                Cityid = Cityid
            };
            string Json = JsonConvert.SerializeObject(chnageCityMailRequest);
            var responce = await restapiHelper.PosyAsync<Responce<bool>>(EndPoint.Complaint.UpdateCityMail, Json);
            if (responce.Success)
            {
                var CityModel = _cityRepo.GetItemByQuery(a => a.Id == Cityid);
                CityModel.FcciEmail = FCCIEmail;
                CityModel.MCEmail = MCEmail;
                _cityRepo.Update(CityModel);
            }
            return responce.ResponeContent;
        }

        public async Task<Responce<bool>> UpdateCityOnServer(CityDTO cityDto)
        {
            Responce<bool> responce = new Responce<bool>();
            try
            {
                string Json = JsonConvert.SerializeObject(cityDto);
                responce = await restapiHelper.PosyAsync<Responce<bool>>(EndPoint.Complaint.UpdateCity, Json);
                return responce;

            }
            catch (Exception ex)
            {

            }
            return responce;

        }

        public async Task<Responce<bool>> CreateCityOnServer(CityDTO cityDto)
        {
            Responce<bool> responce = new Responce<bool>();
            try
            {
                string Json = JsonConvert.SerializeObject(cityDto);
                responce = await restapiHelper.PosyAsync<Responce<bool>>(EndPoint.Complaint.CreateCity, Json);
                return responce;

            }
            catch (Exception ex)
            {

            }
            return responce;

        }

    }
}