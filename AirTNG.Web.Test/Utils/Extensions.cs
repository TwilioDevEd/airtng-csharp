using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using AirTNG.Web.Controllers;
using Moq;
using TestStack.FluentMVCTesting;
using Twilio.TwiML.Mvc;

namespace AirTNG.Web.Test.Utils
{
    public static class Exttensions
    {
        public static TwiMLResult ShouldReturnTwiMLResult<T>(
            this ControllerResultTest<T> controllerResultTest) where T : Controller
        {
            controllerResultTest.ValidateActionReturnType<TwiMLResult>();
            return (TwiMLResult) controllerResultTest.ActionResult;
        }

        public static TwiMLResult ShouldReturnTwiMLResult<T>(
            this ControllerResultTest<T> controllerResultTest,
            Action<XmlDocument> assertion) where T : Controller
        {
            controllerResultTest.ValidateActionReturnType<TwiMLResult>();

            var twiMLResult = (TwiMLResult) controllerResultTest.ActionResult;
            var document = CreateDocument(ReadDataFromActionResult(twiMLResult));
            assertion(document);

            return (TwiMLResult) controllerResultTest.ActionResult;
        }

        private static string ReadDataFromActionResult(ActionResult actionResult)
        {
            var data = new StringBuilder();

            var mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Setup(s => s.Write(It.IsAny<string>())).Callback<string>(c => data.Append(c));
            mockResponse.Setup(s => s.Output).Returns(new StringWriter(data));

            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Setup(x => x.HttpContext.Response).Returns(mockResponse.Object);

            actionResult.ExecuteResult(mockControllerContext.Object);

            return data.ToString();
        }

        private static XmlDocument CreateDocument(string xml)
        {
            var document = new XmlDocument();
            document.LoadXml(xml);

            return document;
        }
    }
}
