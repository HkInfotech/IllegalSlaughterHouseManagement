using SSCWebApi.Helper;
using SSCWebApi.Models;
using SSCWebApi.Models.ModelDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSCWebApi.Services.Interface
{
    public interface ICityService
    {

        Task<Responce<List<CityDTO>>> GetCities(MobileRequest mobileRequest);
        Task<Responce<List<CityLowsDTO>>> GetCitieLows(MobileRequest mobileRequest);
        Task<Responce<bool>> UpdateCityMail(ChnageCityMailRequest chnageCityMailRequest);

        Task<Responce<bool>> UpsertCity(CityDTO model);
        Task<Responce<List<CityDTO>>> GetAllCities(MobileRequest mobileRequest);
    }
}
