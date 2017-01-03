using System;
using System.ComponentModel;
using System.Linq;
using AirTNG.Web.Domain.Twilio;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;
using Twilio.Types;

namespace AirTNG.Web.Domain.NewPhoneNumber
{
    public interface IPurchaser
    {
        PhoneNumber Purchase(string areaCode);
    }

    public class Purchaser : IPurchaser
    {
        private readonly TwilioRestClient _client;

        public Purchaser()
        {
            TwilioClient.Init(Credentials.AccountSID, Credentials.AuthToken);
        }

        public Purchaser(ITwilioRestClient restClient)
        {
            TwilioClient.Init(Credentials.AccountSID, Credentials.AuthToken);
            TwilioClient.SetRestClient(restClient);
        }

        /// <summary>
        /// Purchase the first available phone number.
        /// </summary>
        /// <param name="areaCode">The area code</param>
        /// <returns>The purchased phone number</returns>
        public PhoneNumber Purchase(string areaCode)
        {
            return IncomingPhoneNumberResource
                .Create(phoneNumber: SearchForFirstAvailablePhoneNumber(areaCode),
                        voiceApplicationSid: Credentials.ApplicationSID)
                .PhoneNumber;
        }

        private PhoneNumber SearchForFirstAvailablePhoneNumber(string areaCode)
        {
            return LocalResource.Read("US", areaCode: Int32.Parse(areaCode), voiceEnabled: true, smsEnabled: true)
                .First() // We're only interested in the first available phone number.
                .PhoneNumber;
        }
    }
}