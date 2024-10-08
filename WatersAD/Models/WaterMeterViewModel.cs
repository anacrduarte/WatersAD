using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class WaterMeterViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Rua")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Nº")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string HouseNumber { get; set; }

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

        [Display(Name = "Data de Instalação")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime InstallationDate { get; set; }

        public Client Client { get; set; }

        public int ClientId { get; set; }

        public Locality Locality { get; set; }

        public Country Country { get; set; }

        public City City { get; set; }

        public WaterMeterService WaterMeterService { get; set; }

        public int WaterMeterServicesId { get; set; }
        public IEnumerable<SelectListItem> WaterMeterServices { get; set; }

        [Display(Name = "Codigo-Postal")]
        [MaxLength(4, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string PostalCode { get; set; }

        [Display(Name = "Codigo-Postal")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string RemainPostalCode { get; set; }

    }
}
