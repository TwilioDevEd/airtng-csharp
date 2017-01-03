using System;
using System.Web.Mvc;
using System.Xml.Linq;
using TestStack.FluentMVCTesting;

namespace AirTNG.Web.Tests.Extensions
{
    public static class ControllerResultTestExtensions
    {
        public static ContentResult ShouldReturnXmlResult<T>(
            this ControllerResultTest<T> controllerResultTest) where T : Controller
        {
            controllerResultTest.ValidateActionReturnType<ContentResult>();

            var contentResult = (ContentResult)controllerResultTest.ActionResult;
            ValidateXmlContentType(contentResult);

            return (ContentResult)controllerResultTest.ActionResult;
        }

        public static ContentResult ShouldReturnXmlResult<T>(
            this ControllerResultTest<T> controllerResultTest,
            Action<XDocument> assertion) where T : Controller
        {
            controllerResultTest.ValidateActionReturnType<ContentResult>();

            var contentResult = (ContentResult)controllerResultTest.ActionResult;
            ValidateXmlContentType(contentResult);

            var xdocument = XDocument.Parse(contentResult.Content);
            assertion(xdocument);

            return (ContentResult)controllerResultTest.ActionResult;
        }

        private static void ValidateXmlContentType(ContentResult contentResult)
        {
            const string xmlContentType = "text/xml";
            if (!contentResult.ContentType.Equals(xmlContentType))
            {
                throw new ActionResultAssertionException(
                    $"Expected content type to be '{xmlContentType}', but instead was '{contentResult.ContentType}'.");
            }
        }
    }
}