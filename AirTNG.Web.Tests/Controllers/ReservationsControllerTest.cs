using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.XPath;
using AirTNG.Web.Controllers;
using AirTNG.Web.Domain.NewPhoneNumber;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using AirTNG.Web.ViewModels;
using FluentMvcTesting.Extensions;
using Moq;
using NUnit.Framework;
using TestStack.FluentMVCTesting;
using Twilio.Types;

// ReSharper disable PossibleNullReferenceException

namespace AirTNG.Web.Tests.Controllers
{
    public class ReservationsControllerTest
    {
        [Test]
        public void GivenACreateAction_ThenRendersTheDefaultView()
        {
            var vacationProperty = new VacationProperty {Owner = new ApplicationUser()};
            var mockVacationsRepository = new Mock<IVacationPropertiesRepository>();
            mockVacationsRepository.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(vacationProperty);
            var stubReservationsRepository = Mock.Of<IReservationsRepository>();
            var stubUsersRepository = Mock.Of<IUsersRepository>();
            var stubNotifier = Mock.Of<INotifier>();
            var stubPurchaser = Mock.Of<IPurchaser>();
            var controller = new ReservationsController(
                mockVacationsRepository.Object, stubReservationsRepository, stubUsersRepository, stubNotifier, stubPurchaser);
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
            var stubPurchaser = Mock.Of<IPurchaser>();

            var controller = new ReservationsController(
                stubVacationPropertiesRepository, mockReservationsRepository.Object, stubUsersRepository,
                mockNotifier.Object, stubPurchaser) {UserId = () => "bob-id"};

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
            var stubPurchaser = Mock.Of<IPurchaser>();

            var controller = new ReservationsController(
                stubVacationPropertiesRepository, stubReservationsRepository, stubUsersRepository, stubNotifier, stubPurchaser);
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
            var mockPurchaser = new Mock<IPurchaser>();
            mockPurchaser
                .Setup(p => p.PurchaseAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new PhoneNumber("123")));

            var controller = new ReservationsController(
                stubVacationPropertiesRepository,
                mockReservationsRepository.Object,
                mockUsersRepository.Object,
                stubNotifier,
                mockPurchaser.Object);

            controller.WithCallTo(c => c.Handle("from-number", smsRequestBody))
                .ShouldReturnXmlResult(data =>
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
            var stubPurchaser = Mock.Of<IPurchaser>();

            var controller = new ReservationsController(
                stubVacationPropertiesRepository,
                mockReservationsRepository.Object,
                mockUsersRepository.Object,
                stubNotifier,
                stubPurchaser);

            controller.WithCallTo(c => c.Handle("from-number", "yes"))
                .ShouldReturnXmlResult(data =>
                {
                    StringAssert.Contains("Sorry", data.XPathSelectElement("Response/Message").Value);
                });
        }


        [Test]
        public void GivenAnIndexAction_ThenRendersTheDefaultView()
        {
            var currentUser = new ApplicationUser
            {
                VacationProperties = new List<VacationProperty>
                {
                    new VacationProperty
                    {
                        Reservations = new List<Reservation> {new Reservation()}
                    },
                }
            };
            var stubVacationPropertiesRepository = Mock.Of<IVacationPropertiesRepository>();
            var stubReservationsRepository = Mock.Of<IReservationsRepository>();
            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository
                .Setup(r => r.FindAsync("user-id"))
                .ReturnsAsync(currentUser);
            var stubNotifier = Mock.Of<INotifier>();
            var stubPurchaser = Mock.Of<IPurchaser>();

            var controller = new ReservationsController(
                stubVacationPropertiesRepository,
                stubReservationsRepository,
                mockUsersRepository.Object,
                stubNotifier,
                stubPurchaser) {UserId = () => "user-id"};

            controller.WithCallTo(c => c.Index())
                .ShouldRenderDefaultView()
                .WithModel<IEnumerable<Reservation>>();
        }
    }
}