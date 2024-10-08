using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class Locality: IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Localidade")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }

        public City City { get; set; }

        public int CityId { get; set; }

      

     
    }
}
