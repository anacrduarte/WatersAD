using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class Consumption : IEntity
    {
        public int Id { get; set; }

      
        public Tier? Tier { get; set; }

        [Required]
        [Display(Name = "Order date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime ConsumptionDate { get; set; }

        [Required]
        [Display(Name = "Order date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime RegistrationDate { get; set; }

        public double ConsumptionValue {  get; set; }

        public WaterMeter? WaterMeter { get; set; }

        public Invoice? Invoice { get; set; }
    }
}
