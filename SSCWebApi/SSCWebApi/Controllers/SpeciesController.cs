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
    [RoutePrefix("api/Species")]
    public class SpeciesController : ApiController
    {
        private readonly ISpeciesService _speciesService;
        public SpeciesController()
        {
            _speciesService = new SpeciesService();
        }

        [Route("GetAllSpecies")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetAllSpecies(MobileRequest mobileRequest)
        {
            Responce<List<SpeciesDTO>> responce = new Responce<List<SpeciesDTO>>();
            try
            {
                responce = await _speciesService.GetAllSpecies(mobileRequest);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
