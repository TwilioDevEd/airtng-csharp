using System.Web.Configuration;

namespace AirTNG.Web.Domain.Twilio
{
    public class Credentials
    {
        public static string AccountSid => WebConfigurationManager.AppSettings["TwilioAccountSid"] ??
                                           "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

        public static string AuthToken => WebConfigurationManager.AppSettings["TwilioAuthToken"] ??
                                          "aXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
    }


    public class PhoneNumbers
    {
        public static string Twilio => WebConfigurationManager.AppSettings["TwilioPhoneNumber"] ??
                                       "+123456";
    }
}