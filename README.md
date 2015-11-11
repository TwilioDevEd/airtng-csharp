# AirTNG App: Part 2 - Masked Numbers With Twilio - ASP.NET MVC
[![Build status](https://ci.appveyor.com/api/projects/status/t8vnms8v35y1mul4?svg=true)](https://ci.appveyor.com/project/TwilioDevEd/airtng-csharp)

Protect your customers' privacy, and create a seamless interaction by provisioning Twilio numbers on the fly, and routing all voice calls, and messages through your very own 3rd party. This allows you to control the interaction between your customers, while putting your customer's privacy first.

## Configure Twilio to call your webhooks

You will need to configure Twilio to send requests to your application when SMS are received.

You will need to provision at least one Twilio number with sms capabilities so the application's users can make property reservations. You can buy a number [right here](https://www.twilio.com/user/account/phone-numbers/search). Once you have a number you need to configure your number to work with your application. Open [the number management page](https://www.twilio.com/user/account/phone-numbers/incoming) and open a number's configuration by clicking on it.

Remember that the number where you change the _SMS webhook_ must be the same one you set on the `TwilioPhoneNumber` setting.

![Configure Voice](http://howtodocs.s3.amazonaws.com/twilio-number-config-all-med.gif)

 To start using `ngrok` in our project you'll have execute to the following line in the _command prompt_:
```
ngrok http 4567 -host-header="localhost:4567"
```

Bear in mind that our endpoint is:
```
http://<your-ngrok-subdomain>.ngrok.io/Reservations/Handle
```


## Create a TwiML App

This project is configured to use a _TwiML App_, which allows us to easily set the voice URLs for all Twilio phone numbers we purchase in this app.

Create a new TwiML app at https://www.twilio.com/user/account/apps/add and use its `Sid` as the `TwiMLApplicationSID` application setting.

![Creating a TwiML App](http://howtodocs.s3.amazonaws.com/call-tracking-twiml-app.gif)

Once you have created your TwiML app, configure your Twilio phone number to use it ([instructions here](https://www.twilio.com/help/faq/twilio-client/how-do-i-create-a-twiml-app)).

If you don't have a Twilio phone number yet, you can purchase a new number in your [Twilio Account Dashboard](https://www.twilio.com/user/account/phone-numbers/incoming).

You'll need to update your TwiML app's voice and SMS URL setting to use your `ngrok` hostname, so it will look something like this:
```
http://<your-ngrok-subdomain>.ngrok.io/PhoneExchange/InterconnectUsingSms
http://<your-ngrok-subdomain>.ngrok.io/PhoneExchange/InterconnectUsingVoice
```

## Local Development

1. Clone this repository and `cd` into its directory:
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
     <add key="TwiMLApplicationSID" value="Your TwiML Application SID" />
   </appSettings>
   ```

3. Build the solution.

4. Run `Update-Database` at [Package Manager
   Console](https://docs.nuget.org/consume/package-manager-console) to execute the migrations.

5. Run the application.

6. Check it out at [http://localhost:4567](http://localhost:4567)

That's it!

To let our Twilio Phone number use the endpoints we exposed, our development server will need to be publicly accessible. [We recommend using ngrok to solve this problem](https://www.twilio.com/blog/2015/09/6-awesome-reasons-to-use-ngrok-when-testing-webhooks.html).

## Meta

* No warranty expressed or implied. Software is as is. Diggity.
* [MIT License](http://www.opensource.org/licenses/mit-license.html)
* Lovingly crafted by Twilio Developer Education.
