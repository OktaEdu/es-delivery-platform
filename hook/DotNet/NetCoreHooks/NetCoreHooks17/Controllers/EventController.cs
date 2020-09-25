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
        
        //Set the value of the constant to "x-Okta-Verification-Challenge"
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
            /*Instantiate VerificationResponse object*/
            VerificationResponse response = null;

            string verification = Request.Headers[VERIFICATION_HEADER];
            if (verification == null)
            {
                verification = "header " + VERIFICATION_HEADER + " was not found in the Request Headers collection";
                _logger.LogWarning($"Event EndpointVerify BadRequest will be returned. {verification}");
                return BadRequest(verification);
            }

            response.Verification = verification;
            Debug.WriteLine("Verification: \n" + verification);
            _logger.LogDebug($"Event EndpointVerify suceeded: {response.Verification}");
            return Ok();
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


                    
                    _logger.LogDebug($"Post Event Succeeded\n {oktaEvents.ToString()}");

                    //return Ok IActionResult with your oktaEvents object
                    return Ok(oktaEvents);
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
