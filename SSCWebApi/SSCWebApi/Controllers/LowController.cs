using SSCWebApi.Helper;
using SSCWebApi.Models;
using SSCWebApi.Models.ModelDTO;
using SSCWebApi.Services.Implementation;
using SSCWebApi.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SSCWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Low")]
    public class LowController : ApiController
    {
        private readonly ILowService _lowService;
        public LowController()
        {
            _lowService = new LowService();
        }

        [Route("GetAllLows")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> GetAllLows(MobileRequest mobileRequest)
        {
            Responce<List<LowDTO>> responce = new Responce<List<LowDTO>>();
            try
            {
                responce = await _lowService.GetAllLows(mobileRequest);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetCityLows")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> GetCityLows(MobileRequest mobileRequest)
        {
            Responce<List<LowDTO>> responce = new Responce<List<LowDTO>>();
            try
            {
                responce = await _lowService.GetCityLows(mobileRequest);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
