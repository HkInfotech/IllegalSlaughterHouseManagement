using SSCMobile.Helpers;
using SSCMobile.Models;
using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMobile.Services.Interfaces
{
    public interface IComplaintService
    {
        Task<List<CitysModel>> GetCities(string UserId);
        Task<List<SpeciesModel>> GetAllSpecies(string UserId);
        SpeciesModel GetSpeciesById(int Id);
        Task<List<CityLowsModel>> GetCityLows(int Cityid);
        Task<List<LowsModel>> GetAllLows(string UserId,int CityId);
        Task<bool> UpdateComplaintOnServer(ComplaintsDTO complaintsDTO);

        Task<int> GetSpeciesByName(string name);

        Task<CitysModel> GetCityByName(string name);

        Task SaveComplaint(ComplaintModel model);
        Task<bool> ConfigCityMail(string MCEmail, string FCCIEmail, int Cityid);

        Task<List<ComplaintModel>> GetComplaintModel();

        Task<ComplaintModel> GetNonComplaintModel();
        Task<bool> SaveOnServer(ComplaintModel complaintModel);
        Task<bool> SendMail(int Id);
        Task<List<ComplaintsDTO>> GetComplaintsFromServer(int id);
        void DeleteComplaint();

        void DropTabels();
        ComplaintModel GetNonComplaint();
        CitysModel GetUserCity();
        Task<List<CityDTO>> GetCitiesFromServer(string UserId);

        Task<Responce<bool>> UpdateCityOnServer(CityDTO cityDto);
        Task<Responce<bool>> CreateCityOnServer(CityDTO cityDto);
        void DeleteCity();
    }
}