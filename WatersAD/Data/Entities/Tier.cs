namespace WatersAD.Data.Entities
{
    public class Tier: IEntity
    {
        public int Id { get; set; }

        public byte TierNumber { get; set; }

        public double TierPrice { get; set; }

        public string TierName { get; set; } = null!;

        public double UpperLimit {  get; set; }
    }
}
