using System.Collections.Generic;
using AirTNG.Web.Domain.NewPhoneNumber;
using AirTNG.Web.Models;
using Moq;
using NUnit.Framework;
using Twilio;
using Twilio.Clients;
using Twilio.Http;
using TwilioRestClient = Twilio.TwilioRestClient;

namespace AirTNG.Web.Tests.Domain.PhoneNumber
{
    public class PurchaserTest
    {
        private const string availablePhoneNumberResponse = 
            @"<TwilioResponse>
                <AvailablePhoneNumbers uri=""/2010-04-01/Accounts/ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX/AvailablePhoneNumbers/US/Local?AreaCode=510"">
                <AvailablePhoneNumber>
                    <FriendlyName>(510) 564-7903</FriendlyName>
                    <PhoneNumber>+15105647903</PhoneNumber>
                    <Lata>722</Lata>
                    <RateCenter>OKLD TRNID</RateCenter>
                    <Latitude>37.780000</Latitude>
                    <Longitude>-122.380000</Longitude>
                    <Region>CA</Region>
                    <PostalCode>94703</PostalCode>
                    <IsoCountry>US</IsoCountry>
                    <Capabilities>
                    <Voice>true</Voice>
                    <SMS>true</SMS>
                    <MMS>false</SMS>
                    </Capabilities>
                    <Beta>false</Beta>
                </AvailablePhoneNumber>
                </AvailablePhoneNumbers>
            </TwilioResponse>";


        [Test]
        public void WhenThereAreAvailablePhoneNumbers_APhoneNumberIsPurchased()
        {
            var mockClient = new Mock<ITwilioRestClient>();

            mockClient
               .Setup(c => c.Request(It.Is<Request>(
                   r => Equals(r.Method, HttpMethod.Get) &&
                   r.ConstructUrl().AbsoluteUri.Contains("/AvailablePhoneNumbers/US/Local.json"))
                   ))
               .Returns(new Response(System.Net.HttpStatusCode.OK, availablePhoneNumberResponse));

            mockClient
                .Setup(c => c.Request(It.Is<Request>(
                   r => Equals(r.Method, HttpMethod.Post) &&
                   r.ConstructUrl().AbsoluteUri.Contains("/IncomingPhoneNumbers.json"))
                   ))
               .Returns(new Response(System.Net.HttpStatusCode.OK, ""));
            
            var client = new Purchaser(mockClient.Object);

            client.Purchase("area-code");

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