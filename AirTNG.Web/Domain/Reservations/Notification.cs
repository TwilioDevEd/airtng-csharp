using Twilio.Types;

namespace AirTNG.Web.Domain.Reservations
{
    public class Notification
    {
        public PhoneNumber From { get; set; }
        public PhoneNumber To { get; set; }
        public string Messsage { get; set; }
    }
}