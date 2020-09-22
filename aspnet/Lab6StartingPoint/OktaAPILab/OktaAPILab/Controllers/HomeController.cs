using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OktaAPILab.Models;

namespace OktaAPILab.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _oktaUrl = "https://oktaice###.oktapreview.com";
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

        public IActionResult PortalHome()
        {
            string userId = "";

            // Setup OktaClientConfiguration 

            //
            try
            {
                var appLinks = "";

                ViewBag.IsSuccessful = true;
            }
            catch (Exception oktaError)
            {
                ViewBag.ErrorSummary = oktaError;
                ViewBag.IsSuccessful = false;
            }

            return View();
        }

    }
}
