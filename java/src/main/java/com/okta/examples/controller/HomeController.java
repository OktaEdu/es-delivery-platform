package com.okta.examples.controller;

import com.okta.examples.model.OktaAppLink;
import com.okta.examples.service.OktaUserService;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.servlet.ModelAndView;

import javax.servlet.http.HttpServletRequest;
import java.util.ArrayList;
import java.util.List;

@Controller
public class HomeController {

    private OktaUserService oktaUserService;

    @GetMapping("/")
    public String home() {
        return "home";
    }

    @GetMapping("/portal")
    public ModelAndView portal(HttpServletRequest request) {
        ModelAndView mav = new ModelAndView("portal");

        return mav;
    }

}
