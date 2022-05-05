Back to [main page](README.md).

---

# Okta Customer Identity for Developers Lab Guide

Copyright 2022 Okta, Inc. All Rights Reserved.

🟣 This module is written in **.NET**. You can alternatively complete this lab in [Java](module7-java.md).

## Module 7: Table of Contents

  -  [Lab 7-1: Send User Accounts Updates using an Event Hook](#lab-7-1-send-user-accounts-updates-using-an-event-hook)

  -  [Lab 7-2: (Optional) Customize User Registration Flow using an Inline Hook](#lab-7-2-optional-customize-user-registration-flow-using-an-inline-hook)

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


## Lab 7-2: (Optional) Customize User Registration Flow using an Inline Hook

🎯 **Objective:**  Implement an Okta Registration Inline Hook to validate users before self-registering.

🎬 **Scenario**    Okta Ice enabled user self-service registration. But a user needs to enter their SSN number during self-registration. Okta Ice will call a mock third-party API to validate the SSN. If the SSN entered during registration does not match the one returned by the API, the user cannot proceed with the registration. Okta Ice uses the SSN field only for user validation and does not want Okta to store the SSN information. 

User validation, even with self-service registration, can be extremely helpful and can be part of progressive profiling.  In this use case, a third party system has the validating data that Okta does not have at the time of user registration, but can lookup in an external data store.

⏱️ **Duration:**   30 minutes

⚠️ **Prerequisite:** Completion of [Lab 7-1](#lab-7-1-send-user-accounts-updates-using-an-event-hook) and [Lab 3-1](module3.md#lab-3-1-implement-self-service-registration)

---

### Open and Deploy the Employee Project in Visual Studio

1.	If Visual Studio is not currently open from the previous exercise, click the `Visual Studio 2017` icon in the Windows Task Bar.

2.	Click `Open Project or Solution`.

3.	Navigate to `C:\Classfiles\platform\hook\DotNet\NetCoreHooks`, and double-click the `NetCoreHooks17.sln` file. Visual Studio will open the `NetCoreHooks` project.

Explore the `RegistrationController` class

1.	From solution explorer, navigate to `Json/CustomerRegistration.json`. This is the format of the JSON payload that Okta will send to your `RegistrantController`.

2.	From Solution Explorer, open the `Controllers/RegistrantController` file.

3.	Examine the `VerifyRegistrantSSNByUserName()` method.

This method opens the request body and reads the incoming JSON into a variable named payload. We’ll parse the payload to extract the registrant’s `login` and `SSN`. We’ll use the `login` to search for the Registrant record in the `InMemoryDatabase` to get their `SSN`. If the `SSN`’s match, we’ll allow the registration to continue by sending Okta a `200 OK` action, otherwise we’ll disallow it by sending back an `Error` object.

### Implement coding changes
1.	Locate the `parsedJson` (declared around `Line 60`). Modify this line so that the variable gets the user profile data from the parsed JSON. 

```c#
/* 👇 Lab 7-2: 
 * TODO: Modify the userProfile variable so that
 * it gets the user profile data from the parsed JSON
 */
  JObject userProfile = (JObject)parsedJson
  .SelectToken("data")
  .SelectToken("userProfile");
```

2. Now let’s get the username from the user profile and assign the value to our existing String variable named `userName`:

```c#
/* 👇 Lab 7-2: 
 * TODO: Now extract the username from the userProfile
 * and save it to the String variable named userName
 */
userName = userProfile["login"].ToString();
```

3.	We now want to extract the SSN value from the user profile and assign the value to our existing variable named `ssnFromOkta` (`Line 80`):

```c#
/* 👇 Lab 7-2: 
 * TODO: Modify the ssnFromOkta variable so that it
 * stores the extracted SSN value from userProfile
 */
ssnFromOkta = userProfile["ssn"].ToString();
```

Notice that we only attempt to extract the SSN value once we verify that the ssn key exists in the `userProfile` object. This avoids a potential `KeyError`. If the key doesn’t exist, the else clause will build and return an `Error` object in the `response`.


4. Take a look at `Lines 109-114`. In this segment, we get the database information we have stored for this user in our database. If we got valid data (not null), we then extract the SSN value we had stored in the database. We will compare this value to the SSN we got from Okta to see if we get a match!

```c#
// gets information we have stored for this user from our database so we can compare to the info we got from Okta
var Registrant = await _simulatedRemoteDBComponent.FindByUserName(userName);
var RegistrantDTO = _mapper.Map<RegistrantDTO>(Registrant);
if (RegistrantDTO != null) // make sure we got valid data for the username we supplied
  {
      ssnFromDatabase = RegistrantDTO.SSN; // this is the ssn we got from our database
  }
```

Notice that if we did NOT get valid data from our database for the user, we DENY registration and return an error in the response (`Lines 115-141`). 

```c#
else // we didn't have any info for that user stored in our database (got null)
{
    // So, we can't verify the SSN!
    // Deny registration. Create new Error and return it in a response.
    response = new OktaHookResponse();
    Dictionary<String, String> dict = new Dictionary<string, string>
    {
        { "registration", "DENY" }
    };

    Command command = new Command();
    command.type = "com.okta.action.update";
    command.value = dict;
    response.commands.Add(command);
    Error error = new Error();
    error.ErrorSummary = "Unable to add registrant";
    error.ErrorCauses = new List<ErrorCause>
    {
        new ErrorCause{ErrorSummary = "Unable to convert Registrant to RegistrantDTO",
            Domain="end-user",
            Location="data.UserProfile.login",
            Reason="Unable to convert Registrant"}
    };
    response.Error = error;
    Debug.WriteLine("unable to convert Registrant to RegistrantDTO");
    return Ok(error);
}
```

5.	On Line `146` we check if the `SSN` received from Okta matches the `SSN` from the database. If it does, we create a new `Command` object. This type of object is where we can supply commands to Okta. Each element in a `Command` object consists of a name-value pair:

|  **Property**    | **Description**     | **Data Type**              |
|------------------------|---------------|----------------------------|
| type|One of the [supported commands](https://developer.okta.com/docs/reference/import-hook/#supported-commands)|String|                                      |
| value| The parameter to pass to the command|[value](https://developer.okta.com/docs/reference/import-hook/#value)| 

Under `Line 150` where the `Command` object is instantiated, we are going to specify a command that will update the user's Okta profile so that Okta will no longer store the user's SSN now that it has been verified. We will then add the `Command` object to our `response`:

```c#
/* 👇 Lab 7-2: 
* TODO: Construct a command to add to our Command object
* The type will be "com.okta.user.profile.update" since we will be updating this Okta user's 
* profile.  
* The value will set the user's SSN to the empty string so that we no longer store this 
* information on Okta now that it has been verified.
* Finally, we will add our Command object to the response
*/
Dictionary<String, String> emptySSN = new Dictionary<string, string>
{
    { "ssn", String.Empty }
};
command.type = "com.okta.user.profile.update";
command.value = emptySSN;
response.commands.Add(command);
```

6.	If the `SSN`s do NOT match, we will construct command that will tell Okta to deny registration. We will do this in the inner `else` clause after the `Command` object is instantiated. We will then add the `Command` object to the `response`:

```c#
 /* 👇 Lab 7-2: 
  * TODO: Construct a command to add to our Command object
  * The type will be "com.okta.action.update" which is an action 
  * we use when specifying whether to create a new Okta user when importing
  * users or matching them against existing Okta users.
  * The value DENY registration since the SSNs did not match.
  * 
  * Finally, we will add our Command object to the response
  */
  Dictionary<String, String> denyRegistration = new Dictionary<string, string>
  {
      { "registration", "DENY" }
  };
  command.type = "com.okta.action.update";
  command.value = denyRegistration;
  response.commands.Add(command);
```

7. Finally, we will construct an `Error` to add to the `response`. :Under the instantiated of the Error object (`Line 190`), let's specify that the user could not be registered in the `ErrorSummary`. We will leave the `ErrorCause` blank. 

```c#
/* 👇 Lab 7-2: 
 * TODO: Specify in the ErrorSummary that we could not add the registrant
 * Leave the ErrorCauses empty
 */
error.ErrorSummary = "Unable to add registrant";
error.ErrorCauses = new List<ErrorCause>{};
```
### Run the Hook Project
1.	From the Visual Studio toolbar, click the `IIS Express` button to run the `NetCoreHooks` project.

2.	A web page will open displaying the fictional names and SSNs we will use to test Okta’s Inline Hook capability.

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

3.	Minimize this browser window but leave Visual Studio open.


### Create the Inline Hook

1.  Sign into your Okta org as `okta.service`.

2.  In the Admin console, click `Workflow` > `Inline Hooks`.

3.  Click `Add Inline Hook` > `Registration`.

4.  Complete the fields as follows:

|  **Field**    | **Value**                                                                |
|--------------|---------------------------------------------------------------------------|
| Name           | SSN Validation                                                          |
| URL           | Your ngrok url (e.g. https://`a59d8d5a.ngrok.io`/registration/dblookup)  |
| Authentication field       |    x-api-key                                                |
| Authentication secret       |        Tra!nme4321                                         |


5.  Click `Save`.

### Modify the User Profile Schema to add the SSN Custom Attribute

1.  In the Admin console, click `Directory` > `Profile Editor`.

2.  From the FILTERS list, select `User (default)`.

3.  Scroll down to the **Attributes** section and click `Add Attribute`.

4.  Complete the fields as follows:

|  **Field**    | **Value**                                           |
|------------------------|--------------------------------------------|
| Display name           | SSN                                        |
| Variable name          | ssn                                        |
| Description            | User's SSN                                 |
| Attribute Length       | Between `1` and `20`                       |
| Attribute required     | (UNCHECKED)                                |

5.  Click `Save`.

6.  In the list of filters on the left, click `Custom`.

7.  Next to the SSN attribute, click the edit button (a `pencil icon`).

8.  Change **User permission** to `Read-Write`.

9.  Click `Save Attribute`.

### Configure the Self-Service Registration Page

1.  In the Admin console, navigate to `Directory` > `Self-Service Registration`.

2.  Click `Edit`.

3.  In the **Account** section, update the `Extension` field and select `SSN Validation`.

4.  In the **Registration Form** section, add the `SSN` field but **do not** check Required.

5.  In the **Post-Registration** section, **uncheck** Activation requirements

6.  Click `Save`.

### Test the Self-Service Registration Page

1.  **Sign out** from Okta.

2.  On the sign-in page, click `Sign up`.

3.  Complete the fields as follows:


|  **Field**             | **Value**                                   |
|------------------------|---------------------------------------------|
| Email                  | laura.mipsum@oktaice.com               |
| Password               | Tra!nme4321                                 |
| First Name             | Laura                                     |
| Last Name              | Mipsum                                     |
| SSN                    | LEAVE BLANK                                 |

  ---

4.  Click `Register`.
You should see the error message "SSN is required" and your registration cannot proceed.

<img src="img/7-2-ssn_required.png" width="300px">

5.  In the ngrok command line window, you should see a new `POST` request to the `/dblookup` endpoint.

6.  Change `SSN` to `2223344445` and click `Register`. You should see the error message "SSN doesn't match" and your registration cannot proceed.

<img src="img/7-2-ssn-nomatch.png" width="300px">

7.  Optionally, check the request in the `ngrok` command line window and/or the log file in Visual Studio under the `/logs` folder.

8.  Change `SSN` to `333333333` and click `Register`. Now you should be successful.

9. Optionally, check the request in the `ngrok` command line window and/or the log file in Visual Studio under the `/logs` folder.

10. Setup a recovery question and answer and select a security image.

11. Verify that your new user ends up on the end user dashboard.

12. Click `Laura` and select `Settings`.

13. On the Personal Information tab, verify that the SSN information you entered is not stored in Okta.

14. **Sign out** of Okta.

14.	Go back to Visual Studio and click the `stop` button, which will automatically close the browser window it started. 

15.	**Close** Visual Studio.

---
Back to [main page](README.md).