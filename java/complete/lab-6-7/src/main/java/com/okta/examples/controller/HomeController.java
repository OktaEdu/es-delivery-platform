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

    public HomeController(OktaUserService oktaUserService) {
        this.oktaUserService = oktaUserService;
    }

    @GetMapping("/")
    public String home() {
        return "home";
    }

    @GetMapping("/portal")
    public ModelAndView portal(HttpServletRequest request) {
        ModelAndView mav = new ModelAndView("portal");

        String userId = (String) request.getSession().getAttribute("userId");
        List<OktaAppLink> appLinks = oktaUserService.getAppLinks(userId);
        if (appLinks != null) {
            mav.addObject("apps", appLinks);
        } else {
            mav.addObject("apps", new ArrayList<>());
            mav.addObject(
                "error",
                "Unable to retrieve appLinks. Check the logs."
            );
        }

        return mav;
    }
}
