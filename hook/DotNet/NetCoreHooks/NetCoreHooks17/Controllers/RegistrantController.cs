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

            //grab http request body
            var reader = new StreamReader(Request.Body);
            var payLoad = await reader.ReadToEndAsync();
            Debug.WriteLine($"payLoad received: \n{payLoad}");

            
            if (payLoad != null) //make sure you have a valid payload
            {
                
                var parsedJson = JObject.Parse(payLoad); //convert incoming http request to JSON JObject

                /* 👇 Lab 7-2: 
                 * TODO: Modify the userProfile variable so that
                 * it gets the user profile data from the parsed JSON
                 */
                JObject userProfile = null;

                /* 👇 Lab 7-2: 
                 * TODO: Now extract the username from the userProfile
                 * and save it to the String variable named userName
                 */

                 

            
                if (userProfile.ContainsKey("ssn")) // this verifies that the SSN key exists before we extract the value
                {
                    /* 👇 Lab 7-2: 
                     * TODO: Modify the ssnFromOkta variable so that it
                     * stores the extracted SSN value from userProfile
                     */
                    ssnFromOkta = "";
                    Debug.WriteLine($"ssnFromOkta: {ssnFromOkta}");
                }
                else // ssn key does not exist in the payload. DENY registration and return an Error in response
                {
                    response = new OktaHookResponse();
                    Dictionary<String, String> dict = new Dictionary<string, string>
                    {
                        { "registration", "DENY" }
                    };

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

                
                // get information we have stored for this user from our database so we can compare to the info we got from Okta
                var Registrant = await _simulatedRemoteDBComponent.FindByUserName(userName);
                var RegistrantDTO = _mapper.Map<RegistrantDTO>(Registrant);
                if (RegistrantDTO != null)
                {
                    ssnFromDatabase = RegistrantDTO.SSN; // this is the ssn we got from our database
                }
                else // we didn't have any info for that user stored in our database (got null)
                {
                    // So, we can't verify the SSN!
                    // Deny registration. Create new Error and return it in a response.
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

                
                if (ssnFromOkta == ssnFromDatabase) // Does the SSN the user supplied from Okta match the one from our DB? 
                {
                    
                    response = new OktaHookResponse(); // It matched, so now construct a response back to Okta
                    Command command = new Command();
                    /* 👇 Lab 7-2: 
                    * TODO: Construct a command to add to our Command object
                    * The type will be "com.okta.user.profile.update" since we will be updating this Okta user's 
                    * profile.  
                    * The value will set the user's SSN to the empty string so that we no longer store this 
                    * information on Okta now that it has been verified.
                    * Finally, we will add our Command object to the response
                    */



                    Debug.WriteLine("SSN match detected. Returing 200 OK\n");
                    Debug.WriteLine("Response to send back to Okta:\n " + response);

                    //The happy day scenario. We got a good payload with an SSN
                    //which matched the SSN in the remote system. Return a Command
                    //object with an "ssn" value of String.Empty
                    return Ok(response);
                }
                else // SSNs did not match! Disallow registration. Create new Error object with Error Causes
                {
                    
                    response = new OktaHookResponse();
                    

                    Command command = new Command();
                    /* 👇 Lab 7-2: 
                    * TODO: Construct a command to add to our Command object
                    * The type will be "com.okta.action.update" which is an action 
                    * we use when specifying whether to create a new Okta user when importing
                    * users or matching them against existing Okta users.
                    * The value DENY registration since the SSNs did not match.
                    * 
                    * Finally, we will add our Command object to the response
                    */


                    Error error = new Error();
                    /* 👇 Lab 7-2: 
                    * TODO: Specify in the ErrorSummary that we could not add the registrant
                    * Leave the ErrorCauses empty
                    */


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
