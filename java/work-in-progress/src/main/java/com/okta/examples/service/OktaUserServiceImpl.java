package com.okta.examples.service;

//import com.okta.sdk.lang.Assert;
import org.apache.http.client.fluent.Request;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.io.InputStream;
import java.util.Collections;
import java.util.List;
/* 👇 Lab 6-4:
 * TODO: Import the OktaAppLink class
 */

@Service
public class OktaUserServiceImpl implements OktaUserService {

    private final Logger logger = LoggerFactory.getLogger(OktaUserServiceImpl.class);

    @Value("#{ @environment['okta.client.orgUrl'] }")
    private String orgUrl;

    @Value("#{ @environment['okta.client.token'] }")
    private String apiToken;

    /* 👇 Lab 6-4:
     * TODO: Implement the abstract method getAppLinks() inherited from OktaUserService
     * @param String userId
     * @return List<OktaAppLink>
     * This method should get and return a list of app links
     * associated with a user, given the userId.
     */
    // TODO #1👇:   Define the method signature with an empty method body

        // TODO #2👇:   Build the requestURL string

        // TODO #3👇: Execute a request the requestURL we built.
        //  Store the result as an InputStream and return the mapped data

        // TODO #4👇: Catch any error that results from our request
        //  Log the error and return null

    public List getAppLinks(String userId) {
        try {
            InputStream is = Request.Get(
                    orgUrl
                        //    + "/api/v1/users/" + userId + "/appLinks"
            )
                    .addHeader("Cache-Control", "no-cache")
                    .addHeader("Authorization", "SSWS " + apiToken)
                    .addHeader("Accept", "application/json")
                    .execute().returnContent().asStream();
            return Collections.emptyList();
            //return mapper.readValue(is, List.class);
        } catch (IOException e) {
            logger.error(
                    "Unable to get appLinks: {}", e.getMessage(), e
            );
            return null;
        }
    }
}
