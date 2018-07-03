using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OktaAPILab.Models.AccountViewModels;
using RestSharp;

namespace OktaAPILab.Services
{
    public interface IOktaAuthService
    {
        Task<IRestResponse> AuthenticateAsync(OktaAuthRequest authRequestBody, string oktaUrl, string oktaApiToken);
    }
}
