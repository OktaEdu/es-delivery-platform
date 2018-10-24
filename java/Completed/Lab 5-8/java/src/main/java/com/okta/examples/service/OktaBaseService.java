package com.okta.examples.service;

import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.okta.examples.model.OktaAppLink;

import java.util.List;
import java.util.Map;

public interface OktaBaseService {

    ObjectMapper mapper = new ObjectMapper();
    TypeReference<Map<String, Object>> MAP_TYPE = new TypeReference<Map<String, Object>>() {};
    TypeReference<List<OktaAppLink>> OKTA_LIST_APP_LINK_TYPE = new TypeReference<List<OktaAppLink>>() {};
}
