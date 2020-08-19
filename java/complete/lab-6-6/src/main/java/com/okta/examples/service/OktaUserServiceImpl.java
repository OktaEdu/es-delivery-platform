package com.okta.examples.service;

import com.okta.examples.model.OktaAppLink;
import com.okta.sdk.lang.Assert;
import org.apache.http.client.fluent.Request;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.io.InputStream;
import java.util.List;

@Service
public class OktaUserServiceImpl implements OktaUserService {

    private final Logger logger = LoggerFactory.getLogger(OktaUserServiceImpl.class);

    @Value("#{ @environment['okta.client.orgUrl'] }")
    private String orgUrl;

    @Value("#{ @environment['okta.client.token'] }")
    private String apiToken;

    @Override
    public List<OktaAppLink> getAppLinks(String userId) {
        try {
            InputStream is = Request.Get(
                orgUrl + "/api/v1/users/" + userId + "/appLinks"
            )
                .addHeader("Cache-Control", "no-cache")
                .addHeader("Authorization", "SSWS " + apiToken)
                .addHeader("Accept", "application/json")
                .execute().returnContent().asStream();
            return mapper.readValue(is, List.class);
        } catch (IOException e) {
            logger.error(
                "Unable to get appLinks: {}", e.getMessage(), e
            );
            return null;
        }
    }
}
