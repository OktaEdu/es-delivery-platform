using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OktaAPILab.Models;
using Microsoft.AspNetCore.Http;
using OktaAPILab.Services;
using OktaAPILab.Models.HomeViewModels;
using RestSharp;
using Newtonsoft.Json;

namespace OktaAPILab.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOktaUserService _oktaUserService;
        private readonly string oktaUrl = "https://oktaice###.oktapreview.com";
        private readonly string oktaApiToken = "abc123";

        public HomeController(IOktaUserService oktaUserService)
        {
            _oktaUserService = oktaUserService;
        }

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
            IRestResponse oktaHttpResponse = await _oktaUserService.GetAppLinksAsync(userId, oktaUrl, oktaApiToken);

            ViewBag.IsSuccessful = oktaHttpResponse.IsSuccessful;
            if(oktaHttpResponse.IsSuccessful)
            {
                List<OktaAppLink> appLinks = JsonConvert.DeserializeObject<List<OktaAppLink>>(oktaHttpResponse.Content);
                ViewBag.apps = appLinks;
            }
            else
            {
                OktaError error = JsonConvert.DeserializeObject<OktaError>(oktaHttpResponse.Content);
                ViewBag.ErrorSumary = "Error Summary: " + error.errorSummary;
            }

            return View();
        }
    }
}
