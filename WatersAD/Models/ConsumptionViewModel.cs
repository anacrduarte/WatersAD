using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class ConsumptionViewModel
    {
        public int Id { get; set; }

        public int ConsumptionId { get; set; }

        public Consumption Consumption { get; set; }

        public int ClientId { get; set; }

        public IEnumerable<SelectListItem> Clients { get; set; }

        public Client Client { get; set; }

        public int WaterMeterId { get; set; }

       
        //public IEnumerable<SelectListItem> WaterMeters { get; set; }
        //public WaterMeter WaterMeter { get; set; }

        //public int TierId { get; set; }

        //public Tier Tier { get; set; }

        //public int InvoiceId { get; set; }

        //public Invoice Invoice { get; set; }

        [Required]
        [Display(Name = "Order date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ConsumptionDate { get; set; }

        [Required]
        [Display(Name = "Order date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }

        public double ConsumptionValue { get; set; }

    }
}
