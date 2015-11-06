# AirTNG App: Part 1 - Workflow Automation with Twilio - ASP.NET MVC
[![Build status](https://ci.appveyor.com/api/projects/status/t8vnms8v35y1mul4?svg=true)](https://ci.appveyor.com/project/TwilioDevEd/airtng-csharp)

Learn how to automate your workflow using Twilio's REST API and Twilio SMS. This example app is a vacation rental site, where the host can confirm a reservation via SMS.

## Local Development

1. First clone this repository and `cd` into its directory:
   ```
   git clone git@github.com:TwilioDevEd/airtng-csharp.git

   cd airtng-csharp
   ```

2. Create a new file `AirTNG.Web/Local.config` and update the content with:
   ```
   <appSettings>
     <add key="TwilioAccountSid" value="Your Twilio Account SID" />
     <add key="TwilioAuthToken" value="Your Twilio Auth Token" />
     <add key="TwilioPhoneNumber" value="Your Twilio Phone Number" />
   </appSettings>
   ```

3. Build the solution.

4. Run `Update-Database` at [Package Manager
   Console](https://docs.nuget.org/consume/package-manager-console) to execute the migrations.

5. Run the application.

6. Check it out at [http://localhost:4567](http://localhost:4567)

### Configure Twilio to call your webhooks

   You will also need to configure Twilio to send requests to your application when SMS are received.

   You will need to provision at least one Twilio number with sms capabilities so the application's users can make property reservations. You can buy a number [right here](https://www.twilio.com/user/account/phone-numbers/search). Once you have a number you need to configure your number to work with your application. Open [the number management page](https://www.twilio.com/user/account/phone-numbers/incoming) and open a number's configuration by clicking on it.

   Remember that the number where you change the _SMS webhook_ must be the same one you set on the `TwilioPhoneNumber` setting.

   ![Configure Voice](http://howtodocs.s3.amazonaws.com/twilio-number-config-all-med.gif)

   Bear in mind that our endpoint is:
   ```
   http://<your-ngrok-subdomain>.ngrok.io/Reservations/Handle
   ```

That's it!

To let our Twilio Phone number use the callback endpoint we exposed, our development server will need to be publicly accessible. [We recommend using ngrok to solve this problem](https://www.twilio.com/blog/2015/09/6-awesome-reasons-to-use-ngrok-when-testing-webhooks.html).

## Meta

* No warranty expressed or implied. Software is as is. Diggity.
* [MIT License](http://www.opensource.org/licenses/mit-license.html)
* Lovingly crafted by Twilio Developer Education.
