using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class Locality: IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Codigo-Postal")]
        [Required]
        [MaxLength(4, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string PostalCode { get; set; }

        [Display(Name = "Codigo-Postal")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string? RemainPostalCode { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }

        public City City { get; set; }

        public int CityId { get; set; }

        public string FullPostalCode
        {
            get
            {
                return $"{PostalCode}-{RemainPostalCode}";
            }
        }
    }
}
