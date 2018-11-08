using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirTNG.Web.Domain.Twilio;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;

namespace AirTNG.Web.Domain.Reservations
{
    public interface INotifier
    {
        Task SendNotificationAsync(Reservation reservation);
    }

    public class Notifier : INotifier
    {
        private readonly ITwilioMessageSender _client;
        private readonly IReservationsRepository _repository;

        public Notifier() : this(
            new TwilioMessageSender(),
            new ReservationsRepository()) { }

        public Notifier(ITwilioMessageSender client, IReservationsRepository repository)
        {
            _client = client;
            _repository = repository;
        }

        public async Task SendNotificationAsync(Reservation reservation)
        {
            var pendingReservations = await _repository.FindPendingReservationsAsync();
            if (pendingReservations.Count() < 2)
            {
                var notification = BuildNotification(reservation);
                await _client.SendMessageAsync(notification.To, notification.From, notification.Messsage);
            }
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
