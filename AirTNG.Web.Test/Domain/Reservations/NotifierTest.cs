using System.Collections.Generic;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using Moq;
using NUnit.Framework;
using Twilio;

namespace AirTNG.Web.Test.Domain.Reservations
{
    public class NotifierTest
    {
        [Test]
        public void WhenThereAreMoreThanOneReservation_ThenAnyMessageIsSent()
        {
            var mockClient = new Mock<TwilioRestClient>(null, null);
            mockClient
                .Setup(c => c.SendMessage(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            var mockRepository = new Mock<IReservationsRepository>();
            mockRepository
                .Setup(r => r.FindPendingReservationsAsync())
                .ReturnsAsync(new List<Reservation>
                {
                    new Reservation(),
                    new Reservation()
                });

            var notifier = new Notifier(
                mockClient.Object, mockRepository.Object);

            notifier.SendNotification(new Reservation());

            mockClient.Verify(c => c.SendMessage(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void WhenThereAreLessThanOneReservation_ThenAMessageIsSent()
        {
            var mockClient = new Mock<TwilioRestClient>(null, null);
            mockClient
                .Setup(c => c.SendMessage(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            var mockRepository = new Mock<IReservationsRepository>();
            mockRepository
                .Setup(r => r.FindPendingReservationsAsync())
                .ReturnsAsync(new List<Reservation>());

            var notifier = new Notifier(
                mockClient.Object, mockRepository.Object);

            const string hostPhoneNumber = "host-phone-number";
            notifier.SendNotification(new Reservation
            {
                VacationProperty = new VacationProperty(),
                PhoneNumber = hostPhoneNumber
            });

            mockClient.Verify(c => c.SendMessage(
                It.IsAny<string>(), hostPhoneNumber, It.IsAny<string>()), Times.Once);
        }
    }
}
