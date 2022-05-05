Back to [main page](README.md).

---

# Okta Customer Identity for Developers Lab Guide

Copyright 2022 Okta, Inc. All Rights Reserved.

🟣 This module is written in **.NET**. You can alternatively complete this lab in [Java](module7-java.md).

## Module 7: Table of Contents

  -  [Lab 7-1: Send User Accounts Updates using an Event Hook](#lab-7-1-send-user-accounts-updates-using-an-event-hook)


## Lab 7-1: Send User Accounts Updates using an Event Hook

🎯 **Objective:**  Implement an Okta event hook to received updates on user account events

🎬 **Scenario**    Okta Ice would like to get notified whenever a user account is being update, for example, a user changed the password, or a user profile is updated. In many CIAM use cases user updates are required by third party systems, as well as Okta.  Events Hooks provide one method of retrieving the update and feeding that information to other systems.

⏱️ **Duration:**   20 minutes

---

### Get an Ngrok Authentication Token

1.	Before we begin, stop any web servers left running in the VM. To do this, go to the Command Line Windows where the servers are running and press `Ctrl`+`C`. Close these Command Line Windows.

2.	Open a browser window and navigate to www.ngrok.com.

3.	If you do not already have an ngrok account, click the `Sign Up` button. At the prompt, create a new ngrok account:

<img src="img/7-2-ngrok_signup.png" width=" 400px">

4.	ngrok responds by showing you a screen with your authentication token:

<img src="img/7-2-ngrok_auth_token.png" width=" 600px">

5.	Copy and paste this token into a new Notepad text file, `ngrokToken.txt`.

6.	In your VM, open a NEW Command Prompt window (This can be done by right-clicking on the Command Prompt icon in the task bar and selecting Command Prompt)

7.	Enter the following command to update ngrok:

```bash
ngrok update
```

8.	Enter the following command to set your ngrok authentication token (replace `<your-ngrok-auth-token-here>` with your ngrok auth token):

```bash
ngrok authtoken <your-ngrok-auth-token-here>
```

After submitting this command, you should see a message indicating that a `.yml` file has been created:

<img src="img/7-2-ngrok_yml.png" width=" 600px">

Leave this window open.

### Open the Project in Visual Studio 2017

1.	On your VM, on the Windows Taskbar, click the Visual Studio 2017 icon.

2.	Click Open a Project or Solution.

3.	Navigate to `C:\ClassFiles\platform\hook\DotNet\NetCoreHooks`, and open the `NetCoreHooks17.sln` file.

4.	If warned about security, click OK to open the solution.

5.	Under Solution Explorer panel, right-click the `NetCoreHooks17` project and select `Properties`.

6.	Under the Debug section, check the `Enable SSL` checkbox.

7.	Save the project, accepting any security certificates, and then close the Properties tab.

### Explore the EventController Class

1.	In the Visual Studio Solution Explorer window:
    
    a. Open the `Controllers` folder.

    b. Double-click on `EventController.cs`.

2.	Notice this controller supports two HTTP actions: `GET`  and `POST`. 

    a. The `Get()` method verifies the hook by accepting a verification header, and sending the value back to Okta wrapped in a `VerificationResponse` object.
    b. The `Post()` method uses the `HttpRequest` object to capture incoming `json`. The method parses the `json` using `Linq-to-Json`. Then an `OktaEvents` object is created and populated with the parsed `json` data.

### Implement the Verification Function

1.	In the `EventController` class, locate the constant declaration, `VERIFICATION_HEADER` (around `Line 29`). Set the value of this constant to the following string:

```c#
/*
* 👇 Lab 7-1: 
* TODO: Set the value of VERIFICATION_HEADER to "x-Okta-Verification-Challenge"
*/
private const string VERIFICATION_HEADER = "x-Okta-Verification-Challenge";
```

2.	Locate the `Get()` function (around `Line 41`) and modify the response variable around `Line 49` so that it refers to a new `VerificationResponse` object.

```c#
/*
* 👇 Lab 7-1: 
*  TODO: Modify the response variable below so it refers to 
*  a new VerificationResponse object
*/
 VerificationResponse response = new VerificationResponse();
```

3. Read through the next lines (`64-73`) in the `Get()` method carefully to understand what is happening. Refer to the comments (lines `51-63`) for a walkthrough.

```c#
/*
 * 👇 Lab 7-1: 
 *  Note what is happening in the following lines (you do NOT need to modify this code):
 *  
 *  First we retrieve the value of the verification request header 
 *  and store it in a variable named verification.
 *  
 *  If it DOES NOT exist (null), we replace the null value with a message 
 *  that it was not found, log a warning, and return BadRequest
 *  
 *  If it DOES exist, we wrap the value in our VerificationObject variable (named response)
 *  and log a success.
 */
string verification = Request.Headers[VERIFICATION_HEADER];
if (verification == null)
{
    verification = "header " + VERIFICATION_HEADER + " was not found in the Request Headers collection";
    _logger.LogWarning($"Event EndpointVerify BadRequest will be returned. {verification}");
    return BadRequest(verification);
}
response.Verification = verification;
Debug.WriteLine("Verification: \n" + verification);
_logger.LogDebug($"Event EndpointVerify suceeded: {response.Verification}");
```

4.	At the end of our `Get()` method, we need to return a response status code of `200 OK` along with the `VerificationResponse` object. Modify `Line 78` so that the `Ok()` method gets passed the `VerificationResponse` object:

```c#
/* 👇 Lab 7-1: 
 * TODO: Pass in the VerificationResponse object (response) to the Ok() method
 */
return Ok(response); // return OK if verification request header returned a value
```

### Implement the Post (user-events) Function

1.	In the Solution Explorer panel, navigate to the `json` folder and open `AccountEvents.json`. This is a sample Event Delivery payload of a request from Okta. This illustrates the format of the payload sent to your external service that will be sent during a `POST` request. Close this file when you are done reviewing it.

2.	Back in `EventController.cs`, scroll down to the `Post()` method (around `Line 85`). 

3.	After the `OktaEvents` object gets instantiated on `Line 101`, we need to grab the relevant values from our parsed json (stored in `desiredEvent`) and assign those values to properties in our `OktaEvents` object:

```c#
/* 👇 Lab 7-1: 
 * TODO: Get Parsed JSON stored in desiredEvent and assign
 * these values to oktaEvents properties
 */
oktaEvents.EventType = desiredEvent["eventType"].ToString();
oktaEvents.DisplayMessage = desiredEvent["displayMessage"].ToString();
oktaEvents.EventTime = desiredEvent["published"].ToString();
```

Note that the `OktaEvents` object is returned by this method.

4.	In the Solution Explorer panel, navigate to the model folder. Open `OktaEvents.cs`. Examine the overridden `ToString()` method. This is the format of the string that will printed to the log file upon successful execution.

### Execute the Hook Project

1.	Run the `NetCoreHooks` application by clicking on the `IIS Express` button in the Visual Studio toolbar.
 
2.	A browser window will open with a list of existing registrants along with their fictional SSNs:

```
Hooks Registrant Test Data - SSNs:

Laura Mipsum : 333-33-3333
Hannibal Smith : 123-45-6789
Jane Zielinski : 987-65-4321
Hank Aaron : 444-44-4444
Slick Salesman : 555-55-5555
Quinn Morelli : 222-22-2222
Javier Lopez : 777-77-7777
```

3.	Make note of the **port number** in the address bar (e.g., `44370`. Your port number may vary). This will be used in the next step.

### Execute ngrok from your Command Window

1.	Return to your ngrok command window from the first step. Execute the following command, replacing `<PORT-NUMBER>` with the value you noted in the previous step.:

```bash
ngrok http https://localhost:<PORT-NUMBER> -host-header="localhost:<PORT-NUMBER>"
```

2.	ngrok will respond with some Forwarding URLs: 

<img src="img/7-2-ngrok_forwarding_urls.png" width=" 600px">

3.	Highlight the URL that uses `https` and press `Enter` to copy it. This will copy the URI to your Windows clipboard. We will paste this URL into Okta in the next section. 

📝 **Note**:  If you are unable to select text in the command window, right click and click `Select All`. You should then be able to modify your selection.

### Create and Verify the Event Hook

1.	Log in to your Okta org as `okta.service`.

2.	In the Admin console, click `Workflow` > `Event Hooks`.

3.	Click `Create Event Hook`.

4.	Complete the fields as follows:

|  **Field**  | **Value**                    |
|-----------------|------------------------------|
| Name            | User Account Events         |
| URL             | Your ngrok URL, e.g. https://`a59d8d5a.ngrok.io`/event/user-account     |
| Authentication field        | x-api-key                   |
| Authentication secret        | Tra!nme4321                    |
| Subscribe to events       | <ul><li>- [x] User's Okta profile updated</li></ul><ul><li>- [x] Fired when the user's Okta password is reset</li></ul><ul><li>- [x] User's Okta password updated</li></ul>  

5.  Click `Save and Continue`.

6.  On the Verify Endpoint Ownership window, click `Verify`.

7.  The User Account Events hook is created and verified:

<img src="img/7-1-event_hook_verified.png" width="600px">

8.  Go back to the **ngrok command line window**, you should see a new `GET` request:

<img src="img/7-1-ngrok_get_request.png" width="300px">

### Test the Event Hook

1.  In the Admin console, click `Directory` > `People`.

2.  Select `Kay West` from the list.

3.  Click the `Profile` tab then click `Edit`.

4.  Update the `Middle name` field with `Jackson` and click `Save`.
Since a user’s Okta profile has been updated, this should have triggered our Event Hook. 

5.  In the **ngrok command line window**, you should see a new `POST` request:

<img src="img/7-1-ngrok_post_request.png" width="300px">


5.  In the **ngrok command line window**, you should see a new `POST` request:

<img src="img/7-1-ngrok_post_request.png" width="300px">

6.  In Visual Studio, go to the Solution Explorer and navigate to the logs folder in the current project. Open the log file that corresponds to today’s date. The last entry in the log should look something like:

```
On 2022-05-01T22:44:19.469Z , a user.account.update_profile event happened to your Org with the following description: Update user profile for Okta
```

7.	Click the `Stop Debugging` button (or press `Shift` + `F5`) but leave the ngrok command window open. You’ll need it for the next exercise.


---
Back to [main page](README.md).