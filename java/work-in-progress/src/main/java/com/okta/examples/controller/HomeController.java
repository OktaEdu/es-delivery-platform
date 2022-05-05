package com.okta.examples.controller;

import com.okta.examples.service.OktaUserService;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.servlet.ModelAndView;

import javax.servlet.http.HttpServletRequest;
import java.util.ArrayList;
import java.util.List;
/* 👇 Lab 6-5:
 * Import the OktaAppLink class
 */

@Controller
public class HomeController {

    private OktaUserService oktaUserService;

    /* 👇 Lab 6-5:
     * TODO: Define a one-argument constructor that initializes the value of
     *  the oktaUserService private instance variable
     */

    @GetMapping("/")
    public String home() {
        return "home";
    }

    @GetMapping("/portal")
    public ModelAndView portal(HttpServletRequest request) {
        ModelAndView mav = new ModelAndView("portal");
        /* 👇 Lab 6-5:
         * TODO: Retrieve the userId from the session stored in the request
         */


        /* 👇 Lab 6-5:
         * TODO: Retrieve a list of the user's app links by passing
         *  in the userId to getAppLinks()
         */

        /* 👇 Lab 6-5:
         * TODO: Verify that the appLinks list isn't null
         *  if it's not null, add it to our model
         *  otherwise, add an error to our model
         */



        /* 👆 Lab 6-5:
         * Your code modifications should end here.
         * ensure the return statement below remains the last
         * line in this method
         */
        return mav;
    }
}
