package com.okta.examples.service;

import com.okta.authn.sdk.AuthenticationException;
import com.okta.authn.sdk.AuthenticationStateHandlerAdapter;
import com.okta.authn.sdk.client.AuthenticationClient;
import com.okta.authn.sdk.client.AuthenticationClients;
import com.okta.authn.sdk.resource.AuthenticationResponse;
//import com.okta.sdk.lang.Assert;
import com.okta.sdk.resource.ResourceException;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import javax.annotation.PostConstruct;
/* 👇 Lab 6-1:
 * TODO: import the OktaAuthRequest model
 */

@Service
public class OktaAuthServiceImpl implements OktaAuthService {

    @Value("#{ @environment['okta.client.orgUrl'] }")
    private String orgUrl;

    private AuthenticationClient client;

    @PostConstruct
    public void setup() {
        /* 👇 Lab 6-1:
         * Define the authentication client using your orgUrl
         */
    }

    /* 👇 Lab 6-1:
     * TODO: Implement the authenticate() method declared in the OktaAuthService interface
     * @param oktaAuthRequest: an OktaAuthRequest object
     * @return: an AuthenticationResponse object
     * @throws: an AuthenticationException object
     *
     * This method will get the username and password captured in the OktaAuthRequest object
     * and pass it to our client.
     * If no error is encountered, the client returns an AuthenticationResponse.
     */



    class EmptyAuthenticationStateHandlerAdapter extends AuthenticationStateHandlerAdapter {

        @Override
        public void handleUnknown(AuthenticationResponse typedUnknownResponse) {}
    }
}
