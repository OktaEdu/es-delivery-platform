package com.okta.examples.service;

import com.okta.authn.sdk.AuthenticationException;
import com.okta.authn.sdk.resource.AuthenticationResponse;
import com.okta.examples.model.OktaAuthRequest;

public interface OktaAuthService {
    AuthenticationResponse authenticate(OktaAuthRequest oktaAuthRequest)
            throws AuthenticationException;
}
