using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCoreHooks.Contracts;
using NetCoreHooks.DTOs;
using NetCoreHooks.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace NetCoreHooks.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly IRegistrantRepository _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IRegistrantRepository _simulatedRemoteDBComponent;

        public RegistrationController(ILoggerService loggerService,
            IRegistrantRepository RegistrantRepository, IMapper mapper,
            IConfiguration configuration, IRegistrantRepository registrantRepository)
        {
            _logger = loggerService;
            _db = RegistrantRepository;
            _mapper = mapper;
            _config = configuration;
            _simulatedRemoteDBComponent = registrantRepository;
        }


        [HttpPost("dbLookup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyRegistrantSSNByUserName()
        {
            string ssnFromOkta = String.Empty;
            string userName = String.Empty;
            string ssnFromDatabase = String.Empty;
            OktaHookResponse response = null;

            //grab http reques body
            var reader = new StreamReader(Request.Body);
            var payLoad = await reader.ReadToEndAsync();
            Debug.WriteLine($"payLoad received: \n{payLoad}");

            //make sure you have a good payload
            if (payLoad != null)
            {
                //convert incoming http request to JSon JObject
                var parsedJson = JObject.Parse(payLoad);

                //get SSN from payload via Linq-to-JSon
                JObject userProfile = null;

                /*did you remember to include SSN in Okta User Profile? 
                 * if you didn't, the ssn key will be absent.
                */                
                if (userProfile.ContainsKey("ssn"))
                {
                    //get SSN Okta sent us
                    ssnFromOkta = "";
                    Debug.WriteLine($"ssnFromOkta: {ssnFromOkta}");
                }
                else
                {
                    //couldn't detect SSN from payload. Create new Error object with Error Causes
                    response = new OktaHookResponse();
                    Dictionary<String, String> dict = new Dictionary<string, string>
                    {
                        { "registration", "DENY" }
                    };

                    Command command = new Command();
                    


                    Error error = new Error();
                    error.ErrorSummary = "Unable to add registrant";
                    error.ErrorCauses = new List<ErrorCause>
                    {
                        new ErrorCause{ErrorSummary = "SSN is Required",
                            Domain="end-user", Location="data.UserProfile.login", Reason="SSN could not be verified"}
                    };
                    response.Error = error;
                    Debug.WriteLine(response);
                    return Ok(response);
                }

                //get ssn from database for this user
                var Registrant = await _simulatedRemoteDBComponent.FindByUserName(userName);
                var RegistrantDTO = _mapper.Map<RegistrantDTO>(Registrant);

                if (RegistrantDTO != null)
                {
                    ssnFromDatabase = RegistrantDTO.SSN;
                }
                else
                {
                    //couldn't get data from the simulated remote system (which is really a local in-memory database)
                    //Deny registration. Create new Error object with Error Causes
                    response = new OktaHookResponse();
                    Dictionary<String, String> dict = new Dictionary<string, string>
                    {
                        { "registration", "DENY" }
                    };

                    Command command = new Command();
                    command.type = "com.okta.action.update";
                    command.value = dict;
                    response.commands.Add(command);
                    Error error = new Error();
                    error.ErrorSummary = "Unable to add registrant";
                    error.ErrorCauses = new List<ErrorCause>
                    {
                        new ErrorCause{ErrorSummary = "Unable to convert Registrant to RegistrantDTO",
                            Domain="end-user",
                            Location="data.UserProfile.login",
                            Reason="Unable to convert Registrant"}
                    };
                    response.Error = error;
                    Debug.WriteLine("unable to convert Registrant to RegistrantDTO");
                    return Ok(error);
                }

                Debug.WriteLine(ssnFromDatabase);

                //do the SSNs match? 
                if (ssnFromOkta == ssnFromDatabase)
                {
                    //you have a match, now construct your response back to Okta
                    response = new OktaHookResponse();
                    Dictionary<String, String> dict = new Dictionary<string, string>
                    {
                        { "ssn", String.Empty }
                    };

                    Command command = new Command();
                    command.type = "com.okta.user.profile.update";
                    command.value = dict;
                    response.commands.Add(command);

                    Debug.WriteLine("SSN match detected. Returing 200 OK\n");
                    Debug.WriteLine("Response to send back to Okta:\n " + response);

                    //The happy day scenario. We got a good payload with an SSN
                    //which matched the SSN in the remote system. Return a Command
                    //object with an "ssn" value of String.Empty
                    return Ok(response);
                }
                else
                {
                    //no match. Disallow registration. Create new Error object with Error Causes
                    response = new OktaHookResponse();
                    Dictionary<String, String> dict = new Dictionary<string, string>
                    {
                        { "registration", "DENY" }
                    };

                    Command command = new Command();
                    command.type = "com.okta.action.update";
                    command.value = dict;
                    response.commands.Add(command);
                    Error error = new Error();
                    error.ErrorSummary = "Unable to add registrant";
                    error.ErrorCauses = new List<ErrorCause>
                    {
                        

                    };
                    response.Error = error;
                    return Ok(response);
                }
            }
            else
            {
                //payload came over null. 
                //Create new Error object with Error Causes
                response = new OktaHookResponse();
                Dictionary<String, String> dict = new Dictionary<string, string>
                {
                    { "registration", "DENY" }
                };

                Command command = new Command();
                command.type = "com.okta.action.update";
                command.value = dict;
                response.commands.Add(command);

                Error error = new Error();
                error.ErrorSummary = "Payload not detected";
                error.ErrorCauses = new List<ErrorCause>
                    {
                        new ErrorCause{ErrorSummary = "No payload detected ",
                            Domain="end-user", Location="data.UserProfile.login",
                            Reason="Payload arrived null at API"}
                    };
                response.Error = error;
                return Ok(response);
            }
        }
    }
}
