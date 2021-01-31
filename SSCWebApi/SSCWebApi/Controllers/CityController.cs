using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SSCWebApi.Helper;
using SSCWebApi.Models;
using SSCWebApi.Models.ModelDTO;
using SSCWebApi.Services.Implementation;
using SSCWebApi.Services.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSCWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/City")]
    public class CityController : ApiController
    {
        private readonly ICityService _cityService;
        private ApplicationUserManager _userManager;
        private const string LocalLoginProvider = "Local";

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public CityController()
        {
            _cityService = new CityService();
        }

        [Route("GetCities")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetCities(MobileRequest mobileRequest)
        {
            Responce<List<CityDTO>> responce = new Responce<List<CityDTO>>();
            try
            {
                responce = await _cityService.GetCities(mobileRequest);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetAllCities")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetAllCities(MobileRequest mobileRequest)
        {
            Responce<List<CityDTO>> responce = new Responce<List<CityDTO>>();
            try
            {
                responce = await _cityService.GetAllCities(mobileRequest);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetCitieLows")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetCitieLows(MobileRequest mobileRequest)
        {
            Responce<List<CityLowsDTO>> responce = new Responce<List<CityLowsDTO>>();
            try
            {
                responce = await _cityService.GetCitieLows(mobileRequest);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateCityMail")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpdateCityMail(ChnageCityMailRequest chnageCityMailRequest)
        {
            Responce<bool> responce = new Responce<bool>();
            try
            {
                responce = await _cityService.UpdateCityMail(chnageCityMailRequest);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateCityMail")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpsertCity(CityDTO citydto)
        {
            Responce<bool> responce = new Responce<bool>();
            try
            {

                responce = await _cityService.UpsertCity(citydto);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("CreateCity")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> CreateCity(CityDTO citydto)
        {
            Responce<bool> responce = new Responce<bool>();
            try
            {

                var IsUserExist = await UserManager.FindByNameAsync(citydto.AdminEmail);

                if (IsUserExist != null)
                {
                    var Errors = "User email already registered.";
                    responce.Fail(Errors);
                    responce.ResponeContent = false;
                    return Content(HttpStatusCode.BadRequest, responce);
                }

                using (var db = new SSCEntities())
                {

                    var CheckCity = db.Citys.Where(a => a.CityName.ToLower() == citydto.CityName.ToLower())?.ToList() ?? new List<Citys>();
                    if (CheckCity.Count > 0)
                    {
                        responce.ResponeContent = false;
                        responce.Success = false;
                        responce.Message = "City already added";
                        return Content(HttpStatusCode.BadRequest, responce);
                    }

                    DateTime dateTime = DateTime.UtcNow;

                    Citys citys = new Citys();
                    citys.MCEmail = citydto.MCEmail;
                    citys.FcciEmail = citydto.FcciEmail;
                    citys.CityName = citydto.CityName;
                    citys.IsActive = citydto.IsActive;
                    citys.AdminEmail = citydto.AdminEmail;
                    citys.CreatedBy = citydto.CreatedBy;
                    citys.CreatedDate = dateTime;
                    citys.ModifiedBy = citydto.ModifiedBy;
                    citys.ModifiedDate = dateTime;
                    db.Entry(citys).State = System.Data.Entity.EntityState.Added;
                    await db.SaveChangesAsync();

                    var lows = db.Lows.ToList();
                    SSCEntities sscEntities = new SSCEntities();
                    foreach (var item in lows)
                    {
                        CityLows cityLows = new CityLows();
                        cityLows.Id = citys.Id;
                        cityLows.LowId = item.Id;
                        sscEntities.Entry(cityLows).State = System.Data.Entity.EntityState.Added;
                        await sscEntities.SaveChangesAsync();

                    }

                    var user = new ApplicationUser() { UserName = citydto.AdminEmail, Email = citydto.AdminEmail, City = citydto.CityName, PhoneNumber = citydto.MobileNo, Name = citydto.UserName, IsDefaultAdmin = true, CityId = citys.Id };

                    IdentityResult result = await UserManager.CreateAsync(user, citydto.Password);
                    if (!result.Succeeded)
                    {
                        var Errors = GetIdentityErrorResult(result);
                        responce.Fail(Errors);
                        responce.ResponeContent = false;

                        return Content(HttpStatusCode.BadRequest, responce);
                    }

                    db.UpdateUserRole("989D2937-0FC5-497D-980C-B2E8583602B9", user.Id);                    
                    responce.ResponeContent = true;

                    responce.Success = true;
                }


                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("UpdateCity")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpdateCity(CityDTO citydto)
        {
            Responce<bool> responce = new Responce<bool>();
            try
            {


                using (var db = new SSCEntities())
                {
                    var CheckCity = db.Citys.Find(citydto.Id);



                    if (CheckCity == null)
                    {
                        responce.ResponeContent = false;
                        responce.Success = false;
                        responce.Message = "This city should not exist";
                        return Content(HttpStatusCode.BadRequest, responce);
                    }
                    var GetDefaultCityAdminUser = db.AspNetUsers.Where(a => a.IsDefaultAdmin == true && a.CityId == citydto.Id)?.FirstOrDefault();
                    if (GetDefaultCityAdminUser != null)
                    {
                        var appUser = await UserManager.FindByNameAsync(GetDefaultCityAdminUser.UserName);
                        if (appUser != null)
                        {
                            if (appUser.UserName.ToLower() != citydto.AdminEmail.ToLower())
                            {

                                appUser.UserName = citydto.AdminEmail;
                                appUser.Email = citydto.AdminEmail;
                                //appUser.City = citydto.CityName;
                                //appUser.CityId = citydto.Id;

                                IdentityResult result = await UserManager.UpdateAsync(appUser);
                                if (!result.Succeeded)
                                {
                                    var Errors = GetIdentityErrorResult(result);
                                    responce.Fail(Errors);
                                    responce.ResponeContent = false;
                                    return Content(HttpStatusCode.BadRequest, responce);
                                }

                            }
                        }
                    }




                    DateTime dateTime = DateTime.UtcNow;

                    Citys citys = CheckCity;
                    citys.MCEmail = citydto.MCEmail;
                    citys.FcciEmail = citydto.FcciEmail;
                    citys.CountryId = citydto.CountryId ?? 1;
                    citys.CityName = citydto.CityName;
                    citys.IsActive = citydto.IsActive;
                    citys.AdminEmail = citydto.AdminEmail;
                    citys.ModifiedBy = citydto.ModifiedBy;
                    citys.ModifiedDate = dateTime;
                    db.Entry(citys).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    List<AspNetUsers> aspNetUsers = new List<AspNetUsers>();
                    aspNetUsers = db.AspNetUsers.Where(a => a.CityId == citys.Id).ToList();

                    foreach (var item in aspNetUsers)
                    {
                        item.City = citys.CityName;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    responce.Success = true;
                    responce.ResponeContent = true;
                }
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public string GetIdentityErrorResult(IdentityResult result)
        {
            try
            {
                List<string> errors = new List<string>();
                if (result == null)
                {
                    errors.Add("500 Internal Server Error");
                }
                if (!result.Succeeded)
                {
                    if (result.Errors != null)
                    {
                        foreach (string error in result.Errors)
                        {
                            errors.Add(error);
                            ModelState.AddModelError("", error);
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        // No ModelState errors are available to send, so just return an empty BadRequest.
                        errors.Add("Bad Request");
                    }
                    var ErrorResult = string.Join(",", errors);
                    return ErrorResult;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error:{ex.InnerException}");
                return string.Empty;
            }
        }
    }
}
