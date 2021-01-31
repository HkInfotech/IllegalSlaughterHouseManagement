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
    public class SpeciesService : ISpeciesService
    {
        private SSCEntities SSCEntities;

        public SpeciesService()
        {
            SSCEntities = new SSCEntities();
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<Responce<List<SpeciesDTO>>> GetAllSpecies(MobileRequest mobileRequest)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            Responce<List<SpeciesDTO>> Responce = new Responce<List<SpeciesDTO>>();
            Responce.Success = true;
            try
            {
                List<SpeciesDTO> species = new List<SpeciesDTO>();

                species = (from lo in SSCEntities.Species
                           select lo).Select(a => new SpeciesDTO()
                           {
                               Id = a.Id,
                               SpeciesName = a.SpeciesName,
                               Icon = a.Icon,
                               CreatedBy = a.CreatedBy,
                               CreatedDate = a.CreatedDate,
                               ModifiedBy = a.ModifiedBy,
                               ModifiedDate = a.ModifiedDate,
                           }).ToList();

                Responce.ResponeContent = species;
                return Responce;
            }
            catch (Exception ex)
            {
                Responce.Success = false;
                Responce.Message = $"ERROR GetAllSpecies :{ex.InnerException}";
            }
            return Responce;
        }
    }
}