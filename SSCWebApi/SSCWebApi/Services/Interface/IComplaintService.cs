using SSCWebApi.Helper;
using SSCWebApi.Models;
using SSCWebApi.Models.ModelDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SSCWebApi.Services.Interface
{
    public interface IComplaintService
    {
        Task<Responce<ComplaintsDTO>> SaveComplaint(HttpRequest  complaintRequest, string UserName);
        Task<Responce<List<ComplaintsDTO>>> GetComplaint(MobileRequest mobileRequest);
    }
}
