package com.okta.examples.controller;

import com.okta.authn.sdk.resource.AuthenticationResponse;
import com.okta.examples.model.OktaAuthRequest;
import com.okta.examples.service.OktaAuthService;
import com.okta.sdk.authc.credentials.TokenClientCredentials;
import com.okta.sdk.client.Client;
import com.okta.sdk.client.Clients;
import com.okta.sdk.lang.Assert;
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
    }

    @GetMapping("/register")
    public String register() {
        return "register";
    }

    @PostMapping("/register")
    public ModelAndView doRegister(@ModelAttribute OktaAuthRequest oktaAuthRequest) {
        Assert.notNull(oktaAuthRequest);
        Assert.notNull(oktaAuthRequest.getUsername());
        Assert.notNull(oktaAuthRequest.getPassword());

        Map<String, String> regResponse = new HashMap<>();

        return new ModelAndView("register", regResponse);
    }

    @GetMapping("/login")
    public String login() {
        return "login";
    }

    @PostMapping("/login")
    public ModelAndView doLogin(
        @ModelAttribute OktaAuthRequest oktaAuthRequest, HttpServletRequest request, HttpServletResponse response
    ) throws IOException {
        Map<String, String> authResponse = new HashMap<>();

        return new ModelAndView("login", authResponse);
    }
}
