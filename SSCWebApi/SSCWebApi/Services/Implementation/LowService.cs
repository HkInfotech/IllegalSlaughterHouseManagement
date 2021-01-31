using SSCWebApi.Helper;
using SSCWebApi.Models;
using SSCWebApi.Models.ModelDTO;
using SSCWebApi.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SSCWebApi.Services.Implementation
{
    public class LowService : ILowService
    {
        private SSCEntities SSCEntities;
        public LowService()
        {
            SSCEntities = new SSCEntities();

        }
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<Responce<List<LowDTO>>> GetAllLows(MobileRequest mobileRequest)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            Responce<List<LowDTO>> Responce = new Responce<List<LowDTO>>();
            Responce.Success = true;
            try
            {
                List<LowDTO> lows = new List<LowDTO>();

                lows = (from lo in SSCEntities.Lows
                        select lo).Select(a=>new LowDTO() {
                            Id=a.Id,
                            LowsTitile = a.LowsTitile,
                            LowsDescriptions = a.LowsDescriptions,
                            IsActive = a.IsActive,
                            IsDelete = a.IsDelete,
                            CreatedBy = a.CreatedBy,
                            CreatedDate = a.CreatedDate,
                            ModifiedBy = a.ModifiedBy,
                            ModifiedDate = a.ModifiedDate, 
                        }).ToList();

                Responce.ResponeContent = lows;
                return Responce;
            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.Message = $"ERROR GetAllLows :{ex.InnerException}";
            }
            return Responce;
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<Responce<List<LowDTO>>> GetCityLows(MobileRequest mobileRequest)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            Responce<List<LowDTO>> Responce = new Responce<List<LowDTO>>();
            Responce.Success = true;
            try
            {
                if (mobileRequest.Id!=0)
                {
                    List<LowDTO> lows = new List<LowDTO>();
                    lows = (from lo in SSCEntities.Lows
                           // join clo in SSCEntities.CityLows on lo.Id equals clo.LowId
                            // where clo.CityId == mobileRequest.Id
                            select lo).Select(a => new LowDTO()
                            {
                                Id = a.Id,
                                LowsTitile = a.LowsTitile,
                                LowsDescriptions = a.LowsDescriptions,
                                IsActive = a.IsActive,
                                IsDelete = a.IsDelete,
                                CreatedBy = a.CreatedBy,
                                CreatedDate = a.CreatedDate,
                                ModifiedBy = a.ModifiedBy,
                                ModifiedDate = a.ModifiedDate,
                            }).ToList();

                    Responce.ResponeContent = lows;
                    return Responce;
                }
                else
                {
                    Responce.Success = false;
                    Responce.Message = "No City Found Please verified id";
                }
                
            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.Message = $"ERROR GetAllLows :{ex.InnerException}";
            }
            return Responce;
        }
    }
}