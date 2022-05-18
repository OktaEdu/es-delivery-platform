using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreHooks.Contracts;
using NetCoreHooks.model;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Web.LayoutRenderers;

namespace NetCoreHooks.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        //private ILoggerService _logger;
        private readonly ILogger<EventController> _logger;

        /*
         * 👇 Lab 7-1: 
         * Set the value of VERIFICATION_HEADER to "x-okta-verification-challenge"
         */
        private const string VERIFICATION_HEADER = "";

        public EventController(ILogger<EventController> loggerService)
        {
            _logger = loggerService;
            _logger.LogDebug(1, "NLog injected into Event Controller");
        }

        [HttpGet]
        [Route("{*more}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get()
        {
            _logger.LogDebug("Event EndpointVerify entered.");
            /*
             * 👇 Lab 7-1: 
             *  TODO: Modify the response variable below so it refers to 
             *  a new VerificationResponse object
             */
            VerificationResponse response = null;

            /*
             *  Note what is happening in the following lines 
             *  (you do NOT need to modify this code):
             *  
             *  First we retrieve the value of the verification request header 
             *  and store it in a variable named verification.
             *  
             *  If it DOES NOT exist (null), we replace the null value with a message 
             *  that it was not found, log a warning, and return BadRequest
             *  
             *  If it DOES exist, we wrap the value in our VerificationObject variable (named response)
             *  and log a success.
             */
            string verification = Request.Headers[VERIFICATION_HEADER];
            if (verification == null)
            {
                verification = "header " + VERIFICATION_HEADER + " was not found in the Request Headers collection";
                _logger.LogWarning($"Event EndpointVerify BadRequest will be returned. {verification}");
                return BadRequest(verification); // return BadRequest if verification was null
            }
            response.Verification = verification;
            Debug.WriteLine("Verification: \n" + verification);
            _logger.LogDebug($"Event EndpointVerify suceeded: {response.Verification}");

            /* 👇 Lab 7-1: 
             * TODO: Pass in the VerificationResponse object (response) to the Ok() method
             */
            return Ok(); // return OK if verification request header returned a value
        }

        [HttpPost]
        [Route("user-account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post()
        {
            _logger.LogInformation("Event PostEvent action entered");
            OktaEvents oktaEvents = null;
            var reader = new StreamReader(Request.Body);
            var payLoad = await reader.ReadToEndAsync();
            Debug.WriteLine(payLoad);
            try
            {
                var parsedJson = JObject.Parse(payLoad);
                var desiredEvent = parsedJson
                    .SelectToken("data")
                    .SelectToken("events")
                    .FirstOrDefault();
                if (desiredEvent != null)
                {
                    oktaEvents = new OktaEvents();
                    /* 👇 Lab 7-1: 
                     * TODO: Get Parsed JSON stored in desiredEvent and assign
                     * these values to oktaEvents properties
                     */
                   



                    _logger.LogDebug($"Post Event Succeeded\n {oktaEvents.ToString()}");

                    
                    return Ok(oktaEvents); //return Ok IActionResult with your oktaEvents object
                }
                else
                {
                    _logger.LogWarning($"Event PostEvent detected null event. BadRequest will  be returned"); ;
                    return BadRequest(oktaEvents);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _logger.LogError($"Event PostEvent failed with error message: {ex.Message} - {ex.InnerException}");
                return BadRequest(oktaEvents);
            }
        }
    }
}
