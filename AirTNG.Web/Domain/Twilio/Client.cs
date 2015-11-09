using System.Collections.Generic;
using Twilio;

namespace AirTNG.Web.Domain.Twilio
{
    public interface IClient
    {
        IEnumerable<AvailablePhoneNumber> SearchPhoneNumbers(string areaCode);
        IncomingPhoneNumber PurchasePhoneNumber(string phoneNumber);
    }

    public class Client : IClient
    {
        private readonly TwilioRestClient _client;

        public Client() : this(new TwilioRestClient(Credentials.AccountSID, Credentials.AuthToken)) { }

        public Client(TwilioRestClient client)
        {
            _client = client;
        }

        public IEnumerable<AvailablePhoneNumber> SearchPhoneNumbers(string areaCode)
        {
            var searchParams = new AvailablePhoneNumberListRequest {AreaCode = areaCode};
            return _client
                .ListAvailableLocalPhoneNumbers("US", searchParams)
                .AvailablePhoneNumbers;
        }

        public IncomingPhoneNumber PurchasePhoneNumber(string phoneNumber)
        {
            var phoneNumberOptions = new PhoneNumberOptions
            {
                PhoneNumber = phoneNumber,
                VoiceApplicationSid = Credentials.ApplicationSID
            };

            return _client.AddIncomingPhoneNumber(phoneNumberOptions);
        }
    }
}