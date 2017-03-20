using System;
using System.Web.Mvc;
using System.Xml.Linq;
using TestStack.FluentMVCTesting;
using Twilio.AspNet.Mvc;

namespace AirTNG.Web.Tests.Extensions
{
    public static class ControllerResultTestExtensions
    {
        public static TwiMLResult ShouldReturnTwiMLResult<T>(
            this ControllerResultTest<T> controllerResultTest) where T : Controller
        {
            controllerResultTest.ValidateActionReturnType<TwiMLResult>();
            return (TwiMLResult)controllerResultTest.ActionResult;
        }

        public static TwiMLResult ShouldReturnTwiMLResult<T>(
            this ControllerResultTest<T> controllerResultTest,
            Action<XDocument> assertion) where T : Controller
        {
            controllerResultTest.ValidateActionReturnType<ActionResult>();

            var twiMLResult = (TwiMLResult)controllerResultTest.ActionResult;
            var xdocument = twiMLResult.Data as XDocument;

            assertion(xdocument);

            return (TwiMLResult)controllerResultTest.ActionResult;
        }
    }
}
