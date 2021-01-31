using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSCMobile.Commonfiles
{
    class HkImageHelper
    {
        readonly Lazy<HttpClient> _clientHolder = new Lazy<HttpClient>(() => CreateHttpClient(TimeSpan.FromSeconds(10)));
        static HttpClient CreateHttpClient(TimeSpan timeout)
        {
            HttpClient client;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                case Device.Android:
                    client = new HttpClient();
                    break;
                default:
                    client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
                    break;

            }
            client.Timeout = timeout;
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return client;
        }
        HttpClient Client => _clientHolder.Value;

        public async Task<byte[]> DownloadImage(string imageUrl)
        {
            if (!imageUrl.Contains("http"))
            {
                // OnImageDownloadFailed("URL must use https");
                return null;
            }

            byte[] downloadedImage;

            try
            {
                using (var httpResponse = await Client.GetAsync(imageUrl).ConfigureAwait(false))
                {
                    if (httpResponse.StatusCode is HttpStatusCode.OK)
                    {
                        downloadedImage = await httpResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        return downloadedImage;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }
    }
}
