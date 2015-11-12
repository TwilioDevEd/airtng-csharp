using System.Xml.XPath;
using AirTNG.Web.Controllers;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using AirTNG.Web.Tests.Extensions;
using Moq;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

namespace AirTNG.Web.Tests.Controllers
{
    public class PhoneExchangeControllerTest
    {
        private Mock<IReservationsRepository> _mockRepository;

        [SetUp]
        public void SetUp()
        {
            var reservation = new Reservation
            {
                VacationProperty = new VacationProperty
                {
                    Owner = new ApplicationUser {PhoneNumber = "host-phone-number"}
                },
                Guest = new ApplicationUser {PhoneNumber = "guest-phone-number"},
                AnonymousPhoneNumber = "anonymous-phone-number"
            };

            _mockRepository = new Mock<IReservationsRepository>();
            _mockRepository
                .Setup(r => r.FindByAnonymousPhoneNumberAsync(It.IsAny<string>()))
                .ReturnsAsync(reservation);
        }

        [TestCase("guest-phone-number", "host-phone-number")]
        [TestCase("host-phone-number", "guest-phone-number")]
        public void GivenInterconnectUsingSms_ThenConnectThePartiesUsingSms(
            string incommingPhoneNumber, string outgoingPhoneNumber)
        {

            var controller = new PhoneExchangeController(_mockRepository.Object);
            controller
                .WithCallTo(c => c.InterconnectUsingSms(incommingPhoneNumber, "anonymous-phone-number", "message"))
                .ShouldReturnTwiMLResult(data =>
                {
                    Assert.That(data.XPathSelectElement("Response/Message").Attribute("to").Value,
                        Is.EqualTo(outgoingPhoneNumber));
                    Assert.That(data.XPathSelectElement("Response/Message").Value,
                        Is.EqualTo("message"));
                });
        }

        [TestCase("guest-phone-number", "host-phone-number")]
        [TestCase("host-phone-number", "guest-phone-number")]
        public void GivenInterconnectUsingVoice_ThenConnectThePartiesUsingDial(
            string incommingPhoneNumber, string outgoingPhoneNumber)
        {

            var controller = new PhoneExchangeController(_mockRepository.Object);
            controller
                .WithCallTo(c => c.InterconnectUsingVoice(incommingPhoneNumber, "anonymous-phone-number"))
                .ShouldReturnTwiMLResult(data =>
                {
                    Assert.That(data.XPathSelectElement("Response/Dial").Value,
                        Is.EqualTo(outgoingPhoneNumber));
                });
        }
    }
}
