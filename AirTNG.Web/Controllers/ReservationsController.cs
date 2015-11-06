using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using AirTNG.Web.ViewModels;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace AirTNG.Web.Controllers
{
    public class ReservationsController : TwilioController
    {
        private readonly IVacationPropertiesRepository _vacationPropertiesRepository;
        private readonly IReservationsRepository _reservationsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly INotifier _notifier;

        public ReservationsController() : this(
            new VacationPropertiesRepository(),
            new ReservationsRepository(),
            new UsersRepository(),
            new Notifier()) { }

        public ReservationsController(
            IVacationPropertiesRepository vacationPropertiesRepository,
            IReservationsRepository reservationsRepository,
            IUsersRepository usersRepository,
            INotifier notifier)
        {
            _vacationPropertiesRepository = vacationPropertiesRepository;
            _reservationsRepository = reservationsRepository;
            _usersRepository = usersRepository;
            _notifier = notifier;
        }

        // GET: Reservations/Create
        public async Task<ActionResult> Create(int id)
        {
            var vacationProperty = await _vacationPropertiesRepository.FindAsync(id);
            var reservation = new ReservationViewModel
            {
                ImageUrl = vacationProperty.ImageUrl,
                Description = vacationProperty.Description,
                VacationPropertyId = vacationProperty.Id,
                VacationPropertyDescription = vacationProperty.Description,
                UserName = vacationProperty.User.Name,
                UserPhoneNumber = vacationProperty.User.PhoneNumber,
            };

            return View(reservation);
        }

        // POST: Reservations/Create
        [HttpPost]
        public async Task<ActionResult> Create(ReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var reservation = new Reservation
                {
                    Message = model.Message,
                    PhoneNumber = model.UserPhoneNumber,
                    Name = model.UserName,
                    VactionPropertyId = model.VacationPropertyId,
                    Status = ReservationStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                await _reservationsRepository.CreateAsync(reservation);
                reservation.VacationProperty = new VacationProperty {Description = model.VacationPropertyDescription};
                await _notifier.SendNotificationAsync(reservation);

                return RedirectToAction("Index", "VacationProperties");
            }

            return View();
        }

        // POST Reservations/Handle
        [HttpPost]
        public async Task<ActionResult> Handle(string from, string body)
        {
            string smsResponse;

            try
            {
                var host = await _usersRepository.FindByPhoneNumberAsync(from);
                var reservation = await _reservationsRepository.FindFirstPendingReservationByHostAsync(host.Id);

                var smsRequest = body;
                reservation.Status =
                    smsRequest.Equals("accept", StringComparison.InvariantCultureIgnoreCase) ||
                    smsRequest.Equals("yes", StringComparison.InvariantCultureIgnoreCase)
                    ? ReservationStatus.Confirmed
                    : ReservationStatus.Rejected;

                await _reservationsRepository.UpdateAsync(reservation);
                smsResponse =
                    string.Format("You have successfully {0} the reservation", reservation.Status);
            }
            catch (Exception)
            {
                smsResponse = "Sorry, it looks like you don't have any reservations to respond to.";
            }

            return TwiML(Respond(smsResponse));
        }

        private static TwilioResponse Respond(string message)
        {
            var response = new TwilioResponse();
            response.Message(message);

            return response;
        }
    }
}