using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace OktaAPILab.Services
{
    public interface IOktaUserService
    {
        Task<IRestResponse> GetAppLinksAsync(string userId, string oktaUrl, string oktaApiToken);
    }
}
