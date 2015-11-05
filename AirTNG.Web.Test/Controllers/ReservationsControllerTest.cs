using AirTNG.Web.Controllers;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using AirTNG.Web.Test.Utils;
using AirTNG.Web.ViewModels;
using Moq;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

namespace AirTNG.Web.Test.Controllers
{
    public class ReservationsControllerTest
    {
        [Test]
        public void GivenACreateAction_ThenRendersTheDefaultView()
        {
            var vacationProperty = new VacationProperty
            {
                User = new ApplicationUser()
            };
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
            mockReservationsRepository.Setup(r => r.CreateAsync(It.IsAny<Reservation>())).ReturnsAsync(1);
            var stubUsersRepository = Mock.Of<IUsersRepository>();
            var mockNotifier = new Mock<INotifier>();
            mockNotifier.Setup(n => n.SendNotification(It.IsAny<Reservation>()));

            var controller = new ReservationsController(
                stubVacationPropertiesRepository, mockReservationsRepository.Object, stubUsersRepository,
                mockNotifier.Object);

            controller.WithCallTo(c => c.Create(model))
                .ShouldRedirectTo<VacationPropertiesController>(c => c.Index());

            mockReservationsRepository.Verify(r => r.CreateAsync(It.IsAny<Reservation>()), Times.Once);
            mockNotifier.Verify(n => n.SendNotification(It.IsAny<Reservation>()), Times.Once());
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

        [Test]
        public void GivenAHandleAction()
        {
            var host = new ApplicationUser
            {
                Id = "user-id"
            };

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

            controller.WithCallTo(c => c.Handle("from-number", "yes"))
                .ShouldReturnTwiMLResult();
        }
    }
}