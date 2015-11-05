using AirTNG.Web.Controllers;
using TestStack.FluentMVCTesting;
using Twilio.TwiML.Mvc;

namespace AirTNG.Web.Test.Utils
{
    public static class Exttensions
    {
        public static TwiMLResult ShouldReturnTwiMLResult(this ControllerResultTest<ReservationsController> controllerResultTest)
        {
            controllerResultTest.ValidateActionReturnType<TwiMLResult>();
            return (TwiMLResult) controllerResultTest.ActionResult;
        }
    }
}
