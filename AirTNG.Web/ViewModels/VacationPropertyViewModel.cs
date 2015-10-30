using System.ComponentModel.DataAnnotations;

namespace AirTNG.Web.ViewModels
{
    public class VacationPropertyViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required, Url]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }
    }
}