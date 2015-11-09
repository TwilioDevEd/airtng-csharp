using System.Linq;
using AirTNG.Web.Domain.Twilio;
using Twilio;

namespace AirTNG.Web.Domain.PhoneNumber
{
    public interface IPurchaser
    {
        IncomingPhoneNumber Purchase(string areaCode);
    }

    public class Purchaser : IPurchaser
    {
        private readonly TwilioRestClient _client;

        public Purchaser() : this(new TwilioRestClient(Credentials.AccountSID, Credentials.AuthToken)) { }

        public Purchaser(TwilioRestClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Purchase the first available phone number.
        /// </summary>
        /// <param name="areaCode">The area code</param>
        /// <returns>The purchased phone number</returns>
        public IncomingPhoneNumber Purchase(string areaCode)
        {
            var phoneNumberOptions = new PhoneNumberOptions
            {
                PhoneNumber = SearchForFirstAvailablePhoneNumber(areaCode),
                VoiceApplicationSid = Credentials.ApplicationSID
            };

            return _client.AddIncomingPhoneNumber(phoneNumberOptions);
        }

        private string SearchForFirstAvailablePhoneNumber(string areaCode)
        {
            var searchParams = new AvailablePhoneNumberListRequest
            {
                AreaCode = areaCode,
                VoiceEnabled = true,
                SmsEnabled = true
            };

            return _client
                .ListAvailableLocalPhoneNumbers("US", searchParams)
                .AvailablePhoneNumbers
                .First() // We're only interested in the first available phone number.
                .PhoneNumber;
        }
    }
}