using SSCWebApi.Helper;
using SSCWebApi.Models;
using SSCWebApi.Models.ModelDTO;
using SSCWebApi.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSCWebApi.Services.Implementation
{
    public class CityService : ICityService
    {


        private SSCEntities SSCEntities;

        public CityService()
        {
            SSCEntities = new SSCEntities();
        }


        public async Task<Responce<List<CityLowsDTO>>> GetCitieLows(MobileRequest mobileRequest)

        {
            Responce<List<CityLowsDTO>> Responce = new Responce<List<CityLowsDTO>>();
            Responce.Success = true;
            try
            {

                List<CityLowsDTO> CityLows = new List<CityLowsDTO>();
                CityLows = SSCEntities.CityLows.Select(a => new CityLowsDTO()
                {
                    Id = a.Id,
                    CityId = a.CityId,
                    LowId = a.LowId
                }).ToList();
                Responce.ResponeContent = CityLows;
                return Responce;

            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.Message = $"ERROR GetCities :{ex.InnerException}";
            }
            return Responce;
        }


        public async Task<Responce<List<CityDTO>>> GetCities(MobileRequest mobileRequest)

        {

            Responce<List<CityDTO>> Responce = new Responce<List<CityDTO>>();
            Responce.Success = true;
            try
            {

                List<CityDTO> Citys = new List<CityDTO>();
                Citys = SSCEntities.Citys.Where(a => a.IsActive == true).Select(a => new CityDTO()
                {
                    Id = a.Id,
                    CityName = a.CityName,
                    CountryId = a.CountryId,
                    CreatedBy = a.CreatedBy,
                    AdminEmail = a.AdminEmail,
                    CreatedDate = a.CreatedDate,
                    ModifiedBy = a.ModifiedBy,
                    ModifiedDate = a.ModifiedDate,
                    MCEmail = a.MCEmail,
                    FcciEmail = a.FcciEmail,
                    IsActive = a.IsActive ?? false
                }).ToList();                
                Responce.ResponeContent = Citys;
                return Responce;

            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.Message = $"ERROR GetCities :{ex.InnerException}";
            }
            return Responce;
        }

        public async Task<Responce<bool>> UpdateCityMail(ChnageCityMailRequest chnageCityMailRequest)
        {
            Responce<bool> Responce = new Responce<bool>();
            Responce.Success = true;
            try
            {
                using (var db = new SSCEntities())
                {
                    Citys citys = db.Citys.Find(chnageCityMailRequest.Cityid);
                    citys.MCEmail = chnageCityMailRequest.MCEmail;
                    citys.FcciEmail = chnageCityMailRequest.FCCIEmail;
                    db.Entry(citys).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    Responce.ResponeContent = true;
                }

            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.ResponeContent = false;
                Responce.Message = $"ERROR UpdateCityMail :{ex.InnerException}";

            }
            return Responce;
        }

        public async Task<Responce<bool>> UpsertCity(CityDTO model)
        {
            Responce<bool> Responce = new Responce<bool>();
            Responce.Success = true;
            try
            {
                using (var db = new SSCEntities())
                {

                    if (model.IsCreate) 
                    {
                        
                    }
                    else
                    {

                    }

                    var IsAdminEmailExist = db.AspNetUsers.Where(a => a.Email.ToLower() == model.AdminEmail.ToLower())?.ToList();
                    if (IsAdminEmailExist.Count > 0)
                    {
                        DateTime dateTime = DateTime.UtcNow;
                        Citys citys = db.Citys.Find(model.Id) ?? new Citys();
                        citys.MCEmail = model.MCEmail;
                        citys.FcciEmail = model.FcciEmail;
                        citys.CityName = model.CityName;
                        citys.IsActive = model.IsActive;
                        citys.AdminEmail = model.AdminEmail;
                        if (citys.Id == 0)
                        {
                            citys.CreatedBy = model.CreatedBy;
                            citys.CreatedDate = dateTime;
                            citys.ModifiedBy = model.ModifiedBy;
                            citys.ModifiedDate = dateTime;
                            db.Entry(citys).State = System.Data.Entity.EntityState.Added;
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            citys.ModifiedBy = model.ModifiedBy;
                            citys.ModifiedDate = dateTime;
                            citys.ModifiedBy = model.ModifiedBy;
                            citys.ModifiedDate = DateTime.UtcNow;
                            db.Entry(citys).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }

                        Responce.ResponeContent = true;

                    }
                    else
                    {
                        Responce.ResponeContent = false;
                        Responce.Success = false;
                        Responce.Message = "Admin email is not register";
                    }



                }

            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.ResponeContent = false;
                Responce.Message = $"ERROR UpsertCity :{ex.InnerException}";

            }
            return Responce;
        }

        public async Task<Responce<List<CityDTO>>> GetAllCities(MobileRequest mobileRequest)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {

            Responce<List<CityDTO>> Responce = new Responce<List<CityDTO>>();
            Responce.Success = true;
            try
            {

                List<CityDTO> Citys = new List<CityDTO>();
                Citys = SSCEntities.Citys?.Select(a => new CityDTO()
                {
                    Id = a.Id,
                    CityName = a.CityName,
                    CountryId = a.CountryId,
                    CreatedBy = a.CreatedBy,
                    AdminEmail = a.AdminEmail,
                    CreatedDate = a.CreatedDate,
                    ModifiedBy = a.ModifiedBy,
                    ModifiedDate = a.ModifiedDate,
                    MCEmail = a.MCEmail,
                    FcciEmail = a.FcciEmail,
                    IsActive = a.IsActive ?? false
                }).ToList();
                Responce.ResponeContent = Citys;
                return Responce;

            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.Message = $"ERROR GetCities :{ex.InnerException}";
            }
            return Responce;
        }
    }
}