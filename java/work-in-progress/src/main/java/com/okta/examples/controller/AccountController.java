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
    public ModelAndView doLogin(
        HttpServletRequest request, HttpServletResponse response
    ) throws IOException {
        Map<String, String> authResponse = new HashMap<>();

        try {
            AuthenticationResponse oktaAuthResponse =
                    oktaAuthService.authenticate();
            authResponse.put(
                    "Status",
                    "Status: " + oktaAuthResponse.getStatusString()
            );
//            authResponse.put(
//                    "SessionToken",
//                    "Session Token: " + oktaAuthResponse.getSessionToken()
//            );
        } catch (Exception e) {
            authResponse.put(
                    "ErrorSummary", "Error: " + e.getMessage()
            );
        }

        return new ModelAndView("login", authResponse);
    }
}
