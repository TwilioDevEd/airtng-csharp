using System;
using System.Collections.Generic;

namespace AirTNG.Web.Models
{
    public class VacationProperty
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<Reservation> Reservations { get; set; }
    }
}