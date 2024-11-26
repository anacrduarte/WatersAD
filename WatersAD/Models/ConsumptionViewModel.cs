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

        public Client Client { get; set; }

        public int WaterMeterId { get; set; }


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
