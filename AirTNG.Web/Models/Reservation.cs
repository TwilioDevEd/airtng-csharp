using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirTNG.Web.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public ReservationStatus Status { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int VactionPropertyId { get; set; }
        [ForeignKey("VactionPropertyId")]
        public virtual VacationProperty VacationProperty { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser Guest { get; set; }
        public string AnonymousPhoneNumber { get; set; }
    }
}