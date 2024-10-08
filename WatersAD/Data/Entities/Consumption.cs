using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class Consumption : IEntity
    {
        public int Id { get; set; }

        public int TierId { get; set; }

        public Tier Tier { get; set; }

        [Required]
        [Display(Name = "Data de consumo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ConsumptionDate { get; set; }

        [Required]
        [Display(Name = "Data de registo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Valor consumo")]
        public double ConsumptionValue {  get; set; }

        public int WaterMeterId { get; set; }

        public WaterMeter WaterMeter { get; set; }


        public Invoice Invoice { get; set; }

      
    }
}
