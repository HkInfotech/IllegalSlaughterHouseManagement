using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSCMobileDataBus.OfflineStore.Models.Commons;
using SSCMobileServiceBus.OnlineSync.Models.ComplaintModelDTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSCMobile.Helpers
{
    public class RestApiHelpers
    {
        private HttpClient httpClient;
        private readonly IAppSettings _settings;

        public RestApiHelpers()
        {
            _settings = DependencyService.Get<IAppSettings>();
            httpClient = new HttpClient()
            {
                Timeout = new TimeSpan(0, 10, 0)
            };
        }

        public async Task<bool> RequestTokenAsync(string UserName, string Password)
        {
            string accessToken = string.Empty;
            try
            {
                var Body = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type","password"),
                    new KeyValuePair<string, string>("Username",UserName),
                    new KeyValuePair<string, string>("password",Password)
                };
                string UrlRequest = App.UsLocalService == true ? App.LocalhostUrl : _settings.BaseUrl;
                UrlRequest = UrlRequest + EndPoint.Login.Token;

                var request = new HttpRequestMessage(HttpMethod.Post, UrlRequest);
                request.Content = new FormUrlEncodedContent(Body);

                var responce = await httpClient.SendAsync(request).ConfigureAwait(false);
                var result = await responce.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject results = JObject.Parse(result);
                if (results != null)
                {
                    TokenResponce tokenResponce = new TokenResponce();
                    tokenResponce = JsonConvert.DeserializeObject<TokenResponce>(result);
                    if (tokenResponce.access_token != null)
                    {
                        _settings.Token = tokenResponce.access_token;
                        _settings.IsLogin = true;
                        _settings.UserName = tokenResponce.userName;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                Debug.WriteLine($"LOGIN RESPONCE : {results}");
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<T> PosyAsync<T>(string endPoint, string json, string baseUrl = null, bool isLocalHost = false)
        {
            T objectResponce = default(T);
            httpClient = new HttpClient();
            try
            {
                isLocalHost = App.UsLocalService;
                var url = isLocalHost ? App.LocalhostUrl : !string.IsNullOrEmpty(baseUrl) ? baseUrl : _settings.BaseUrl;
                var UrlRequest = $"{url}{endPoint}";
                Debug.WriteLine($"POST ASYNC:{UrlRequest}");
                HttpRequestMessage httpcontent = new HttpRequestMessage(HttpMethod.Post, UrlRequest);
                HttpContent contentpost = new StringContent(json, Encoding.UTF8, "application/json");

                if (!string.IsNullOrEmpty(_settings.Token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _settings.Token);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                httpcontent.Headers.ExpectContinue = false;
                httpcontent.Content = contentpost;

                HttpResponseMessage responce = await httpClient.SendAsync(httpcontent).ConfigureAwait(false);
                if (responce != null)
                {
                    using (var stream = await responce.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var reader = new StreamReader(stream))
                    {
                        string strResponce = reader.ReadToEnd();
                        var JObj = JObject.Parse(strResponce);
                        JsonSerializerSettings jt = new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        };
                        objectResponce = JsonConvert.DeserializeObject<T>(JObj.ToString(), jt);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error On Rest Api Call:{ex}");
                throw ex;
            }

            return objectResponce;
        }

        public async Task<bool> SaveComplaint(string endPoint, List<KeyValuePair<string, string>> ListOfParameters, List<byte[]> Images, string baseUrl = null, bool isLocalHost = false)
        {
            httpClient = new HttpClient();
            try
            {
                isLocalHost = App.UsLocalService;
                var url = isLocalHost ? App.LocalhostUrl : !string.IsNullOrEmpty(baseUrl) ? baseUrl : _settings.BaseUrl;
                var UrlRequest = $"{url}{endPoint}";
                Debug.WriteLine($"POST ASYNC:{UrlRequest}");
                HttpRequestMessage httpcontent = new HttpRequestMessage(HttpMethod.Post, UrlRequest);
                var contentpost = new MultipartFormDataContent();
                if (!string.IsNullOrEmpty(_settings.Token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _settings.Token);
                foreach (var item in Images)
                {
                    string filename = Guid.NewGuid() + ".jpg";
                    Stream stream = new MemoryStream(item);
                    contentpost.Add(new StreamContent(stream), "\"file\"", filename);
                }
                foreach (var keyValuePair in ListOfParameters)
                {
                    contentpost.Add(new StringContent(keyValuePair.Value),
                        String.Format("\"{0}\"", keyValuePair.Key));
                }
                httpcontent.Headers.ExpectContinue = false;
                httpcontent.Content = contentpost;

                HttpResponseMessage responce = await httpClient.SendAsync(httpcontent).ConfigureAwait(false);
                if (responce != null)
                {
                    if (responce.IsSuccessStatusCode)
                    {
                        using (var stream = await responce.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        using (var reader = new StreamReader(stream))
                        {
                            string strResponce = reader.ReadToEnd();
                            var JObj = JObject.Parse(strResponce);
                            JsonSerializerSettings jt = new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            };
                            var objectResponce = JsonConvert.DeserializeObject<Responce<ComplaintsDTO>>(JObj.ToString(), jt);
                            return true;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public async Task<T> GetAsync<T>(string endpoint, bool isLocalhost = false, bool isLoggingIn = false) where T : new()
        {
            T objResponse;
            string uriRequest = "";

            try
            {
                //Configures the Request
                uriRequest = FormatUriRequest(endpoint);
                Debug.WriteLine("GET ASYNC: " + uriRequest);
                HttpRequestMessage httpContent = new HttpRequestMessage(HttpMethod.Get, uriRequest);
                if (!string.IsNullOrEmpty(_settings.Token))
                    httpContent.Headers.Authorization = new AuthenticationHeaderValue("bearer", _settings.Token);
                httpContent.Headers.ExpectContinue = false;

                //Sends the request and reads the response
                HttpResponseMessage response = await httpClient.SendAsync(httpContent);
                objResponse = await GetResponseAsync<T>(response, uriRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in REST CALL(JSON): url'{uriRequest}' {ex}");

                objResponse = GetErrorResponse<T>(ex);
            }

            return objResponse;
        }
        protected string FormatUriRequest(string endpoint, string baseUrl = null)
        {
            bool isLocalhost = App.UsLocalService;
            string url = isLocalhost ? App.LocalhostUrl : !string.IsNullOrEmpty(baseUrl) ? baseUrl : _settings.BaseUrl;
            string uriRequest = $"{url}{endpoint}";

            return uriRequest;
        }

        private async Task<T> GetResponseAsync<T>(HttpResponseMessage response, string uriRequest) where T : new()
        {
            T objResponse = default;
            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var reader = new StreamReader(stream))
                    {
                        string strResponse = reader.ReadToEnd();
                        var jobj = JObject.Parse(strResponse);

                        objResponse = JsonConvert.DeserializeObject<T>(jobj.ToString());
                    }
                }
                else
                {
                    Debug.WriteLine($"Error in REST CALL(JSON): url'{uriRequest}' {response.ReasonPhrase}");
                    objResponse = GetErrorResponse<T>(message: $"url'{uriRequest}' {response.ReasonPhrase}");
                }
            }
            else
            {
                objResponse = GetErrorResponse<T>(message: "Invalid Response.");
            }

            return objResponse;
        }

        private T GetErrorResponse<T>(Exception ex = null, string message = null) where T : new()
        {
            Debug.WriteLine($"Error in REST CALL(JSON): {ex}");
            T obj = new T();
            Type type = obj.GetType();
            var errorResponse = (T)Activator.CreateInstance(type);

            if (ex != null)
            {
                message = message != null ? $"{message}; {ex.Message}" : ex.Message;
                TrySetProperty(errorResponse, "StackTrace", ex.StackTrace);
            }
            if (message != null)
            {
                TrySetProperty(errorResponse, "Message", message);
            }

            TrySetProperty(errorResponse, "Success", false);

            return errorResponse;
        }

        protected void TrySetProperty(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, value, null);
        }
    }
}