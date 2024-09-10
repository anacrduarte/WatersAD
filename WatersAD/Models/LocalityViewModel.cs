using System.ComponentModel.DataAnnotations;

namespace WatersAD.Models
{
    public class LocalityViewModel
    {
        public int CityId { get; set; }

        public int LocalityId { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Codigo-Postal")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Codigo-Postal")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string? RemainPostalCode { get; set; }
    }
}
