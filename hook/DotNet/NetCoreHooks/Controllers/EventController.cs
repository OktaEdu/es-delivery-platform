using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreHooks.Contracts;
using NetCoreHooks.model;
using Newtonsoft.Json.Linq;
using NLog;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreHooks.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private ILoggerService _logger;
        private const string VERIFICATION_HEADER = "x-Okta-Verification-Challenge";
        

        public EventController(ILoggerService loggerService)
        {
            _logger = loggerService;
        }

        [HttpPost]
        [Route("user-account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post()
        {
            _logger.LogInfo("Event PostEvent action entered");
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
                    oktaEvents.EventType = desiredEvent["eventType"].ToString();
                    oktaEvents.DisplayMessage = desiredEvent["displayMessage"].ToString();
                    oktaEvents.EventTime = parsedJson["eventTime"].ToString();
                    _logger.LogInfo($"Post Event Succeeded with {oktaEvents.DisplayMessage} at {oktaEvents.EventTime}");
                    return Ok(oktaEvents);
                }
                else
                {
                    _logger.LogWarn($"Event PostEvent detected null event. BadRequest will  be returned"); ;
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

        [HttpGet]
        [Route("{*more}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get()
        {
            _logger.LogInfo("Event EndpointVerify entered.");            
            VerificationResponse response = new VerificationResponse();
            string verification = Request.Headers[VERIFICATION_HEADER]; 
            if (verification == null)
            {
                verification = "header " + VERIFICATION_HEADER + " was not found in the Request Headers collection";
                _logger.LogWarn($"Event EndpointVerify BadRequest will be returned. {verification}");
                return BadRequest(verification);
            }

            response.Verification = verification;
            Debug.WriteLine("Verification: \n" + verification);
            _logger.LogInfo($"Event EndpointVerify suceeded: {response.Verification}");
            return Ok(response);
        }
    }
}
