﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using SSCWebApi.Helper;
using SSCWebApi.Models;
using SSCWebApi.Models.ModelDTO;
using SSCWebApi.Providers;
using SSCWebApi.Results;
using SSCWebApi.Services.Implementation;
using SSCWebApi.Services.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace SSCWebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private readonly IAccountService _accountService;

        public AccountController()
        {
            _accountService = new AccountService();
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

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

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public async Task<UserInfoViewModel> GetUserInfoAsync()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            string City = "";
            string UserId = "";
#pragma warning disable CS0219 // The variable 'CityId' is assigned but its value is never used
            string CityId = "";
#pragma warning restore CS0219 // The variable 'CityId' is assigned but its value is never used
            string Name = "";
            string PhoneNumber = "";
            var Email = User.Identity.GetUserName();
            var UserRole = await UserManager.GetRolesAsync(User.Identity.GetUserId());
            using (SSCEntities db = new SSCEntities())
            {
                var User = db.AspNetUsers.Where(a => a.UserName == Email)?.FirstOrDefault();
                if (User != null)
                {
                    City = User.City;
                    UserId = User.Id;
                    PhoneNumber = User.PhoneNumber;
                    Name = User.Name;
                }
            }

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null,
                City = City,
                UserId = UserId,
                PhoneNumber = PhoneNumber,
                Name = Name,
                UserRole = UserRole.FirstOrDefault()
            };
        }

        // POST api/Account/UpdateuserInfo
        [Route("UpdateUserInfo")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserInfo(UpdateUserInfoRequest model)
        {
            Responce<UserInfoViewModel> responce = new Responce<UserInfoViewModel>();
            responce.Success = true;
            try
            {
                string City = "";
                string UserId = "";
#pragma warning disable CS0219 // The variable 'CityId' is assigned but its value is never used
                string CityId = "";
#pragma warning restore CS0219 // The variable 'CityId' is assigned but its value is never used
                string Name = "";
                string PhoneNumber = "";
                var UserRole = await UserManager.GetRolesAsync(User.Identity.GetUserId());
                //IdentityUser user = await UserManager.FindByIdAsync(model.UserId);
                using (SSCEntities db = new SSCEntities())
                {
                    var AspNetUser = db.AspNetUsers.Find(model.UserId);
                    AspNetUser.City = model.City;
                    AspNetUser.Name = model.Name;
                    AspNetUser.PhoneNumber = model.PhoneNumber;
                    AspNetUser.CityId = model.CityId;
                    db.Entry(AspNetUser).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    City = AspNetUser.City;
                    UserId = AspNetUser.Id;
                    PhoneNumber = AspNetUser.PhoneNumber;
                    Name = AspNetUser.Name;

                    responce.ResponeContent = new UserInfoViewModel
                    {
                        Email = AspNetUser.Email,
                        City = City,
                        UserId = UserId,
                        PhoneNumber = PhoneNumber,
                        Name = Name,
                        UserRole = UserRole.FirstOrDefault()
                    };
                }
                return Ok(responce);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        //// POST api/Account/ChangePassword
        //[Route("ChangePassword")]
        //public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
        //        model.NewPassword);

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterRequest model)
        {
            Responce<RegistrationDTO> responce = new Responce<RegistrationDTO>();
            int CityId = 0;
            responce.Success = true;
            RegistrationDTO registrationDTO = new RegistrationDTO();
            if (!ModelState.IsValid)
            {
                var Errors = GetModelStateError(ModelState);

                responce.Fail(Errors);
                return Content(HttpStatusCode.BadRequest, responce);
            }
            SSCEntities db = new SSCEntities();
            if (model.Cityid == 0)
            {
                var UserCity = db.Citys.Where(a => a.CityName.ToLower() == model.City.ToLower()).FirstOrDefault();
                CityId = UserCity.Id;
            }
            else
            {
                CityId = model.Cityid;
            }
            //var UserCity = db.Citys.Where(a => a.CityName.ToLower() == model.City.ToLower()).FirstOrDefault();
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, City = model.City, PhoneNumber = model.PhoneNumber, Name = model.Name, CityId = CityId };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var Errors = GetIdentityErrorResult(result);
                responce.Fail(Errors);
                return Content(HttpStatusCode.BadRequest, responce);
            }
            else
            {
                IdentityResult assignroleresult = await UserManager.AddToRoleAsync(user.Id, "Volunteer");

                if (!assignroleresult.Succeeded)
                {
                    var Errors = GetIdentityErrorResult(assignroleresult);
                    responce.Fail(Errors);
                    return Content(HttpStatusCode.BadRequest, responce);
                }
                else
                {
                    registrationDTO.Name = user.Name;
                    registrationDTO.Email = user.Email;
                    registrationDTO.City = user.City;
                    registrationDTO.PhoneNumber = user.PhoneNumber;
                    registrationDTO.Password = model.Password;
                    responce.ResponeContent = registrationDTO;
                }
            }

            return Ok(responce);
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // POST api/Account/ForgotPassword
        [Route("ForgotPassword")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            Responce<bool> responce = new Responce<bool>();
            responce.Success = true;
            try
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    user.OTP = common.RandomNumber();
                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        var HtmlBody = $"<html>" +
                            $"<title>Password Reset Detail</title>" +
                            $"<body>" +
                            $"<div>" +
                            $"<p>Hi {user.Name}</p>" +
                            $"<p>You recently requested to reset your password for your <b>Stop Slaughter Cruelty</b> account. Your OTP is <b> {user.OTP } </b> " +
                            $" </p>" +
                            $"<br />" +
                            $"<br />" +
                            $"Thank You <br />" +
                            $"The Stop Slaughter Cruelty Team" +
                            $"</div>" +
                            $"<body>" +
                            $"</html>";
                        common.SendMail("Reset Password Details", HtmlBody, user.Email, "support@resquark.com");
                        responce.ResponeContent = true;
                        responce.Success = true;
                        return Ok(responce);
                    }
                    else
                    {
                        responce.Success = false;
                        responce.ResponeContent = false;
                        return Content(HttpStatusCode.BadRequest, responce);
                    }
                }
                return Content(HttpStatusCode.BadRequest, responce);
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.ResponeContent = false;

                responce.Message = $"ERROR ForgotPassword :{ex.ToString()}";
                return Content(HttpStatusCode.BadRequest, responce);
            }
        }

        // POST api/Account/ForgotPassword
        [Route("ChangePassword")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ChangePassword(ForgotPasswordRequest model)
        {
            Responce<bool> responce = new Responce<bool>();
            responce.Success = true;
            try
            {
                var user = await UserManager.FindByNameAsync(model.Email);

                if (user != null)
                {
                    if (string.Equals(model.OTP, user.OTP) == true)
                    {
                        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                        IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
                        //user.OTP = string.Empty;
                        //await UserManager.UpdateAsync(user);
                        if (!result.Succeeded)
                        {
                            List<string> errors = new List<string>();
                            foreach (var item in result.Errors)
                            {
                                errors.Add(item);
                            }
                            var Allerrors = string.Join(",", errors);
                            responce.ResponeContent = false;
                            responce.Fail(Allerrors);
                            return Content(HttpStatusCode.BadRequest, responce);
                        }
                        responce.ResponeContent = true;
                        responce.Success = true;
                        return Ok(responce);
                    }
                    else
                    {
                        responce.Success = false;
                        responce.ResponeContent = false;
                        return Content(HttpStatusCode.BadRequest, responce);
                    }
                }
                else
                {
                    responce.Success = false;
                    responce.ResponeContent = false;
                    return Content(HttpStatusCode.BadRequest, responce);
                }
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.ResponeContent = false;

                responce.Message = $"ERROR ChangePassword :{ex.ToString()}";
                return Content(HttpStatusCode.BadRequest, responce);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public string GetModelStateError(ModelStateDictionary modelstatevalues)
        {
            try
            {
                List<string> errors = new List<string>();
                foreach (var modelStateVal in modelstatevalues.Values)
                {
                    foreach (var error in modelStateVal.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                        // You may log the errors if you want
                    }
                }
                var result = string.Join(",", errors);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error:{ex.InnerException}");
                return "";
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

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion Helpers

        [Route("GetRoles")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult GetRoles()
        {
            Responce<List<AppRolesDTO>> responce = new Responce<List<AppRolesDTO>>();
            try
            {
                responce = _accountService.GetRoles();
                return Ok(responce);
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.Message = $"ERROR GetRoles : {ex.InnerException}";
                responce.ResponeContent = new List<AppRolesDTO>();

                return Content(HttpStatusCode.BadRequest, responce);
            }
        }

        [Route("GetUsersList")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult GetUsersList()
        {
            Responce<List<AppUsersDTO>> responce = new Responce<List<AppUsersDTO>>();
            try
            {
                responce = _accountService.GetUsersList();
                return Ok(responce);
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.Message = $"ERROR GetUsersList : {ex.InnerException}";
                responce.ResponeContent = new List<AppUsersDTO>();

                return Content(HttpStatusCode.BadRequest, responce);
            }
        }

        [Route("GetUsersListByCity")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult GetUsersListByCity(MobileRequest mobileRequest)
        {
            Responce<List<AppUsersDTO>> responce = new Responce<List<AppUsersDTO>>();
            try
            {
                responce = _accountService.GetUsersListByCity(mobileRequest);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.Message = $"ERROR GetUsersList : {ex.InnerException}";
                responce.ResponeContent = new List<AppUsersDTO>();

                return Content(HttpStatusCode.BadRequest, responce);
            }
        }


        [Route("UpdateUserRole")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult UpdateUserRole(UpdateUserRoleRequest request)
        {
            Responce<bool> responce = new Responce<bool>();
            try
            {
                responce = _accountService.UpdateUserRole(request);
                return Ok(responce);
            }
            catch (Exception ex)
            {
                responce.Success = false;
                responce.Message = $"ERROR UpdateUserRole : {ex.InnerException}";
                responce.ResponeContent = false;

                return Content(HttpStatusCode.BadRequest, responce);
            }
        }
    }
}