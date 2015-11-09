using System.Reflection;
using Twilio.TwiML.Mvc;

namespace AirTNG.Web.Tests.Extensions
{
    public static class TwiMLResultExtensions
    {
        public static object Data(this TwiMLResult twiMLResult)
        {
            return GetInstanceField(typeof (TwiMLResult), twiMLResult, "data");
        }

        private static object GetInstanceField(IReflect type, object instance, string fieldName)
        {
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            var field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
    }
}