package com.okta.examples.service;

import com.okta.examples.model.OktaAppLink;

import java.util.List;

public interface OktaUserService extends OktaBaseService {

    List<OktaAppLink> getAppLinks(String userId);
}
