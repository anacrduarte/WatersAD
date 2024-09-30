namespace WatersAD.Data.Entities
{
    public class Notification : IEntity
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public RequestWaterMeter RequestWaterMeter { get; set; }
    }
}
