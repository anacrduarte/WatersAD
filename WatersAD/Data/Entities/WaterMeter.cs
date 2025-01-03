﻿using Syncfusion.EJ2.TreeGrid;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WatersAD.Data.Entities
{
    public class WaterMeter : IEntity
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
        [Display(Name = "Codigo-Postal")]
        [Required]
        [MaxLength(4, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string PostalCode { get; set; }

        [Display(Name = "Codigo-Postal")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string RemainPostalCode { get; set; }

        public int LocalityId { get; set; }

        public Locality Locality { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;

       
        [Display(Name = "Data de Instalação")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime InstallationDate { get; set; }

        public int ClientId { get; set; }

        
        public Client Client { get; set; }

        public int WaterMeterServiceId { get; set; }

      
        public WaterMeterService WaterMeterService { get; set; }

        [JsonIgnore]
        public ICollection<Consumption> Consumptions { get; set; }

  
        [Display(Name = "Código postal")]
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
                return $"{Address}, {HouseNumber}";
            }
        }
    }
}
