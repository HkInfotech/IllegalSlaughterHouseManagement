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
    public interface ILowService
    {
        Task<Responce<List<LowDTO>>> GetAllLows(MobileRequest mobileRequest);
        Task<Responce<List<LowDTO>>> GetCityLows(MobileRequest mobileRequest);
    }
}
