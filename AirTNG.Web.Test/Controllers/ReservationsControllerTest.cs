using AirTNG.Web.Controllers;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
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

            var controller = new ReservationsController(
                mockVacationsRepository.Object, stubReservationsRepository);
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

            var controller = new ReservationsController(
                stubVacationPropertiesRepository, mockReservationsRepository.Object);

            controller.WithCallTo(c => c.Create(model))
                .ShouldRedirectTo<VacationPropertiesController>(c => c.Index());

            mockReservationsRepository.Verify(r => r.CreateAsync(It.IsAny<Reservation>()), Times.Once);
        }

        [Test]
        public void GivenACreateAction_WhenTheModelStateIsInalid_ThenRenderTheDefaultView()
        {
            var model = new ReservationViewModel();
            var stubVacationPropertiesRepository = Mock.Of<IVacationPropertiesRepository>();
            var stubReservationsRepository = Mock.Of<IReservationsRepository>();

            var controller = new ReservationsController(
                stubVacationPropertiesRepository, stubReservationsRepository);
            controller.ModelState.AddModelError("Message", "The Message field is required");

            controller.WithCallTo(c => c.Create(model))
                .ShouldRenderDefaultView();
        }
    }
}
