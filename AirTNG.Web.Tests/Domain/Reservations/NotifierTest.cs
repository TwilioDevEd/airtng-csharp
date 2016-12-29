using System.Collections.Generic;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using Moq;
using NUnit.Framework;
using Twilio;
using Twilio.Clients;
using Twilio.Http;

namespace AirTNG.Web.Tests.Domain.Reservations
{
    public class NotifierTest
    {
        [Test]
        public async void WhenThereAreMoreThanOneReservation_ThenAnyMessageIsSent()
        {
            // Given
            TwilioClient.Init("AccountSid", "AuthToken");
            var mockClient = new Mock<TwilioRestClient>(null, null);
            mockClient
                .Setup(c => c.Request(It.IsAny<Request>()));
            TwilioClient.SetRestClient(mockClient.Object);
            var mockRepository = new Mock<IReservationsRepository>();
            mockRepository
                .Setup(r => r.FindPendingReservationsAsync())
                .ReturnsAsync(new List<Reservation>
                {
                    new Reservation(),
                    new Reservation()
                });

            var notifier = new Notifier(mockRepository.Object);

            // When
            await notifier.SendNotificationAsync(new Reservation());

            // Then
            mockClient.Verify(c => c.Request(It.IsAny<Request>()), Times.Never);
        }

        [Test]
        public async void WhenThereAreLessThanOneReservation_ThenAMessageIsSent()
        {
            // Given
            TwilioClient.Init("AccountSid", "AuthToken");
            var mockClient = new Mock<TwilioRestClient>(null, null);
            mockClient
                .Setup(c => c.Request(It.IsAny<Request>()));
            var mockRepository = new Mock<IReservationsRepository>();
            mockRepository
                .Setup(r => r.FindPendingReservationsAsync())
                .ReturnsAsync(new List<Reservation>());

            var notifier = new Notifier(mockRepository.Object);

            const string hostPhoneNumber = "host-phone-number";

            // When
            await notifier.SendNotificationAsync(new Reservation
            {
                VacationProperty = new VacationProperty(),
                PhoneNumber = hostPhoneNumber
            });

            // Then
            mockClient.Verify(c => c.Request(It.IsAny<Request>()), Times.Once);
        }
    }
}
