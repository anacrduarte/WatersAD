using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class WaterMeterService : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Nº de série")]
        public string SerialNumber { get; set; }

        public int Quantity { get; set; }

        public bool Available { get; set; } = true;
    }
}
