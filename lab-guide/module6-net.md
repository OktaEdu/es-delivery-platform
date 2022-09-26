Back to [main page](README.md).

---

# Okta Customer Identity for Developers Lab Guide

Copyright 2022 Okta, Inc. All Rights Reserved.

🟣 This module is written in **.NET**. You can alternatively complete this lab in [Java](module6-java.md). Throughout this lab guide, we refer to the [Okta .NET Authentication SDK](https://github.com/okta/okta-auth-dotnet) as simply the .NET SDK.

## Module 6: Table of Contents

  -  [Lab 6-1: Setup Authentication with the .NET SDK](#lab-6-1-setup-authentication-with-the-net-sdk)

  -  [Lab 6-2: Add Redirect to a Portal Home Page with the .NET SDK](#lab-6-2-add-redirect-to-a-portal-home-page-with-the-net-sdk)

  -  [Lab 6-3: Establish a Session with Okta with the .NET SDK](#lab-6-3-establish-a-session-with-okta-with-the-net-sdk)

  -  [Lab 6-4: Update Service to Retrieve Application Embed Links with the .NET SDK](#lab-6-4-update-home-controller-to-retrieve-application-embed-links-with-the-net-sdk)

  -  [Lab 6-5: Enable Single Sign-On to Assigned Applications with the .NET SDK](#lab-6-5-enable-single-sign-on-to-assigned-applications-with-the-net-sdk)

## Lab 6-1: Setup Authentication with the .NET SDK

🎯 **Objective:**  Add display parameters in the View to show that authentication was successful. Define that backing service that will perform the callouts to Okta for authentication. This will be called from the controller later. Update the controller logic to do handle the request to login.         

⏱️ **Duration:**   20 minutes

⚠️ **Prerequisite**: Completion of [Lab 3-2](module3.md/#lab-3-2-create-a-registration-page-using-the-okta-management-sdk---net).

---

### Open the OktaAPILab

1.	Launch Visual Studio 2017.

2.	From the menu, select `File` > `Open` > `Project/Solution...`

3.	In the Open Project window, inside the `C:/ClassFiles/platform/aspnet/Lab6StartingPoint/OktaAPILab` folder, select the `OktaAPILab.sln` and click `Open`.

### Update the Account Controller with Okta Org Information

1.	Open the `Controllers` > `AccountController.cs` file.

2.	Declare and initialize the `_oktaUrl`, and `_oktaApiToken` variables. 

   a. For `_oktaApiToken`: replace the sample value with the API token you generated.

   b. For `_oktaUrl`: replace `###` with your assigned Okta org number.

```c#
// TODO: 👇 Lab 6.1: replace value with your Okta org URL 👇
private readonly string _oktaUrl = "https://oktaice###.oktapreview.com"; 
// TODO: 👇 Lab 6.1: replace value with your API token 👇
private readonly string _oktaApiToken = "abc123"; 
```

📝 **Note:** In our example, we are placing the API token directly in the source code for simplicity sake. **This should not be done for production systems**. The API token is a secret that should be not stored in source code.

### Update the Login.cshtml

1.	In the Solution Explorer pane, open the `Views` > `Account` > `Login.cshtml` file.

2.	Add an additional `<h4>` tag under the comment to display the Session Token from the controller:

```html
 <!--
    👇 Lab 6-1: Add an additional h4 tag below to display the Session Token
 -->
<h4 style="color:green">@ViewBag.SessionToken</h4>
```

3.	Save and close `Login.cshtml`.

### Import the Okta Auth SDK with NuGet

1.	In the Solution Explorer pane, right-click on the `OktaAPILab` project and then click `Manage NuGet Packages...`

2.	In the `NuGet: OktaAPILab` page, perform the following:

   a. In the top left pane, if necessary, change the tab from `Installed` to `Browse`.

   b. In the upper left Search field, enter: `okta`

   c. Select `Okta.Auth.Sdk by Okta, Inc`.

   d. Select `OktaAPILab` from the project list.

   e. Select `1.1.0` from the Version dropdown on the right side and click `Install`.

   f. In the Preview Changes window, select `OK`.

   g. After the process has completed, close the tab for NuGet.

3.	In the Solution Explorer, open `Controllers` > `AccountController`.

4.	Under the `using namespace` statements at the top, add the `Okta.Auth.Sdk` namespace:

```c#
// 👇 Lab 6.1: add the add the Okta.Auth.Sdk namespace
using Okta.Auth.Sdk;
```
5.	Click `Build` > `Build OktaAPILab`.

📝 **Note:** If you encounter any errors and need assistance, let your instructor know.

### Update the Login method to use the Okta authentication API

1.	In the `AccountController` class, locate the  `Login()` method for our `POST` request. This is the second `Login()` in this class and is located around `Line 68`.

2.	Inside the `if` clause in this `Login()` method, there is a variable called `oktaClientConfig`. Currently it is set to the empty string. We want to modify this so that creates a new Okta client configuration based on your Okta Org URL and your Okta API token. 

```c#
if (ModelState.IsValid) 
{
 /* 
  * 👇 Lab 6-1: 
  * Modify the value of oktaClientConfig with your Okta Client Configuration details 
  */
  var oktaClientConfig =  new Okta.Sdk.Abstractions.Configuration.OktaClientConfiguration
    {
	  OktaDomain = _oktaUrl,
	  Token = _oktaApiToken
    };
```

3. The next variable is `authnClient`. We're going to modify this variable so that it holds a new instance of the `AuthenticationClient` class from the SDK. We'll supply our client configuration details by passing in `oktaClientConfig`.

```c#
/* 
 * 👇 Lab 6-1:
 * Modify the value of authnClient so it instantiates a new AuthenticationClient object with our client configs 
 */
var authnClient = new AuthenticationClient(oktaClientConfig);
```

4.	Now we will modify `authnOptions` so that it contains mappings for Username and Password:

```c#
/* 
 * 👇 Lab 6-1: 
 * Modify the value of authnOptions with mappings for Username and Password 
 */
var authnOptions = new AuthenticateOptions()
  {
    Username = model.Email,
    Password = model.Password
  };

```

5.	In the `try` block that follows, modify the `authnResponse` variable so with a callout to Okta using the `AuthenticateAsync()` method that passes in the authentication details:

```c#
try {
 /* 👇 Lab 6-1:
  * Update the variable below with a callout to Okta via our Authentication Client
  * using the AuthenticateAsync() method that passes in the authentication details
  */
  	var authnResponse = await authnClient.AuthenticateAsync(authnOptions); 
}
```

Note that the return type of this callout is an `IAuthenticationResponse` object.

6. Since we changed the type of data the `authnResponse` variable holds from a `String` to a `IAuthenticationResponse`, you'll note in the `try` block that follows, the IDE now tells us that we cannot compare `authResponse` to a `String`. Let's fix this so that we're now comparing the `AuthenticationStatus` property of `authnResponse` (a `String`) to the `String` `"SUCCESS"`:

```c#
/* 👇 Lab 6-1:
* Since we changed authnResponse from a String to an
* IAuthenticationResponse object, we need to change the line below 
* so it compares the AuthenticationStatus property of authnResponse to "SUCCESS"
*/
if (authnResponse.AuthenticationStatus == "SUCCESS") 
```

7.	Now that we're correctly checking if a `"SUCCESS"` was returned, let's specify what happens. If we got a `"SUCCESS"`, store the `AuthenticationStatus` and `SessionToken` so we can display it.

```c#
  if (authnResponse.AuthenticationStatus == "SUCCESS") 
  {
   /* 
    * 👇 Lab 6-1: 
    * Update the code below so that when you get a SUCCESS
    * response, you save the AuthenticationStatus and SessionToken
    */
    ViewBag.Status = authnResponse.AuthenticationStatus;
    ViewBag.SessionToken = authnResponse.SessionToken;
  }
```

8. What if we get some other status other than a `"SUCCESS"`? Scroll down to the `else` statement and specify that we should store the `AuthenticationStatus` in the `ErrorSummary` property so we can display it:

```c#
else
  {
    /* 
    * 👇 Lab 6-1: 
    * Update the code below so that when you DON'T get a SUCCESS
    * response, you store the unexpected status to the ErrorSummary
    * Note that the change we are making here is that we're 
    * accessing the AuthenticationStatus property of authResponse
    * since authResponse is now an IAuthenticationResponse object
    */
    ViewBag.ErrorSummary = "Unexpected Status: " + authnResponse.AuthenticationStatus;
  }
```

9.	Another possibility is that we encounter an error. Scroll down to the `catch` clause. In this case, let's change the type of exception we expect to `Okta.Sdk.Abstractions.OktaApiException` and then specify that we want to store the `ErrorSummary` property of that exception to our view so we can display it: 

```c#
   /* 
    * 👇 Lab 6-1:
    * If an exception is caught, store the error.
    * Make sure you update oktaError's type
    * and access oktaError's ErrorSummary property
    */
  catch (Okta.Sdk.Abstractions.OktaApiException oktaError)
  {
      ViewBag.ErrorSummary = oktaError.ErrorSummary;
  }
```

10.	Save `AccountController.cs`.

### Test the Log In without Okta SDK 
1.	From the menu, select `Debug` > `Start Debugging`.

2.	In the top-right corner of the page, click `Log in`. 

3.	Sign in again as `kay.west@oktaice.com` using an **incorrect** password.
The page should refresh with the "Authentication failed" message.

4.	Sign in again as `kay.west@oktaice.com` using **correct** credentials.

5.	Verify that the status of `SUCCESS` is returned with the session token.

6.	Close the web browser running inside the VM, *or* in Visual Studio, click `Debug` > `Stop Debugging`.

## Lab 6-2: Add Redirect to a Portal Home Page with the .NET SDK

🎯 **Objective:**  Update the controller to redirect the user to a landing page after successful authentication. 

⏱️ **Duration:**   10 minutes

⚠️ **Prerequisite**: Completion of [Lab 6-1](#lab-6-1-setup-authentication-with-the-net-sdk).

---

### Examine Landing Page Image

1.	In the Solution Explorer pane, expand the `wwwroot/images folder`, then double-click the `Ice-Cream-3Flavors.jpg` file.

2.	This image will be used in the portal view.

3.	Close `Ice-Cream-3Flavors.jpg`.

### Examine Mock Portal Landing Page

1.	In the Solution Explorer pane, expand the Views/Home folder, then double-click the PortalHome.cshtml file.

2.	Examine the code segment under the `<body>` section. Right now, this file just displays a mock of some buttons and the landing page image. We will add more to this file in the following labs.

3.	Close the `PortalHome.cshtml` file.

### Update the AccountController to Redirect User to the Portal Home Page

1.	In Solution Explorer open the `Controllers` > `AccountController.cs` file.

2.	Inside the `Login()` method for our `POST` request, locate the `if` statement (around `Line 115`) for a successful response. Add a `return` statement to redirect the user to the landing page as follows:

```c#
 /* 
  * 👇 Lab 6-2: 
  * add a return statement below to redirect the user to the landing page
  */
return RedirectToAction("PortalHome", "Home");
```

3.	Save `AccountController.cs`.

### Test the Log In with redirection to the Landing Page

1.	Click `Debug` > `Start Debugging`.

2.	In the top-right corner, click `Log in`. 

3.	Sign in as `kay.west@oktaice.com`.

4.	Confirm that you landed on the new Portal Home landing page with 3 ice cream cones.

5.	Close the web browser to stop debugging mode.

## Lab 6-3: Establish a Session with Okta with the .NET SDK

🎯 **Objective:**  Update the controller to redirect to Okta to establish a session.    

⏱️ **Duration:**   15-20 minutes

⚠️ **Prerequisite**: Completion of [Lab 6-2](#lab-6-2-add-redirect-to-a-portal-home-page-with-the-net-sdk).

---

### Enable SSO to Okta in the AccountController

Instead of redirecting the user to the portal landing page directly, we are going to direct the user to Okta first to establish a session in our Okta domain.

1.	Open the `AccountController.cs` file.

2.	Inside the `Login()` method for our `POST` request, locate the `if` statement that checks for a successful response (around `Line 115`). Comment out the `return` statement we just added in [Lab 6-2](#lab-6-2-add-redirect-to-a-portal-home-page-with-the-net-sdk). 

```c#
// return RedirectToAction("PortalHome", "Home");
/* 
 * ☝️ Lab 6-3: 
 * Comment out this return statement for Lab 6-3
 */
```

3.	Store the `userId` that we get from the response from the Authentication Client to our Session:

```c#
/* 
 * 👇 Lab 6-3: 
 * Store the userid that we get from the response from 
 * the Authentication Client to our Session
 */
HttpContext.Session.SetString(
     "userId", 
authnResponse.Embedded
             .GetProperty<Okta.Auth.Sdk.Resource>("user")
             .GetProperty<String>("id")
);
```

📝 **Note:** Later, you will use the `userId` to fetch the apps available for the user.

4.	On the next line, we will build the redirect URL string. It will be composed of your Okta Org URL and the `login/sessionCookieRedirect` endpoint with the `token` and `redirectUrl` query strings.

```c#
/* 
 * 👇 Lab 6-3: 
 * Build the redirect URL string composed of your Okta Org URL and the 
 * login/sessionCookieRedirect endpoint with the token and redirectUrl query strings. 
 */
string redirectToOktaUrl = _oktaUrl + "/login/sessionCookieRedirect";

redirectToOktaUrl += "?token=" + authnResponse.SessionToken;
redirectToOktaUrl += "&redirectUrl=https://" 
                     + HttpContext.Request.Host.ToString() 
                     + "/Home/PortalHome";

return Redirect(redirectToOktaUrl);
```

5.	Save the file.

### Setup the .NET host as a Trusted Origin

Your instance of your .NET server needs to be granted access in Okta.

1.	In Visual Studio, click `Debug` > `Start Debugging`.

2.	Copy the application URL to Notepad.

📝 **Note:** The URL should be similar to `https://localhost:56506`. You will use this URL  in Step 7 to authorize redirects for this .NET app in Okta.

3.	In Chrome, launch a new browser tab and log into your Okta org as `oktatraining`.

4.	In the Admin page, go to `Security` > `API`.

5.	Select `Trusted Origins`.

6.	Click `Add Origin`.

7.	Provide the information as follows: 

|  **Attribute**  | **Value**                    |
|-----------------|------------------------------|
| Name            | .NET Portal                  |
| Origin URL      | `https://localhost:<PORT-NUMBER>` |
| CORS            | (UNCHECKED)                  |
| Redirect        | (CHECKED)                    |

8.	Click `Save`.

9.	Sign out of Okta and then close that browser tab.

### Test the Log in with SSO to Okta

1.	Return to the browser tab with the .NET Application. 

2.	In the top-right corner of the page, click `Log in`. 

3.	Sign in as `kay.west@oktaice.com`.

4.	Verify that you land on the new Portal Home page.

5.	Close Chrome or in Visual Studio, click `Debug` > `Stop Debugging`.
 


## Lab 6-4: Update Home Controller to Retrieve Application Embed Links with the .NET SDK

🎯 **Objective:**  Update the Home Controller to retrieve the assigned apps for the currently signed in user.

⏱️ **Duration:**   15 minutes

⚠️ **Prerequisite**: Completion of [Lab 6-3](#lab-6-3-establish-a-session-with-okta-with-the-net-sdk).

---

### Enable SSO to Promos UI app in the HomeController 
1.	In Visual Studio, open the `Controllers` > `HomeController.cs` file.

2.	Under the `using namespace` statements at the top, add the `Okta.Sdk` and `Okta.Sdk.Configuration` namespaces:

```c#
// 👇 Lab 6.4: add the Okta.Sdk and Okta.Sdk.Configuration namespaces
using Okta.Sdk;
using Okta.Sdk.Configuration;
```
3.	Next, modify the `_oktaUrl`, and `_oktaApiToken` variables. 

   a. For `_oktaUrl`: replace `###` with your assigned Okta org number.

   b. For `_oktaApiToken`: replace the sample value with the API token you generated.
 
    
   📝 **Note:** In our example, we are placing the API token directly in the source code for simplicity sake. **This should not be done for production systems**. The API token is a secret that should be not stored in source code.

```c#
// TODO: 👇 Lab 6.4: replace value with your Okta org URL 👇
private readonly string _oktaUrl = "https://oktaice###.oktapreview.com"; 
// TODO: 👇 Lab 6.4: replace value with your API token 👇
private readonly string _oktaApiToken = "abc123"; 
```

4.	Save the file. 

### Update HomeController to get App Links

1.	In the `PortalHome()` method (around `Line 44`), change the method signature so that it runs asynchronously (keyword `async`) and so that the return type is `Task<IActionResult>`:

```c#
/* 👇 Lab 6-4:
 * Update the PortalHome() method signature so that it is an asynchronous action
 */
 public async Task<IActionResult> PortalHome()
 ```

 2.	Now update the `PortalHome()` body so that it retrieves the `userId` from the `Session` that was set by the `AccountController`:

 ```c#
 /* 👇 Lab 6-4:
  * Update the the userId variable below so that 
  * it stores the userId from the Session 
  */
 string userId = HttpContext.Session.GetString("userId");
 ```

3.	Initialize the `OktaClient` object with the connection details for your Okta org.

```c#
/* 👇 Lab 6-4:
 * Define your OktaClientConfiguration
 * and instantiate a new instance of the OktaClient with
 * these configurations
 */
OktaClientConfiguration oktaConfig = new OktaClientConfiguration
  {
    OktaDomain = _oktaUrl,
    Token = _oktaApiToken
  };
  OktaClient oktaClient = new OktaClient(oktaConfig);
```

4.	Now we will `try` to get the application data assigned to the currently logged in user. We will do this by calling the `ListAppLink()` method and passing in the `userId` we previously retrieved from the `Session`:

```c#
try {
   /* 👇 Lab 6-4:
   * Try to get the list of the app data associated 
   * with the user in the current session
   */
  var appLinks = await oktaClient.Users.ListAppLinks(userId).ToList();
  ViewBag.Apps = appLinks;
  ViewBag.IsSuccessful = true;
}
```

5.	If the callout does not succeed, we will catch the error and store the `ErrorSummary` to our view so we can display it:

```c#
 /* 👇 Lab 6-4:
  * If the above results in an error,
  * we want to catch the exception
  * (change the type of the error to OktaApiException) 
  * and store the error's ErrorSummary property
  * (change what Viewbag.ErrorSummary stores)
  */
  catch (OktaApiException oktaError)
  {
    ViewBag.ErrorSummary = oktaError.ErrorSummary;
    ViewBag.IsSuccessful = false;
  }
  return View();
```

📝 **Note:**

-	The list of apps is returned as a JSON array. 

-	In this lab, there is only one app assigned to the user, so we only get one app returned in the list. 

-	Each app has two important pieces of data: the **SSO link** and the **URL for the app logo**. These are stored in the `ViewBag` for later reference.

6.	Save the file.

7.	Click `Build` > `Build OktaAPILab`.

📝 **Note:** If you encounter errors and need assistance, let your instructor know.


## Lab 6-5: Enable Single Sign-On to Assigned Applications with the .NET SDK

🎯 **Objective:**  Update the landing page to provide federate access to Promos UI app via OIDC.      

⏱️ **Duration:**   20 minutes

⚠️ **Prerequisite**: Completion of [Lab 6-4](#lab-6-4-update-home-controller-to-retrieve-application-embed-links-with-the-net-sdk).

---

### Enable SSO to Promos UI app in the PortalHome

The PortalHome page must be updated to include a new link for the Promos UI app.

1.	Open the `Views` > `Home` > `PortalHome.cshtml` file.
If there is an error, we want to display it. So, under the `<h2>` tag  (`Line 36`), let's add some code that will display the error if there is one stored in the `ViewBag`.

```html
<!--👇 Lab 6-5: Add code to display the error summary if we have one stored in the Viewbag-->
<h4 style="color:red">@ViewBag.ErrorSummary</h4>
```
2.	Now we’ll check if our attempt to get the `applinks` associated with the user logged in during this session `is successful`. Write the code for this check just under the opening `<ul>` tag:

```html
<ul>
<!--
 👇 Lab 6-5: Check ViewBag to see if the call to get the user's applinks was successful
-->
  @if (ViewBag.IsSuccessful)
  {
  }
  <li>
```

3.	If the  add the logic to iterate over the list of `applinks` returned by the controller:

```html
@if (ViewBag.IsSuccessful)
{
  <!--
    👇 Lab 6-5: If it was successful, iterate over the list of returned applinks and display them as list items
  -->
  @foreach (var app in ViewBag.Apps)
  {
    <li>
      <a class="navbutton" href="@app.LinkUrl" target="_blank">
        <img src="@app.LogoUrl" /> @app.Label
      </a>
    </li>
  }
}
```

4.	Save the file.

### Test the Log in with SSO to Promos UI app

1.	Click `Debug` > `Start Debugging`.

📝 **Note:** If the web browser is already running, verify that you are not currently logged into Promos UI app or Okta.

2.	Sign in as `kay.west@oktaice.com`.

> The home page displays all apps assigned to Kay West, including Promos UI app.

3.	Click `Promos UI Green`. 

> A new browser tab opens, and you should be successfully logged into the promos app.

4.	Close Chrome or in Visual Studio, click `Debug` > `Stop Debugging`. 

5.	Click `File` > `Close Solution` and Shut down Visual Studio.


---
Back to [main page](README.md).