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
    public interface IAccountService
    {
        Responce<List<AppRolesDTO>> GetRoles();

        Responce<List<AppUsersDTO>> GetUsersList();
        Responce<bool> UpdateUserRole(UpdateUserRoleRequest request);

        Responce<List<AppUsersDTO>> GetUsersListByCity(MobileRequest mobileRequest);
    }
}
