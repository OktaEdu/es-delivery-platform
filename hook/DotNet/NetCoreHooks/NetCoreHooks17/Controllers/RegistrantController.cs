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
            
            // Construct our response, which we will modify as we step through this method
            OktaHookResponse response = new OktaHookResponse();


            /*
            * 👇 Lab 7-2:
            * Review this code segment to understand what is happening.
            * (No modification necessary)
            *
            * 1. First we read the characters from the request body asynchronously 
            * until the end and store it as a single string.
            * 
            * 2. Then we check if the request body is valid (not null)
            * 
            * 3. If the request body is valid, we parse the JSON and convert it to a JObject
            *
            * 4. Then we extract the information from the "data" entry in that JObject
            * and extract the information from the "userProfile" entry in the "data" object.
            * We store this information to it to the variable named userProfile.  
            * Notice that this entry is another JObject.
            * 
            * 5. Finally, we extract the "login" entry from the data JObject.
            * Notice that this is a string that refers to the username.
            *
            */
            var reader = new StreamReader(Request.Body);
            var payLoad = await reader.ReadToEndAsync();
            Debug.WriteLine($"payLoad received: \n{payLoad}");
            var parsedJson = JObject.Parse(payLoad);
                JObject userProfile = (JObject)parsedJson
                    .SelectToken("data")
                    .SelectToken("userProfile");
                string userName = userProfile["login"].ToString().ToLower();
            /*
            * ☝️ End of review segment
            */



            // Check if we got a valid entry keyed on "ssn" in the JObject we stored in userProfile
            if (userProfile.ContainsKey("ssn")) 
            {
                /* 👇 Lab 7-2: 
                * TODO: If we have a valid ssn entry, 
                *  Modify the ssnFromOkta variable so that it
                * stores the extracted SSN value from userProfile
                * ignoring any dashes the user may have entered
                */
                string ssnFromOkta = "";
                Debug.WriteLine($"ssnFromOkta: {ssnFromOkta}");


                /*
                * 👇 Lab 7-2:
                * Review this code segment to understand what is happening.
                * (No modification necessary)
                * 1. First, we retrieve the employee info we have stored in our database using the username
                * 2. Then we map this information to a RegistrantDTO object so we can access its properties
                * (e.g., RegistrantDTO.SSN will get the ssn associated with the registrant)
                */
                var Registrant = await _simulatedRemoteDBComponent.FindByUserName(userName);
                var RegistrantDTO = _mapper.Map<RegistrantDTO>(Registrant);
                /*
                * ☝️ End of review segment
                */


                if (RegistrantDTO != null && ssnFromOkta == Registrant.SSN.Replace("-", ""))
                {
                    Command allowAndResetSSN = new Command();
                    /* 👇 Lab 7-2: 
                    * TODO: Construct a command to add to our Command object
                    * The type will be "com.okta.user.profile.update" since we will be updating this Okta user's 
                    * profile.  
                    * The value will set the user's SSN to the empty string so that we no longer store this 
                    * information on Okta now that it has been verified.
                    * Finally, we will add our Command object to the response
                    */
                    

                    return Ok(response);
                }

                else // no ssn match
                {
                    Debug.WriteLine("No SSN Match");
                    
                    
                    Command denyRegNoMatch = new Command();
                    /* 👇 Lab 7-2:
                    * TODO: Specify a command to add to our Commands object
                    * The type will be "com.okta.action.update" since we 
                    * The value will be to set registration to "DENY" since the SSN did not match
                    * Finally, we will add this command to our response
                    */



                    Error error = new Error();
                    ErrorCause errorCauses = new ErrorCause();
                    List<ErrorCause> causesList = new List<ErrorCause> {};
                    /* 👇 Lab 7-2:
                    * TODO: Specify in the ErrorSummary that we could not add the registrant
                    *  Add the error to the payload
                    */


                    return Ok(response);
                }
            }


            else // ssn key does not exist in the payload.        
            {
                // construct Command that denies registration
                Command denyRegNoSSN = new Command();
                Dictionary<String, String> value = new Dictionary<string, string>
                {
                    { "registration", "DENY" }
                };
                denyRegNoSSN.value = value;
                denyRegNoSSN.type = "com.okta.action.update";

                // construct Error
                Error error = new Error();
                ErrorCause errorCauses = new ErrorCause();
                List<ErrorCause> causesList = new List<ErrorCause> { };
                errorCauses.ErrorSummary = "The request payload was not in the expected format. SSN is required.";
                errorCauses.Reason = "INVALID_PAYLOAD";
                error.ErrorSummary = "Unable to add registrant";
                causesList.Add(errorCauses);
                error.ErrorCauses = causesList;

                // add Command and Error to the response
                response.commands.Add(denyRegNoSSN);
                response.Error = error;
                return Ok(response);
            }
        }
    }
}
