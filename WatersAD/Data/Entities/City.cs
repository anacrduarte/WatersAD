﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WatersAD.Data.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }

        public Country Country { get; set; }

        public int CountryId { get; set; }

        [JsonIgnore]
        public ICollection<Locality> Localities { get; set; }
    }
}
