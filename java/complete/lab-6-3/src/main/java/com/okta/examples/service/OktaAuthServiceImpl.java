package com.okta.examples.service;

import com.okta.authn.sdk.AuthenticationException;
import com.okta.authn.sdk.AuthenticationStateHandlerAdapter;
import com.okta.authn.sdk.client.AuthenticationClient;
import com.okta.authn.sdk.client.AuthenticationClients;
import com.okta.authn.sdk.resource.AuthenticationResponse;
import com.okta.examples.model.OktaAuthRequest;
//import com.okta.sdk.lang.Assert;
import com.okta.sdk.resource.ResourceException;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import javax.annotation.PostConstruct;

@Service
public class OktaAuthServiceImpl implements OktaAuthService {

    @Value("#{ @environment['okta.client.orgUrl'] }")
    private String orgUrl;

    private AuthenticationClient client;

    @PostConstruct
    public void setup() {
        client = AuthenticationClients.builder()
            .setOrgUrl(orgUrl)
            .build();
    }

    @Override
    public AuthenticationResponse authenticate(OktaAuthRequest oktaAuthRequest) throws AuthenticationException {

        return client.authenticate(
            oktaAuthRequest.getUsername(),
            oktaAuthRequest.getPassword(),
            null,
            new EmptyAuthenticationStateHandlerAdapter()
        );
    }

    class EmptyAuthenticationStateHandlerAdapter extends AuthenticationStateHandlerAdapter {

        @Override
        public void handleUnknown(AuthenticationResponse typedUnknownResponse) {}
    }
}
