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
    public class AccountService : IAccountService
    {
        public Responce<List<AppRolesDTO>> GetRoles()
        {
            Responce<List<AppRolesDTO>> responce = new Responce<List<AppRolesDTO>>();
            List<AppRolesDTO> appRolesDTOs = new List<AppRolesDTO>();
            try
            {
                responce.Success = true;
                using (SSCEntities db = new SSCEntities())
                {

                    appRolesDTOs = db.AspNetRoles.Select(a => new AppRolesDTO() { Id = a.Id, Name = a.Name }).ToList();
                    responce.ResponeContent = appRolesDTOs;
                }
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.Message = $"ERROR GetRoles :{ex.InnerException}";
                responce.ResponeContent = appRolesDTOs;
            }
            return responce;
        }

        public Responce<List<AppUsersDTO>> GetUsersList()
        {
            Responce<List<AppUsersDTO>> responce = new Responce<List<AppUsersDTO>>();
            List<AppUsersDTO> appUsersDTO = new List<AppUsersDTO>();
            try
            {
                responce.Success = true;
                using (SSCEntities db = new SSCEntities())
                {

                    appUsersDTO = db.UserDetailWithRole?.Select(a => new AppUsersDTO() { Id = a.Id, Name = a.Name, Email = a.Email, City = a.City, Phone = a.Phonenumber, RoleId = a.RoleId, UserRole = a.UserRole }).ToList();
                    responce.ResponeContent = appUsersDTO;
                }
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.Message = $"ERROR GetUsersList : {ex.InnerException}";
                responce.ResponeContent = appUsersDTO;
            }
            return responce;
        }

        public Responce<List<AppUsersDTO>> GetUsersListByCity(MobileRequest mobileRequest)
        {
            Responce<List<AppUsersDTO>> responce = new Responce<List<AppUsersDTO>>();
            List<AppUsersDTO> appUsersDTO = new List<AppUsersDTO>();
            try
            {
                responce.Success = true;
                using (SSCEntities db = new SSCEntities())
                {
                    List<UserDetailWithRole> UserList = new List<UserDetailWithRole>();
                    var Users = db.AspNetUsers.Find(mobileRequest.UserId);
                    var UserRoles = db.GetUserRole(mobileRequest.UserId).FirstOrDefault().UserRole;
                    bool UsersCityAdded = false;

                    foreach (Citys city in db.Citys.Where(a => a.AdminEmail == Users.UserName || UserRoles.Equals("Site Admin")).ToList())
                    {
                        if (Users.City.ToLower() == city.CityName.ToLower())
                            UsersCityAdded = true;

                        UserList = UserList.Concat(db.UserDetailWithRole.Where(a => a.City.ToLower() == city.CityName.ToLower()).ToList()).ToList();
                    }
                    
                    if (!UsersCityAdded)
                        UserList = UserList.Concat(db.UserDetailWithRole.Where(a => a.City.ToLower() == Users.City.ToLower()).ToList()).ToList();

                    appUsersDTO = UserList?.Select(a => new AppUsersDTO() { Id = a.Id, Name = a.Name, Email = a.Email, City = a.City, Phone = a.Phonenumber, RoleId = a.RoleId, UserRole = a.UserRole }).ToList();

                    //var Users = db.AspNetUsers.Find(mobileRequest.UserId);
                    //appUsersDTO = db.UserDetailWithRole.Where(a => a.City.ToLower() == Users.City.ToLower())?.Select(a => new AppUsersDTO() { Id = a.Id, Name = a.Name, Email = a.Email, City = a.City, Phone = a.Phonenumber, RoleId = a.RoleId, UserRole = a.UserRole }).ToList();
                    
                    responce.ResponeContent = appUsersDTO;
                }
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.Message = $"ERROR GetUsersList : {ex.InnerException}";
                responce.ResponeContent = appUsersDTO;
            }
            return responce;
        }

        public Responce<bool> UpdateUserRole(UpdateUserRoleRequest request)
        {
            Responce<bool> responce = new Responce<bool>();
            responce.Success = true;

            try
            {
                responce.Success = true;
                using (SSCEntities db = new SSCEntities())
                {
                    db.UpdateUserRole(request.RoleId, request.UserId);
                    responce.ResponeContent = true;
                }
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.Message = $"ERROR UpdateUserRole : {ex.InnerException}";
                responce.ResponeContent = false;
            }
            return responce;
        }
    }
}