using System.Web.Configuration;

namespace AirTNG.Web.Domain.Twilio
{
    public class Credentials
    {
        public static string AccountSID {
            get { return WebConfigurationManager.AppSettings["TwilioAccountSid"]; }
        }

        public static string AuthToken {
            get { return WebConfigurationManager.AppSettings["TwilioAuthToken"]; }
        }
    }


    public class PhoneNumbers
    {
        public static string Twilio
        {
            get { return WebConfigurationManager.AppSettings["TwilioPhoneNumber"]; }
        }
    }
}