using AirTNG.Web.Domain.Twilio;
using Moq;
using NUnit.Framework;
using Twilio;

namespace AirTNG.Web.Tests.Domain.Twilio
{
    public class ClientTest
    {
        [Test]
        public void GivenSearchPhoneNumbers_ThenReturnAvailablePhoneNumbers()
        {
            var mockClient = new Mock<TwilioRestClient>(null, null);
            mockClient
                .Setup(c => c.ListAvailableLocalPhoneNumbers("US", It.IsAny<AvailablePhoneNumberListRequest>()))
                .Returns(new AvailablePhoneNumberResult());
            var client = new Client(mockClient.Object);

            client.SearchPhoneNumbers("area-code");

            mockClient.Verify(c => c.ListAvailableLocalPhoneNumbers("US", It.IsAny<AvailablePhoneNumberListRequest>()),
                Times.Once);
        }

        [Test]
        public void GivenPurchasePhoneNumber_ThenAddIncomingNumberIsCalled()
        {
            var mockClient = new Mock<TwilioRestClient>(null, null);
            var client = new Client(mockClient.Object);

            client.PurchasePhoneNumber("phone-number");

            mockClient.Verify(c => c.AddIncomingPhoneNumber(It.IsAny<PhoneNumberOptions>()), Times.Once);
        }
    }
}