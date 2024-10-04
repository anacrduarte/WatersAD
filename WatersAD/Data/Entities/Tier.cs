using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class Tier: IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Nº")]
        public byte TierNumber { get; set; }
        [Display(Name = "Preço")]
        public double TierPrice { get; set; }
        [Display(Name = "Nome")]
        public string TierName { get; set; } = null!;
        [Display(Name = "Limite ")]
        public double UpperLimit {  get; set; }
    }
}
