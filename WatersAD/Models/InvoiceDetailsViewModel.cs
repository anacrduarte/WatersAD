using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class InvoiceDetailsViewModel
    {
        public Client Client { get; set; }

        public WaterMeter WaterMeter { get; set; }

        public Invoice Invoice { get; set; }

        public Consumption Consumption { get; set; }
    }
}
