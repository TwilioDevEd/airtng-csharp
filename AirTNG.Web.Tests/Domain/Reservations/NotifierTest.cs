using System.Collections.Generic;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using Moq;
using NUnit.Framework;

namespace AirTNG.Web.Tests.Domain.Reservations
{
    public class NotifierTest
    {
        [Test]
        public async void WhenThereAreMoreThanOneReservations_ThenNoMessageIsSent()
        {
            // Given
            var mockMessageSender = new Mock<ITwilioMessageSender>();
            var mockRepository = SetupRepositoryMock(new List<Reservation>() {
                    new Reservation(),
                    new Reservation()
                });
            var notifier = BuildNotifier(mockMessageSender, mockRepository);

            // When
            await notifier.SendNotificationAsync(new Reservation());

            // Then
            mockMessageSender.Verify(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async void WhenThereIsOneOrNoReservation_ThenAMessageIsSent()
        {
            // Given
            const string hostPhoneNumber = "host-phone-number";
            var mockMessageSender = new Mock<ITwilioMessageSender>();
            var mockRepository = SetupRepositoryMock(new List<Reservation>());
            var notifier = BuildNotifier(mockMessageSender, mockRepository);

            // When
            await notifier.SendNotificationAsync(new Reservation
            {
                VacationProperty = new VacationProperty(),
                PhoneNumber = hostPhoneNumber
            });

            // Then
            mockMessageSender.Verify(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        private static Notifier BuildNotifier(Mock<ITwilioMessageSender> mockClient, Mock<IReservationsRepository> mockRepository)
        {
            return new Notifier(mockRepository.Object,
                            mockClient.Object);
        }

        private static Mock<IReservationsRepository> SetupRepositoryMock(List<Reservation> reservations)
        {
            var mockRepository = new Mock<IReservationsRepository>();
            mockRepository
                .Setup(r => r.FindPendingReservationsAsync())
                .ReturnsAsync(reservations);
            return mockRepository;
        }
    }
}
