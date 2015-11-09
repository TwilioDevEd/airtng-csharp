using System.Collections.Generic;
using AirTNG.Web.Domain.PhoneNumber;
using Moq;
using NUnit.Framework;
using Twilio;

namespace AirTNG.Web.Tests.Domain.PhoneNumber
{
    public class PurchaserTest
    {
        [Test]
        public void WhenThereAreAvailablePhoneNumbers_APhoneNumberIsPurchased()
        {
            var mockClient = new Mock<TwilioRestClient>(null, null);
            mockClient
                .Setup(c => c.ListAvailableLocalPhoneNumbers("US", It.IsAny<AvailablePhoneNumberListRequest>()))
                .Returns(new AvailablePhoneNumberResult
                {
                    AvailablePhoneNumbers = new List<AvailablePhoneNumber>
                    {
                        new AvailablePhoneNumber {PhoneNumber = "first-available-phone-number"}
                    }
                });
            var client = new Purchaser(mockClient.Object);

            client.Purchase("area-code");

            mockClient.Verify(c => c.ListAvailableLocalPhoneNumbers("US", It.IsAny<AvailablePhoneNumberListRequest>()),
                Times.Once);

            mockClient.Verify(c => c.AddIncomingPhoneNumber(It.IsAny<PhoneNumberOptions>()), Times.Once);
        }
    }
}