package com.okta.examples.controller;

import com.okta.authn.sdk.resource.AuthenticationResponse;
import com.okta.examples.service.OktaAuthService;
import com.okta.sdk.authc.credentials.TokenClientCredentials;
import com.okta.sdk.client.Client;
import com.okta.sdk.client.Clients;
//import com.okta.sdk.lang.Assert;
import com.okta.sdk.resource.ResourceException;
import com.okta.sdk.resource.user.User;
import com.okta.sdk.resource.user.UserBuilder;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.servlet.ModelAndView;

import javax.annotation.PostConstruct;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
/* TODO: import your OktaAuthRequest class */
@Controller
public class AccountController {

    @Value("#{ @environment['okta.client.orgUrl'] }")
    private String orgUrl;

    @Value("#{ @environment['okta.client.token'] }")
    private String apiToken;

    private OktaAuthService oktaAuthService;
    private Client client;

    /* 👇 Lab 6-1:
     * TODO: Add a one argument constructor that sets the value of the
     *  oktaAuthService property
     */

    @PostConstruct
    void setup() {
        /* TODO: Add code to construct a client instance by passing it your Okta domain name and API token  */
    }

    @GetMapping("/register")
    public String register() {
        return "register";
    }

    @PostMapping("/register")
    /* TODO: Add ModelAttribute annotation and OktaAuthRequest parameter */

    public ModelAndView doRegister() {
        Map<String, String> regResponse = new HashMap<>();
        /* TODO: Try to create a new user. Catch any exception that occurs */

        return new ModelAndView("register", regResponse);
    }

    @GetMapping("/login")
    public String login() {
        return "login";
    }

    @PostMapping("/login")
    /* 👇 Lab 6-1:
     * TODO: Add an additional OktaAuthRequest parameter to the doLogin method signature
     *  It should be the first param and be annotated with @ModelAttribute
     */
    public ModelAndView doLogin(
        HttpServletRequest request,
        HttpServletResponse response) throws IOException {
        Map<String, String> authResponse = new HashMap<>();

        try {
            /* 👇 Lab 6-2:
             * TODO: Pass the oktaAuthRequest to the OktaAuthService and extract
             *  the Status and SessionToken from the response. Put this data in authResponse.
             */


            /* 👇 Lab 6-2:
             * TODO: Pass the oktaAuthRequest to the OktaAuthService and extract
             *  the Status and SessionToken from the response. Put this data in authResponse.
             *
             * Later in 👇 Lab 6-3:
             * TODO: Comment out the return statement in the try block
             *  Instead, we will initiate a session and associate the userId
             */


            /* 👇 Lab 6-3:
             * TODO: Build the URL string for directing the user to our Okta subdomain
             * This will consist of our orgUrl, the /login/sessionCookieRedirect endpoint,
             * a query string parameter (token), that gets the session token from the Okta Auth Response,
             * and another query string parameter (redirectUrl), which gets our portal URL.
             * Finally, we will send this redirect URL as part of our response.
             */




        }
        catch (Exception e) {
            /* 👇 Lab 6-1:
             * TODO: Store the error message in authResponse if we encounter an exception
             */

        }

        return new ModelAndView("login", authResponse);
    }
}
