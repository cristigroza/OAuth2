using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IdentityModel.Client;

namespace TripGallery.MVCClient.Helpers
{
    public static class TripGalleryHttpClient
    {

        public static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();

            var accesToken = RequestAccessTokenClientCredentials();
            client.BaseAddress = new Uri(Constants.TripGalleryAPI);
            client.SetBearerToken(accesToken);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private static string RequestAccessTokenClientCredentials()
        {
            var cookie = HttpContext.Current.Request.Cookies.Get("TripGalleryCookie");
            if (cookie != null && cookie["access_token"] != null)
            {
                return cookie["access_token"];
            }

            var tokenClient = new TokenClient(
                Constants.TripGallerySTSTokenEndpoint,
                "tripgalleryclientcredentials",
                Constants.TripGalleryClientSecret);
            var tokenResponse = tokenClient.RequestClientCredentialsAsync("gallerymanagement").Result;
            TokenHelper.DecodeAndWrite(tokenResponse.AccessToken);

            HttpContext.Current.Response.Cookies["TripGalleryCookie"]["access_token"] = tokenResponse.AccessToken;

            return tokenResponse.AccessToken;
        }
    }
}