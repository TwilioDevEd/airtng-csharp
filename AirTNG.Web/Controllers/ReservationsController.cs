using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AirTNG.Web.Domain.NewPhoneNumber;
using AirTNG.Web.Domain.Reservations;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using AirTNG.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Twilio.TwiML;

namespace AirTNG.Web.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
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

        public async Task<ActionResult> Index()
        {
            var user = await _usersRepository.FindAsync(UserId());
            var reservations = user.Reservations;

            return View(reservations);
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
                UserName = vacationProperty.Owner.Name,
                UserPhoneNumber = vacationProperty.Owner.PhoneNumber,
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
            string smsResponseBody;

            try
            {
                var host = await _usersRepository.FindByPhoneNumberAsync(from);
                var reservation = await _reservationsRepository.FindFirstPendingReservationByHostAsync(host.Id);

                var smsRequest = body;
                if (IsSmsRequestAccepted(smsRequest))
                {
                    var purchasedPhoneNumber = _phoneNumberPurchaser.Purchase(host.AreaCode);

                    reservation.Status = ReservationStatus.Confirmed;
                    reservation.AnonymousPhoneNumber = purchasedPhoneNumber.ToString();
                }
                else
                {
                    reservation.Status = ReservationStatus.Rejected;
                }

                await _reservationsRepository.UpdateAsync(reservation);
                smsResponseBody =
                    string.Format("You have successfully {0} the reservation", reservation.Status);
            }
            catch (Exception)
            {
                smsResponseBody = "Sorry, it looks like you don't have any reservations to respond to.";
            }

            return Content(Respond(smsResponseBody, from), "text/xml");
        }

        private static string Respond(string message, string from)
        {
            var response = new MessagingResponse();
            response.Message(message, to: from);

            return response.ToString();
        }

        private static bool IsSmsRequestAccepted(string smsRequest)
        {
            return smsRequest.Equals("accept", StringComparison.InvariantCultureIgnoreCase) ||
                   smsRequest.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}