using System.Threading.Tasks;
using System.Web.Mvc;
using AirTNG.Web.Models.Repository;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace AirTNG.Web.Controllers
{
    public class PhoneExchangeController : TwilioController
    {
        private readonly IReservationsRepository _repository;

        public PhoneExchangeController() : this(new ReservationsRepository()) { }

        public PhoneExchangeController(IReservationsRepository repository)
        {
            _repository = repository;
        }

        // POST: PhoneExchange/InterconnectUsingSms
        [HttpPost]
        public async Task<ActionResult> InterconnectUsingSms(string from, string to)
        {
            var outgoingPhoneNumber = await GatherOutgoingPhoneNumberAsync(from, to);

            var response = new TwilioResponse();
            response.Message("message", new {to = outgoingPhoneNumber});

            return TwiML(response);
        }

        // POST: PhoneExchange/InterconnectUsingVoice
        [HttpPost]
        public async Task<ActionResult> InterconnectUsingVoice(string from, string to)
        {
            var outgoingPhoneNumber = await GatherOutgoingPhoneNumberAsync(from, to);

            var response = new TwilioResponse();
            response.Play("http://howtodocs.s3.amazonaws.com/howdy-tng.mp3");
            response.Dial(outgoingPhoneNumber);

            return TwiML(response);
        }

        private async Task<string> GatherOutgoingPhoneNumberAsync(
            string incomingPhoneNumber, string anonymousPhoneNumber)
        {
            var outgoingPhoneNumber = string.Empty;
            var reservation = await _repository.FindByAnonymousPhoneNumberAsync(anonymousPhoneNumber);

            // Connect from Guest to Host
            if (reservation.Guest.PhoneNumber.Equals(incomingPhoneNumber))
            {
                outgoingPhoneNumber = reservation.Host.PhoneNumber;
            }

            // Connect from Host to Guest
            if (reservation.Host.PhoneNumber.Equals(incomingPhoneNumber))
            {
                outgoingPhoneNumber = reservation.Guest.PhoneNumber;
            }

            return outgoingPhoneNumber;
        }
    }
}