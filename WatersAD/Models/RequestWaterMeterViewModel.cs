using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class RequestWaterMeterViewModel
    {
        public int ClientId { get; set; }

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

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "The NIF must be exactly 9 digits.")]
        public string NIF { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
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


        [Required]
        [Display(Name = "Rua")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string AddressWaterMeter { get; set; }

        [Required]
        [Display(Name = "Nº")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string HouseNumberWaterMeter { get; set; }

        [Display(Name = "Codigo-Postal")]
        [Required]
        [MaxLength(4, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string PostalCodeWaterMeter { get; set; }

        [Display(Name = "Codigo-Postal")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string RemainPostalCodeWaterMeter { get; set; }

        [Display(Name = "Locality")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Locality.")]
        public int LocalityId { get; set; }

        public IEnumerable<SelectListItem> Localities { get; set; }
        [Display(Name = "Locality")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Locality.")]
        public int LocalityWaterMeterId { get; set; }

        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city.")]
        public int CityId { get; set; }

        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city.")]
        public int CityWaterMeterId { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }


        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country.")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country.")]
        public int CountryWaterMeterId { get; set; }

        [Display(Name = "Nome")]
        public string FullName
        {
            get
            {

                return $"{FirstName} {LastName}";
            }
        }
        [Display(Name = "Código Postal")]
        public string FullPostalCode
        {
            get
            {
                return $"{PostalCode}-{RemainPostalCode}";
            }
        }

        [Display(Name = "Morada")]
        public string FullAdress
        {
            get
            {
                return $"{Address}, nº {HouseNumber}";
            }
        }

        public Locality Locality { get; set; }

        public City City { get; set; }

     
        [Display(Name = "Código Postal")]
        public string FullPostalCodeWM
        {
            get
            {
                return $"{PostalCode}-{RemainPostalCode}";
            }
        }

        [Display(Name = "Morada")]
        public string FullAdressWM
        {
            get
            {
                return $"{Address}, nº {HouseNumber}";
            }
        }

        public Locality LocalityWM { get; set; }

        public City CityWM { get; set; }
    }
}
