package com.okta.example.hookproject.controller;

import com.okta.example.hookproject.model.EmployeeBasicInfo;
import com.okta.example.hookproject.model.OktaHookResponse;
import com.okta.example.hookproject.model.inlineHookResponseObjects.*;
import com.okta.example.hookproject.model.inlineHookResponseObjects.Error;
import com.okta.example.hookproject.utility.*;
import org.json.JSONObject;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.client.RestTemplate;

import javax.servlet.http.HttpServletRequest;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;


@RestController
public class RegistrationInlineHookController {

    /*
     * Verify that the SSN supplied to Okta matches the one
     * we have stored in our DB before permitting registration
     */
    @PostMapping("/registration/dblookup")
    public OktaHookResponse accountEvents(HttpServletRequest request) {

        // Construct our response, which we will modify as we step through this method
        OktaHookResponse response = new OktaHookResponse();



        /*
         * 👇 Lab 7-2:
         * Review this code segment to understand what is happening.
         * (No modification necessary)
         *
         * 1. First, we use our utility function to parse through the
         * JSON payload of Okta's request to our external service
         * This function will convert the payload into a JSONObject (eventBody)
         *
         * For an example of what this payload looks like see:
         * https://developer.okta.com/docs/concepts/event-hooks/#sample-event-delivery-payload
         *
         * 2. Then we extract the information from the "data" entry in that JSONObject
         * and store it to the variable named data.  Notice that this entry is another JSONObject.
         *
         * 3. Finally, we extract the "login" entry from the data JSONObject.
         * Notice that this is a String that refers to the username.
         *
         */
        JSONObject eventBody = RequestConverter.httpToJSON(request);
        JSONObject userProfile = eventBody.getJSONObject("data").getJSONObject("userProfile");
        String username = userProfile.getString("login");
        /*
         * ☝️ End of review segment
         */

        // Check if we got a valid entry keyed on "ssn" in the JSONObject we stored in userProfile
        if (!userProfile.isNull("ssn")) {
            /*
             * 👇 Lab 7-2:
             * TODO: If we have a valid ssn entry, let's store it to a String named ssnFromOkta
             *  Modify the ssnFromOkta variable so that it stores this value.
             * The username and ssnFromOkta are then used to construct a new EmployeeBasicInfo object
             */
            String ssnFromOkta = "";
            EmployeeBasicInfo newEmployeeFromOkta = new EmployeeBasicInfo(username, ssnFromOkta);

            /*
             * 👇 Lab 7-2:
             * Review this code segment to understand what is happening.
             * (No modification necessary)
             * 1. First, we retrieve the employee info we have stored in our database using the username
             * 2. Then we parse through this information using our utility function.
             * This will create a new EmployeeBasicInfo object that stores the username and the SSN
             * from the database.
             * Last, we store the SSN from the database to a String variable named ssnFromDB
             */
            String employeeInfo = getEmployees(username);
            EmployeeBasicInfo employeeFromDB = EmployeeConverter.parseEmployeeInfo(employeeInfo);
            String ssnFromDB = employeeFromDB.getSsn();
            /*
             * ☝️ End of review segment
             */

            if(ssnFromOkta.equals(ssnFromDB)){
                // construct command
                Commands command1 = new Commands();
                List<Commands> commandsList = new ArrayList<>();

                /* 👇 Lab 7-2:
                 * TODO: Specify a command to add to our Commands object registration is allowed
                 * The type will be "com.okta.user.profile.update" since we will be updating this Okta user's
                 * profile.
                 * The value will set the user's SSN to the empty string so that we no longer store this
                 * information on Okta now that it has been verified.
                 * Finally, we will add our Command object to the response
                 */


            }

            else {
                // construct command
                Commands command1 = new Commands();
                List<Commands> commandsList = new ArrayList<>();

                /* 👇 Lab 7-2:
                 * TODO: Specify a command to add to our Commands object when registration is denied
                 * The type will be "com.okta.action.update" which is an action
                 * we use when specifying whether to create a new Okta user when importing
                 * users or matching them against existing Okta users.
                 * The value DENY registration since the SSNs did not match.
                 *
                 * Finally, we will add our Command object to the response
                 */




                // construct Error
                Error error = new Error();
                ErrorCauses errorCauses = new ErrorCauses();
                List<ErrorCauses> causesList = new ArrayList<>();
                /* 👇 Lab 7-2:
                 * TODO: Specify in the ErrorSummary that we could not add the registrant
                 *  Add the error to the payload
                 *
                 */


            }

        }
        else { // ssn does not exist in the payload.
            // construct Command that denies registration
            Commands command1 = new Commands();
            List<Commands> commandsList = new ArrayList<>();
            HashMap<String, String> value = new HashMap<>();
            value.put("registration", "DENY");
            command1.setValue(value);
            command1.setType("com.okta.action.update");
            commandsList.add(command1);

            // construct Error
            Error error = new Error();
            ErrorCauses errorCauses = new ErrorCauses();
            List<ErrorCauses> causesList = new ArrayList<>();
            errorCauses.setErrorSummary("The request payload was not in the expected format. SSN is required.");
            errorCauses.setReason("INVALID_PAYLOAD");
            error.setErrorSummary("Invalid request payload");
            causesList.add(errorCauses);
            error.setErrorCauses(causesList);


            // add Command and Error to the response
            response.setCommands(commandsList);
            response.setError(error);
        }


        System.out.println(response.toString());
        return response;

    }

    private String getEmployees(String username) {
        /*
         * 👇 Lab 7-2:
         * TODO: Set the value of URI to
         *  "http://localhost:8085/employees/search/findByUsername?username="
         *   concatenated with the username parameter that is passed into this method
         */
        final String URI = "";

        String employee = "";
        /*
         * 👇 Lab 7-2:
         * TODO: Instantiate a new RestTemplate object that will serve as our REST Client
         *  Pass our URI to the REST client using its getForObject() method
         * See https://www.baeldung.com/rest-template for documentation on RestTemplate
         */

        return employee;

    }

}

