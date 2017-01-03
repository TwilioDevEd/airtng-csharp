using AirTNG.Web.Domain.NewPhoneNumber;
using Moq;
using NUnit.Framework;
using Twilio.Clients;
using Twilio.Http;

namespace AirTNG.Web.Tests.Domain.PhoneNumber
{
    public class PurchaserTest
    {
        private const string readAvailablePhoneNumberResponse =
            @"{
              'uri': '\/2010-04-01\/Accounts\/ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\/AvailablePhoneNumbers\/US\/Local.json?AreaCode=510',
              'page_size': 1,
              'first_page_uri': 'first-page',
              'next_page_uri': 'next-page',
              'previous_page_uri': 'previous-page',
              'available_phone_numbers': [
                {
                  'friendly_name': '(510) 564-7903',
                  'phone_number': '+15105647903',
                  'lata': '722',
                  'rate_center': 'OKLD TRNID',
                  'latitude': '37.780000',
                  'longitude': '-122.380000',
                  'region': 'CA',
                  'postal_code': '94703',
                  'iso_country': 'US',
                  'capabilities':{
                    'voice': true,
                    'SMS': true,
                    'MMS': false
                  },
                  'beta': false
                }    
              ]
            }";

        private const string createIncomingPhoneNumberResponse =
            @"{
                  'sid': 'PN2a0747eba6abf96b7e3c3ff0b4530f6e',
                  'account_sid': 'ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX',
                  'friendly_name': 'My Company Line',
                  'phone_number': '+15105647903',
                  'voice_url': 'http://demo.twilio.com/docs/voice.xml',
                  'voice_method': 'POST',
                  'voice_fallback_url': null,
                  'voice_fallback_method': 'POST',
                  'voice_caller_id_lookup': null,
                  'voice_application_sid': null,
                  'date_created': 'Mon, 16 Aug 2010 23:31:47 +0000',
                  'date_updated': 'Mon, 16 Aug 2010 23:31:47 +0000',
                  'sms_url': null,
                  'sms_method': 'POST',
                  'sms_fallback_url': null,
                  'sms_fallback_method': 'GET',
                  'sms_application_sid': null,
                  'beta': false,
                  'status_callback': null,
                  'status_callback_method': null,
                  'api_version': '2010-04-01',
                  'uri': '\/2010-04-01\/Accounts\/ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\/IncomingPhoneNumbers\/PN2a0747eba6abf96b7e3c3ff0b4530f6e.json'
                }";

        [Test]
        public void WhenThereAreAvailablePhoneNumbers_APhoneNumberIsPurchased()
        {
            var mockClient = new Mock<ITwilioRestClient>();

            mockClient
               .Setup(c => c.Request(It.Is<Request>(
                   r => Equals(r.Method, HttpMethod.Get) &&
                   r.ConstructUrl().AbsoluteUri.Contains("/AvailablePhoneNumbers/US/Local.json"))
                   ))
               .Returns(new Response(System.Net.HttpStatusCode.OK, readAvailablePhoneNumberResponse));

            mockClient
                .Setup(c => c.Request(It.Is<Request>(
                   r => Equals(r.Method, HttpMethod.Post) &&
                   r.ConstructUrl().AbsoluteUri.Contains("/IncomingPhoneNumbers.json"))
                   ))
               .Returns(new Response(System.Net.HttpStatusCode.OK, createIncomingPhoneNumberResponse));
            
            var client = new Purchaser(mockClient.Object);

            client.Purchase("1");

            mockClient.Verify(c => c.Request(It.Is<Request>(
                   r => Equals(r.Method, HttpMethod.Get) &&
                   r.ConstructUrl().AbsoluteUri.Contains("/AvailablePhoneNumbers/US/Local.json"))
                   ), Times.Once);

            mockClient.Verify(c => c.Request(It.Is<Request>(
                   r => Equals(r.Method, HttpMethod.Post) &&
                   r.ConstructUrl().AbsoluteUri.Contains("/IncomingPhoneNumbers.json"))
                   ), Times.Once);
        }

       
    }
}