using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WatersAD.Data.Entities;

namespace WatersAD.Helpers
{
    public class BasicInformationHelper
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Address { get; set; }


        public string Email { get; set; }

        public User User { get; set; }

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "The NIF must be exactly 9 digits.")]
        public string NIF { get; set; }

        [Required]
        [Display(Name = "Nº telefone")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Nº")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string HouseNumber { get; set; }

        [Display(Name = "Codigo-Postal")]
        [Required]
        [MaxLength(4, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string PostalCode { get; set; }

        [Display(Name = "Codigo-Postal")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string RemainPostalCode { get; set; }

        [Display(Name = "Locality")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Locality.")]
        public int LocalityId { get; set; }

        public IEnumerable<SelectListItem> Localities { get; set; }


        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city.")]
        public int CityId { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country.")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public Locality Locality { get; set; }

        public Country Country { get; set; }

        public City City { get; set; }
    }
}
