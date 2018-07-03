using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OktaAPILab.Models.AccountViewModels;
using RestSharp;

namespace OktaAPILab.Services
{
    public class OktaAuthService : IOktaAuthService
    {
        RestClient httpClient;
        
        public async Task<IRestResponse> AuthenticateAsync(OktaAuthRequest authRequestBody, string oktaUrl, string oktaApiToken)
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", "SSWS " + oktaApiToken);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(authRequestBody);

            httpClient = new RestClient(oktaUrl + "/api/v1/authn");
            IRestResponse authResponse = httpClient.Execute(request);
            return authResponse;
        }
    }
}
