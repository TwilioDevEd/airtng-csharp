using System.Collections.Generic;
using AirTNG.Web.Controllers;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using AirTNG.Web.ViewModels;
using Moq;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

namespace AirTNG.Web.Test.Controllers
{
    public class VacationPropertiesControllerTest
    {
        [Test]
        public void GivenAnIndexAction_ThenRendersTheDefaultView()
        {
            var properties = new List<VacationProperty>
            {
                new VacationProperty {Description = "bob's property"}
            };

            var mockRepository = new Mock<IVacationPropertiesRepository>();
            mockRepository.Setup(r => r.AllAsync()).ReturnsAsync(properties);

            var controller = new VacationPropertiesController(mockRepository.Object);
            controller.WithCallTo(c => c.Index())
                .ShouldRenderDefaultView()
                .WithModel(properties);
        }

        [Test]
        public void GivenACreateAction_ThenRendersTheDefaultView()
        {
            var stubRepository = Mock.Of<IVacationPropertiesRepository>();
            var controller = new VacationPropertiesController(stubRepository);
            controller.WithCallTo(c => c.Create())
                .ShouldRenderDefaultView();
        }

        [Test]
        public void GivenACreateAction_WhenTheModelStateIsValid_ThenItRedirectsToIndex()
        {
            var model = new VacationPropertyViewModel();
            var mockRepository = new Mock<IVacationPropertiesRepository>();
            mockRepository.Setup(r => r.CreateAsync(It.IsAny<VacationProperty>())).ReturnsAsync(1);

            var controller = new VacationPropertiesController(mockRepository.Object)
            {
                UserId = () => "bob-id"
            };

            controller.WithCallTo(c => c.Create(model))
                .ShouldRedirectTo(c => c.Index());

            mockRepository.Verify(r => r.CreateAsync(It.IsAny<VacationProperty>()), Times.Once);
        }

        [Test]
        public void GivenACreateAction_WhenTheModelStateIsInalid_ThenRenderTheDefaultView()
        {
            var model = new VacationPropertyViewModel();
            var stubRepository = Mock.Of<IVacationPropertiesRepository>();

            var controller = new VacationPropertiesController(stubRepository);
            controller.ModelState.AddModelError("Description", "The Description field is required");
            controller.WithCallTo(c => c.Create(model))
                .ShouldRenderDefaultView();
        }

        [Test]
        public void GivenAnEditAction_ThenRendersTheDefaultView()
        {
            var vacationProperty = new VacationProperty();
            var mockRepository = new Mock<IVacationPropertiesRepository>();
            mockRepository.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(vacationProperty);

            var controller = new VacationPropertiesController(mockRepository.Object);
            controller.WithCallTo(c => c.Edit(1))
                .ShouldRenderDefaultView()
                .WithModel(vacationProperty);
        }

        [Test]
        public void GivenAnEditAction_WhenTheModelStateIsValid_ThenItRedirectsToIndex()
        {
            var model = new VacationPropertyViewModel();
            var vacationProperty = new VacationProperty();

            var mockRepository = new Mock<IVacationPropertiesRepository>();
            mockRepository.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(vacationProperty);
            mockRepository.Setup(r => r.UpdateAsync(vacationProperty)).ReturnsAsync(1);

            var controller = new VacationPropertiesController(mockRepository.Object);

            controller.WithCallTo(c => c.Edit(model))
                .ShouldRedirectTo(c => c.Index());

            mockRepository.Verify(r => r.UpdateAsync(It.IsAny<VacationProperty>()), Times.Once);
        }

        [Test]
        public void GivenAnEditAction_WhenTheModelStateIsInalid_ThenRenderTheDefaultView()
        {
            var model = new VacationPropertyViewModel();
            var stubRepository = Mock.Of<IVacationPropertiesRepository>();

            var controller = new VacationPropertiesController(stubRepository);
            controller.ModelState.AddModelError("Description", "The Description field is required");

            controller.WithCallTo(c => c.Edit(model))
                .ShouldRenderDefaultView();
        }

        [Test]
        public void GivenADetailsAction_ThenRendersTheDefaultView()
        {
            var vacationProperty = new VacationProperty();
            var mockRepository = new Mock<IVacationPropertiesRepository>();
            mockRepository.Setup(r => r.FindAsync(It.IsAny<int>())).ReturnsAsync(vacationProperty);

            var controller = new VacationPropertiesController(mockRepository.Object);
            controller.WithCallTo(c => c.Details(1))
                .ShouldRenderDefaultView()
                .WithModel(vacationProperty);
        }
    }
}
