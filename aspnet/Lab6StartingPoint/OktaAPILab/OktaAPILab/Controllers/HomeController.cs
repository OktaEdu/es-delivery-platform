using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OktaAPILab.Models;
// 👇 Lab 6-4: add the Okta.Sdk and Okta.Sdk.Configuration namespaces

namespace OktaAPILab.Controllers
{
    public class HomeController : Controller
    {
        // TODO: 👇 Lab 6.4: replace value with your Okta org URL 👇
        private readonly string _oktaUrl = "https://oktaice###.oktapreview.com"; 
        // TODO: 👇 Lab 6.4: replace value with your API token 👇
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

        /* 👇 Lab 6-4:
         * Update the PortalHome() method signature so that it is an asynchronous action
         */
        public IActionResult PortalHome()
        {
            /* 👇 Lab 6-4:
             * Update the the userId variable below so that 
             * it stores the userId from the Session 
             */
            string userId = "";


            /* 👇 Lab 6-4:
             * Define your OktaClientConfiguration
             * and instantiate a new instance of the OktaClient with
             * these configurations
             */
            // OktaClientConfiguration oktaConfig = "";


            try
            {
                /* 👇 Lab 6-4:
                * Try to get the list of the app data associated 
                * with the user in the current session
                */
                var appLinks = "";

                ViewBag.IsSuccessful = true;
            }
            /* 👇 Lab 6-4:
             * If the above results in an error,
             * we want to catch the exception
             * (change the type of the error to OktaApiException) 
             * and store the error's ErrorSummary property
             * (change what Viewbag.ErrorSummary stores)
             */
            catch (Exception oktaError)
            {
                ViewBag.ErrorSummary = oktaError;
                ViewBag.IsSuccessful = false;
            }

            return View();
        }

    }
}
