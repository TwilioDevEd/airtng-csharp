using System.Collections.Generic;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using Moq;
using NUnit.Framework;
using Twilio.Clients;
using Twilio.Http;

namespace AirTNG.Web.Tests.Domain.Reservations
{
    public class NotifierTest
    {
        [Test]
        public async void WhenThereAreMoreThanOneReservations_ThenNoMessageIsSent()
        {
            // Given
            var mockClient = new Mock<ITwilioRestClient>();
            var mockRepository = SetupRepositoryMock(new List<Reservation>() {
                    new Reservation(),
                    new Reservation()
                });
            var notifier = BuildNotifier(mockClient, mockRepository);

            // When
            await notifier.SendNotificationAsync(new Reservation());

            // Then
            mockClient.Verify(c => c.Request(It.IsAny<Request>()), Times.Never);
        }

        [Test]
        public async void WhenThereIsOneOrNoReservation_ThenAMessageIsSent()
        {
            // Given
            const string hostPhoneNumber = "host-phone-number";
            var mockClient = SetupTwilioRestClientMock();
            var mockRepository = SetupRepositoryMock(new List<Reservation>());
            var notifier = BuildNotifier(mockClient, mockRepository);
            var owner = new ApplicationUser {PhoneNumber = "123"};

            // When
            await notifier.SendNotificationAsync(new Reservation
            {
                VacationProperty = new VacationProperty { Owner = owner},
                AnonymousPhoneNumber = hostPhoneNumber,
                Guest = new ApplicationUser { Name = "guest" }
            });

            // Then
            mockClient.Verify(c => c.Request(It.IsAny<Request>()), Times.Once);
        }

        private static Notifier BuildNotifier(Mock<ITwilioRestClient> mockClient, Mock<IReservationsRepository> mockRepository)
        {
            return new Notifier(mockRepository.Object,
                            mockClient.Object,
                            "AccountSid",
                            "AuthToken");
        }

        private static Mock<IReservationsRepository> SetupRepositoryMock(List<Reservation> reservations)
        {
            var mockRepository = new Mock<IReservationsRepository>();
            mockRepository
                .Setup(r => r.FindPendingReservationsAsync())
                .ReturnsAsync(reservations);
            return mockRepository;
        }

        private static Mock<ITwilioRestClient> SetupTwilioRestClientMock()
        {
            var mockClient = new Mock<ITwilioRestClient>();
            mockClient
                .Setup(c => c.Request(It.Is<Request>(
                    r => r.Method == HttpMethod.Post &&
                    r.ConstructUrl().AbsoluteUri.Contains("Messages.json"))
                    ))
                .Returns(new Response(System.Net.HttpStatusCode.OK, ""));
            return mockClient;
        }
    }
}
