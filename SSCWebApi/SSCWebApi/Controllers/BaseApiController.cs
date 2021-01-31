using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;

namespace SSCWebApi.Controllers
{
    public class BaseApiController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
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

        #endregion
    }
}
