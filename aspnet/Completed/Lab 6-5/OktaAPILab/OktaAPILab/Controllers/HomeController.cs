using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OktaAPILab.Models;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace OktaAPILab.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _oktaUrl = "https://oktaiceXXX.oktapreview.com";
        private readonly string _oktaApiToken = "abc123";

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> PortalHome()
        {
            string userId = HttpContext.Session.GetString("userId");

            // Setup OktaClientConfiguration 
            OktaClientConfiguration oktaConfig = new OktaClientConfiguration
            {
                OktaDomain = _oktaUrl,
                Token = _oktaApiToken
            };
            OktaClient oktaClient = new OktaClient(oktaConfig);

            try
            {
                var appLinks = await oktaClient.Users.ListAppLinks(userId).ToList();
                ViewBag.Apps = appLinks;
                ViewBag.IsSuccessful = true;
            }
            catch (OktaApiException oktaError)
            {
                ViewBag.ErrorSummary = oktaError.ErrorSummary;
                ViewBag.IsSuccessful = false;
            }

            return View();
        }

    }
}
