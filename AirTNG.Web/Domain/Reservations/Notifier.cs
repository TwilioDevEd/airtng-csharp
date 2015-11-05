using System;
using System.Linq;
using System.Text;
using AirTNG.Web.Domain.Twilio;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using Twilio;

namespace AirTNG.Web.Domain.Reservations
{
    public interface INotifier
    {
        void SendNotification(Reservation reservation);
    }

    public class Notifier : INotifier
    {
        private readonly TwilioRestClient _client;
        private readonly IReservationsRepository _repository;

        public Notifier() : this(
            new TwilioRestClient(Credentials.AccountSID, Credentials.AuthToken),
            new ReservationsRepository()) { }

        public Notifier(TwilioRestClient client, IReservationsRepository repository)
        {
            _client = client;
            _repository = repository;
        }

        public async void SendNotification(Reservation reservation)
        {
            var pendingReservations = await _repository.FindPendingReservationsAsync();

            // Don't send the message if we have more than one or we aren't being forced
            if (pendingReservations.Count() > 1)
            {
                return;
            }


            var notification = BuildNotification(reservation);
            _client.SendMessage(notification.From, notification.To, notification.Messsage);
        }

        private static Notification BuildNotification(Reservation reservation)
        {
            var message = new StringBuilder();
            message.AppendFormat("You have a new reservation request from {0} for {1}:{2}",
                reservation.Name,
                reservation.VacationProperty.Description,
                Environment.NewLine);
            message.AppendFormat("{0}{1}",
                reservation.Message,
                Environment.NewLine);
            message.Append("Reply [accept] or [reject]");

            return new Notification
            {
                From = PhoneNumbers.Twilio,
                To = reservation.PhoneNumber,
                Messsage = message.ToString()
            };
        }
    }
}