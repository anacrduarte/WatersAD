using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class Country : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }

        public ICollection<City>? Cities { get; set; }

        [Display(Name = "Número de cidades")]
        public int NumberCities => Cities == null ? 0 : Cities.Count();
    }
}
