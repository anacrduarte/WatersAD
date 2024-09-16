using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class WaterMeter : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Rua")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string? Address { get; set; }

        [Required]
        [Display(Name = "Nº")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string? HouseNumber { get; set; }
        [Display(Name = "Codigo-Postal")]
        [Required]
        [MaxLength(4, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string? PostalCode { get; set; }

        [Display(Name = "Codigo-Postal")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string? RemainPostalCode { get; set; }

        public int LocalityId { get; set; }

        public Locality? Locality { get; set; }

        public bool IsActive { get; set; } = true;

       
        [Display(Name = "Data de Instalação")]
        public DateTime? InstallationDate { get; set; }

        public int ClientId { get; set; }

        public Client? Client { get; set; }

        public int WaterMeterServiceId { get; set; }

        public WaterMeterService? WaterMeterService { get; set; }
    }
}
