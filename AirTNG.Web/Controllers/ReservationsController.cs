using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AirTNG.Web.Domain.PhoneNumber;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using AirTNG.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace AirTNG.Web.Controllers
{
    [Authorize]
    public class ReservationsController : TwilioController
    {
        private readonly IVacationPropertiesRepository _vacationPropertiesRepository;
        private readonly IReservationsRepository _reservationsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly INotifier _notifier;
        private readonly IPurchaser _phoneNumberPurchaser;

        public Func<string> UserId;

        public ReservationsController() : this(
            new VacationPropertiesRepository(),
            new ReservationsRepository(),
            new UsersRepository(),
            new Notifier(),
            new Purchaser()) { }

        public ReservationsController(
            IVacationPropertiesRepository vacationPropertiesRepository,
            IReservationsRepository reservationsRepository,
            IUsersRepository usersRepository,
            INotifier notifier,
            IPurchaser phoneNumberPurchaser)
        {
            _vacationPropertiesRepository = vacationPropertiesRepository;
            _reservationsRepository = reservationsRepository;
            _usersRepository = usersRepository;
            _notifier = notifier;
            _phoneNumberPurchaser = phoneNumberPurchaser;
            UserId = () => User.Identity.GetUserId();
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
                    UserId = UserId(), // This is the reservee user ID
                    VactionPropertyId = model.VacationPropertyId,
                    Status = ReservationStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                await _reservationsRepository.CreateAsync(reservation);
                await _reservationsRepository.LoadNavigationPropertiesAsync(reservation);

                await _notifier.SendNotificationAsync(reservation);

                return RedirectToAction("Index", "VacationProperties");
            }

            return View(model);
        }

        // POST Reservations/Handle
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Handle(string from, string body)
        {
            string smsResponse;

            try
            {
                var host = await _usersRepository.FindByPhoneNumberAsync(from);
                var reservation = await _reservationsRepository.FindFirstPendingReservationByHostAsync(host.Id);

                var smsRequest = body;
                if (IsSmsRequestAccepted(smsRequest))
                {
                    // TODO: Update user model to handle the area code.
                    var purchasedPhoneNumber = _phoneNumberPurchaser.Purchase("area-code");

                    reservation.Status = ReservationStatus.Confirmed;
                    reservation.AnonymousPhoneNumber = purchasedPhoneNumber.PhoneNumber;
                }
                else
                {
                    reservation.Status = ReservationStatus.Rejected;
                }

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

        private static bool IsSmsRequestAccepted(string smsRequest)
        {
            return smsRequest.Equals("accept", StringComparison.InvariantCultureIgnoreCase) ||
                   smsRequest.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}