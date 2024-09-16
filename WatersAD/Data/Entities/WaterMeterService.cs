namespace WatersAD.Data.Entities
{
    public class WaterMeterService : IEntity
    {
        public int Id { get; set; }

        public string? SerialNumber { get; set; }

        public int Quantity { get; set; }

        public bool Available { get; set; } = true;
    }
}
