using System.ComponentModel.DataAnnotations;

namespace WatersAD.Models
{
    public class CityViewModel
    {
        public int CountryId { get; set; }

        public string CountryName { get; set; }
        public int CityId { get; set; }

        [Required]
        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }
    }
}
