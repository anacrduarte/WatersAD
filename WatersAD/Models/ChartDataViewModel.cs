using static WatersAD.Controllers.DashboardController;
using WatersAD.Data.Entities;
using Org.BouncyCastle.Asn1.Pkcs;
using WatersAD.Helpers;

namespace WatersAD.Models
{
    public class ChartDataViewModel
    {
       
        public IEnumerable<Notification> UnreadNotifications { get; set; }


       
        public List<ChartDataHelper> ChartData { get; set; }

        public List<LineDataHelper> LineData { get; set; }

        public List<PieDataHelper> PieData { get; set; }

        
        public double[] CellSpacing { get; set; }

        public IEnumerable<Client> Clients { get; set; }

        public IEnumerable<WaterMeter> WaterMeters { get; set; }

        public IEnumerable<Invoice> Invoices { get; set; }

        public IEnumerable<Consumption> Consumptions { get; set; }

        public IEnumerable<InvoiceYearHelper> TotalAmount { get; set; }
    }
}
