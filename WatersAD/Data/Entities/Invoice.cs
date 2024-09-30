namespace WatersAD.Data.Entities
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }

        public DateTime InvoiceDate { get; set; }

        public int ClientId { get; set; }

        public Client? Client { get; set; }

        ICollection<Consumption>? Consumptions { get; set; }

        public bool Issued { get; set; } = false;

        public bool Sent { get; set; } = false;

        public decimal TotalAmount { get; set; }

        public DateTime LimitDate { get; set; }
    }
}
