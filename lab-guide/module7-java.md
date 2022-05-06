Back to [main page](README.md).

---

# Okta Customer Identity for Developers Lab Guide

Copyright 2022 Okta, Inc. All Rights Reserved.

☕ This module is written in **Java**. You can alternatively complete this lab in [.NET](module7-net.md).

## Module 7: Table of Contents

  -  [Lab 7-1: Send User Accounts Updates using an Event Hook](#lab-7-1-send-user-accounts-updates-using-an-event-hook)


## Lab 7-1: Send User Accounts Updates using an Event Hook

🎯 **Objective:**  Implement an Okta event hook to received updates on user account events

🎬 **Scenario**    Okta Ice would like to get notified whenever a user account is being update, for example, a user changed the password, or a user profile is updated. In many CIAM use cases user updates are required by third party systems, as well as Okta.  Events Hooks provide one method of retrieving the update and feeding that information to other systems.

⏱️ **Duration:**   20 minutes

---

### Open the Project in IntelliJ

1.  Before you begin, **stop any web servers** left running in the VM. To do this, go to the command line windows where the servers are running and press `Ctrl`+`C`. Close these command line windows.

2.  On the **Windows Taskbar**, click the **IntelliJ** icon.

3.  Click `Open`.

4.  Navigate to `C:\` and click the **refresh** button.

5.  Expand `ClassFiles` > `platform` > `hook` > `java` > `hookproject` and select `pom.xml`, and click `OK`.

6.  Click `Open as Project` and wait for the Maven build to complete.

### Explore the RequestConverter Class

1.  Open the `hookproject` > `src` > `main` > `java` > `com.okta.example.hookproject` > `utitliy` > `RequestConverter.java` file.

2.  Examine the `httpToJSON()` method (Starting on `Line 11`).
This method takes in an `HTTPServletRequest` object, parses through it, and returns a `JSONObject` with the request data. You will use this converter for both the Okta Event Hook and Okta Inline Hook.

### Implement the Verification Function

1.  Open the `hookproject` > `src` > `main` > `java` > `com.okta.example.hookproject` > `controller` > `EventHookController.java` file.

2.  Scroll down to the `endpointVerify()` method (around `Line 76`).

3.  Inside this method, set the value of the `String` named `VERIFICATION_HEADER` to `"x-okta-verification-challenge"`. 

```java
/*
 * 👇 Lab 7-1: 
 * TODO: Set the value of VERIFICATION_HEADER to "x-okta-verification-challenge"
 */
final String VERIFICATION_HEADER = "x-okta-verification-challenge";
```

4.  Set value of the `String` named `verification` with the value from the request header:

```java
/*
 * 👇 Lab 7-1:
 * TODO: Set the value of the verification variable to the value from the request header
 * that is keyed on our VERIFICATION_HEADER
 * This will be used in the subsequent lines to set the verification value in the response
 */
String verification = request.getHeader(VERIFICATION_HEADER);
```

### Implement the User Account Events Receiver Function

1.  Scroll back up to the `accountEvents()` method (around `Line 27`).

2. Review the first segment of this method to understand what is happening. Note that this segment makes use of the utility function you examined in the [Explore the RequestConverter Class](#explore-the-requestconverter-class) of this lab.

```java
 /*
  * 👇 Lab 7-1:
  * Review this code segment to understand what is happening.
  * (No modification necessary)
  *
  * 1. First, we use our utility function to parse through the
  * JSON payload of Okta's request to our external service
  * This function will convert the payload into a JSONObject (eventBody)
  *
  * For an example of what this payload looks like see:
  * https://developer.okta.com/docs/concepts/event-hooks/#sample-event-delivery-payload
  *
  * 2. Then we extract the information from the "data" entry in that JSONObject
  * and store it to the variable named data.  Notice that this entry is another JSONObject.
  *
  * 3. Finally, we extract the "events" entry from the data JSONObject.
  * Notice that this is a JSONArray. We will use the events JSONArray
  * to access pertinent String data from the response.
  *
  */
JSONObject eventBody = RequestConverter.httpToJSON(request);
JSONObject data = eventBody.getJSONObject("data");
JSONArray events = (JSONArray) data.get("events");
 /*
  * ☝️ End of review segment
  */
```

3.  Update the `eventType`, `displayMessage` and `eventTime` Strings with values extracted from the HTTP Request so we can pass these values to our `OktaEvent` model and log the information:

```java
 /*
  * 👇 Lab 7-1:
  * TODO: Update the Strings below with values extracted from events JSONArray
  *   retrieved from the HTTP request.
  * We want to store the event type, the display message, and the time event was published.
  * The keys for these entries are eventType, displayMessage, and published.
  *
  * For an example JSON payload of a request from Okta to your external service see:
  * https://developer.okta.com/docs/concepts/event-hooks/#sample-event-delivery-payload
  *
  * We will pass these values to our OktaEvent model and log the details.
  */
  String eventType = events.getJSONObject(0).optString("eventType");
  String displayMessage = events.getJSONObject(0).optString("displayMessage");
  String eventTime = (String)eventBody.get("eventTime");
```
4.  In the next line of code, right-click `OktaEvents` and select `Go To` > `Declaration`.

5.  The `OktaEvents` class is opened. Look at the `toString()` method and examine the format of the event details that will be printed out.

```java
public String toString(){
  String eventDetail = "On "+eventTime +" , a "+ eventType + " event happened to your Org with the following description: "+ displayMessage;
  return eventDetail;
    }
```

  You can close the `OktaEvents` class when you are finished reviewing it.

### Deploy the Hook Project

1.  Expand `hookproject` > `src` > `main` > `java` > `com.okta.example.hookproject`. Right-click `HookprojectApplication` and click `Debug 'HookprojectApplication'`.

2.  Wait until the console tab displays the message **Started `HookprojectApplication`**.

3.  Leave IntelliJ open.

4.  Open a new command prompt inside your VM.

5.  Enter the following command to update ngrok:

```bash
ngrok update
```

6.  Enter the following command to start ngrok:

```bash
ngrok http 8080
```

7.  ngrok will respond with some Forwarding URLs:

<img src="img/7-1-ngrok_urls.png" width="600px">

8.  **Highlight** the URL that uses `https` and press `Enter` to copy it. This will copy the URI to your Windows clipboard. We will paste this URL into Okta in the next section. 
    
📝 **Note** If you are unable to select text in the command window, right click and click `Select All`. You should then be able to modify your selection.

### Create and Verify the Event Hook

1.  Sign into your Okta org as `okta.service`.

2.  In the Admin console, click `Workflow` > `Event Hooks`.

3.  Click `Create Event Hook`.

4.  Complete the fields as follows:

|  **Field**  | **Value**                    |
|-----------------|------------------------------|
| Name            | User Account Events         |
| URL             | Your ngrok URL, e.g. https://`a59d8d5a.ngrok.io`/event/user-account     |
| Authentication field        | x-api-key                   |
| Authentication secret        | Tra!nme4321                    |
| Subscribe to events       | <ul><li>- [x] User's Okta profile updated</li></ul><ul><li>- [x] Fired when the user's Okta password is reset</li></ul><ul><li>- [x] User's Okta password updated</li></ul>                  |

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

4.  Update the `Middle name` field with `O'Reilly` and click `Save`.
Since a user’s Okta profile has been updated, this should have triggered our Event Hook. 

5.  In the **ngrok command line window**, you should see a new `POST` request:

<img src="img/7-1-ngrok_post_request.png" width="300px">

6.  In IntelliJ, the event message similar to the one below is displayed in the console:
```
On 2022-05-01T22:44:19.469Z , a user.account.update_profile event happened to your Org with the following description: Update user profile for Okta
```

7.  In IntelliJ click `Run` > `Stop 'HookprojectApplication'`.

8.  Close the `hookproject`project.

9. Go to your ngrok command window and press `Ctrl`+`C` to close the session.


---
Back to [main page](README.md).