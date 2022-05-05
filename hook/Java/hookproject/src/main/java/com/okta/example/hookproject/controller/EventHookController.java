package com.okta.example.hookproject.controller;

import com.okta.example.hookproject.model.OktaEvents;
import com.okta.example.hookproject.model.VerificationResponse;
import com.okta.example.hookproject.utility.*;
import org.json.JSONArray;
import org.json.JSONObject;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

import javax.servlet.http.HttpServletRequest;
import java.io.BufferedReader;
import java.io.IOException;


@RestController
public class EventHookController {

    // Display events that begin with "user.account"
    @PostMapping("/event/user-account") // Okta Hook
    public OktaEvents accountEvents(HttpServletRequest request) {
        /*
         * 👇 Lab 7-1:
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
         * 3. Finally, we extract the "events" entry from the data JSONObject.
         * Notice that this is a JSONArray. We will use the events JSONArray
         * to access pertinent String data from the response.
         *
         */
        JSONObject eventBody = RequestConverter.httpToJSON(request);
        JSONObject data = eventBody.getJSONObject("data");
        JSONArray events = (JSONArray) data.get("events");
        /*
         * ☝️ End of review segment
         */



        /*
         * 👇 Lab 7-1:
         * TODO: Update the Strings below with values extracted from events JSONArray
         *   retrieved from the HTTP request.
         * We want to store the event type, the display message, and the time event was published.
         * The keys for these entries are eventType, displayMessage, and published.
         *
         * For an example JSON payload of a request from Okta to your external service see:
         * https://developer.okta.com/docs/concepts/event-hooks/#sample-event-delivery-payload
         *
         * We will pass these values to our OktaEvent model and log the details.
         */
        String eventType = "";
        String displayMessage = "";
        String eventTime = "";

        OktaEvents oktaEvents = new OktaEvents(eventType, displayMessage, eventTime);
        System.out.println(oktaEvents.toString());
        return oktaEvents;

    }

    // Verify endpoint ownership
    @GetMapping("/event/*")
    public VerificationResponse endpointVerify(HttpServletRequest request) {
        /*
         * 👇 Lab 7-1:
         * TODO: Set the value of VERIFICATION_HEADER to "x-okta-verification-challenge"
         */
        final String VERIFICATION_HEADER = "";

        /*
         * 👇 Lab 7-1:
         * TODO: Set the value of the verification variable to the value from the request header
         *  that is keyed on our VERIFICATION_HEADER
         * This will be used in the subsequent lines to set the verification value in the response
         */
        String verification = "";
        VerificationResponse response = new VerificationResponse();
        response.setVerification(verification); // set verification in the response

        System.out.println(verification);

        return response;
    }
}
