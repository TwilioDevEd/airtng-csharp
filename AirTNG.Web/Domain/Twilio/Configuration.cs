using System.Web.Configuration;

namespace AirTNG.Web.Domain.Twilio
{
    public class Credentials
    {
        public static string AccountSID {
            get
            {
                return WebConfigurationManager.AppSettings["TwilioAccountSid"] ??
                     "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            }
        }

        public static string AuthToken {
            get
            {
                return WebConfigurationManager.AppSettings["TwilioAuthToken"] ??
                    "aXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            }
        }

        public static string ApplicationSID {
            get
            {
                return WebConfigurationManager.AppSettings["TwiMLApplicationSID"] ??
                    "APXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            }
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