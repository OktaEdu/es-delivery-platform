using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace OktaAPILab.Services
{
    public class OktaUserService : IOktaUserService
    {
        RestClient httpClient;

        public async Task<IRestResponse> GetAppLinksAsync(string userId, string oktaUrl, string oktaApiToken)
        {
            httpClient = new RestClient(oktaUrl + "/api/v1/users/" + userId + "/appLinks");
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", "SSWS " + oktaApiToken);
            request.AddHeader("Accept", "application/json");

            IRestResponse response = httpClient.Execute(request);
            return response;
        }
    }
}
