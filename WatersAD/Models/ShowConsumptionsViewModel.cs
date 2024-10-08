using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class ShowConsumptionsViewModel
    {
        public int ClientId { get; set; }
        public IEnumerable<WaterMeter> WaterMeters { get; set; }

        public IEnumerable<Consumption> Consumptions { get; set; }



    }
}
