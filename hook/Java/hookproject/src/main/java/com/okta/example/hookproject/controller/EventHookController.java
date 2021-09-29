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

    //Display events that begin with "user.account"
    @PostMapping("/event/user-account")
    public OktaEvents accountEvents(HttpServletRequest request) {
        /* TODO: Parse the events from the HTTP Request */

        /* TODO: Update the Strings with values extracted from the HTTP request */
        String eventType = "";
        String displayMessage = "";
        String eventTime = "";

        OktaEvents oktaEvents = new OktaEvents(eventType, displayMessage, eventTime);
        System.out.println(oktaEvents.toString());
        return oktaEvents;

    }

    //Verify endpoint ownership
    @GetMapping("/event/*")
    public VerificationResponse endpointVerify(HttpServletRequest request) {
        /* TODO: Declare and assign headerName String */

        /* TODO: Update the verification String value */
        String verification = "";
        VerificationResponse response = new VerificationResponse();
        response.setVerification(verification);

        System.out.println(verification);

        return response;
    }
}
