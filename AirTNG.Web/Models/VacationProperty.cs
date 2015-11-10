using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirTNG.Web.Models
{
    public class VacationProperty
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser Owner { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual IList<Reservation> Reservations { get; set; }
    }
}