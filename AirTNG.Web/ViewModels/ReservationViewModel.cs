using System.ComponentModel.DataAnnotations;

namespace AirTNG.Web.ViewModels
{
    public class ReservationViewModel
    {
        public int VacationPropertyId { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Would you like to say anything to the host?")]
        public string Message { get; set; }

        public string UserName { get; set; }
        public string UserPhoneNumber { get; set; }
    }
}