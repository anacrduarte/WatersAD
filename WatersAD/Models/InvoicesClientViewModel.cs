using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class InvoicesClientViewModel
    {
        public int ClientId { get; set; }

        public IEnumerable<WaterMeter> WaterMeters { get; set; }

        public IEnumerable<Consumption> Consumptions { get; set; }

        public IEnumerable<Invoice> Invoices { get; set; }

       public WaterMeter WaterMeter { get; set; }
    }
}
