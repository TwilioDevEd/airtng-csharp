using System;
using System.Xml.XPath;
using AirTNG.Web.Controllers;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using AirTNG.Web.Test.Extensions;
using AirTNG.Web.ViewModels;
using Moq;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

// ReSharper disable PossibleNullReferenceException

namespace AirTNG.Web.Test.Controllers
{
    public class ReservationsControllerTest
    {
        [Test]
        public void GivenACreateAction_ThenRendersTheDefaultView()
        {
            var vacationProperty = new VacationProperty {User = new ApplicationUser()};
            var mockVacationsRepository = new Mock<IVacationPropertiesRepository>();
            mockVacationsRepository.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(vacationProperty);
            var stubReservationsRepository = Mock.Of<IReservationsRepository>();
            var stubUsersRepository = Mock.Of<IUsersRepository>();
            var stubNotifier = Mock.Of<INotifier>();
            var controller = new ReservationsController(
                mockVacationsRepository.Object, stubReservationsRepository, stubUsersRepository, stubNotifier);
            controller.WithCallTo(c => c.Create(1))
                .ShouldRenderDefaultView();
        }

        [Test]
        public void GivenACreateAction_WhenTheModelStateIsValid_ThenItRedirectsToVacationPropertiesIndex()
        {
            var model = new ReservationViewModel();

            var stubVacationPropertiesRepository = Mock.Of<IVacationPropertiesRepository>();
            var mockReservationsRepository = new Mock<IReservationsRepository>();
            var stubUsersRepository = Mock.Of<IUsersRepository>();
            var mockNotifier = new Mock<INotifier>();

            var controller = new ReservationsController(
                stubVacationPropertiesRepository, mockReservationsRepository.Object, stubUsersRepository,
                mockNotifier.Object);

            controller.WithCallTo(c => c.Create(model))
                .ShouldRedirectTo<VacationPropertiesController>(c => c.Index());

            mockReservationsRepository.Verify(r => r.CreateAsync(It.IsAny<Reservation>()), Times.Once);
            mockNotifier.Verify(n => n.SendNotificationAsync(It.IsAny<Reservation>()), Times.Once());
        }

        [Test]
        public void GivenACreateAction_WhenTheModelStateIsInalid_ThenRenderTheDefaultView()
        {
            var model = new ReservationViewModel();
            var stubVacationPropertiesRepository = Mock.Of<IVacationPropertiesRepository>();
            var stubReservationsRepository = Mock.Of<IReservationsRepository>();
            var stubUsersRepository = Mock.Of<IUsersRepository>();
            var stubNotifier = Mock.Of<INotifier>();

            var controller = new ReservationsController(
                stubVacationPropertiesRepository, stubReservationsRepository, stubUsersRepository, stubNotifier);
            controller.ModelState.AddModelError("Message", "The Message field is required");

            controller.WithCallTo(c => c.Create(model))
                .ShouldRenderDefaultView();
        }

        [TestCase("yes", "Confirmed")]
        [TestCase("no", "Rejected")]
        public void GivenAHandleAction_WhenThereAreAPendingReservation_AndTheUserRespondsYesOrNo_ThenRespondWithReservationStatus(
            string smsRequestBody, string expectedMessage)
        {
            var host = new ApplicationUser {Id = "user-id"};
            var stubVacationPropertiesRepository = Mock.Of<IVacationPropertiesRepository>();
            var mockUsersRepository = new Mock<IUsersRepository>();
            var mockReservationsRepository = new Mock<IReservationsRepository>();
            mockReservationsRepository
                .Setup(r => r.FindFirstPendingReservationByHostAsync(host.Id))
                .ReturnsAsync(new Reservation());
            mockUsersRepository
                .Setup(r => r.FindByPhoneNumberAsync(It.IsAny<string>()))
                .ReturnsAsync(host);
            var stubNotifier = Mock.Of<INotifier>();

            var controller = new ReservationsController(
                stubVacationPropertiesRepository,
                mockReservationsRepository.Object,
                mockUsersRepository.Object,
                stubNotifier);

            controller.WithCallTo(c => c.Handle("from-number", smsRequestBody))
                .ShouldReturnTwiMLResult(data =>
                {
                    StringAssert.Contains(expectedMessage, data.XPathSelectElement("Response/Message").Value);
                });
        }

        [Test]
        public void GivenAHandleAction_WhenThereAreNoPendingReservations_TheResponseContainsSorryMessage()
        {
            var host = new ApplicationUser {Id = "user-id"};
            var stubVacationPropertiesRepository = Mock.Of<IVacationPropertiesRepository>();
            var mockUsersRepository = new Mock<IUsersRepository>();
            var mockReservationsRepository = new Mock<IReservationsRepository>();
            mockReservationsRepository
                .Setup(r => r.FindFirstPendingReservationByHostAsync(host.Id))
                .ThrowsAsync(new InvalidOperationException()); // There are no reservations
            mockUsersRepository
                .Setup(r => r.FindByPhoneNumberAsync(It.IsAny<string>()))
                .ReturnsAsync(host);
            var stubNotifier = Mock.Of<INotifier>();

            var controller = new ReservationsController(
                stubVacationPropertiesRepository,
                mockReservationsRepository.Object,
                mockUsersRepository.Object,
                stubNotifier);

            controller.WithCallTo(c => c.Handle("from-number", "yes"))
                .ShouldReturnTwiMLResult(data =>
                {
                    StringAssert.Contains("Sorry", data.XPathSelectElement("Response/Message").Value);
                });
        }
    }
}