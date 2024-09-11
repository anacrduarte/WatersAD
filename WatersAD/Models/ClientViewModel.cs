﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class ClientViewModel 
    {

        public int ClientId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Address { get; set; }

        [Required]
        public string Email { get; set; }

        public User? User { get; set; }


        [Required]
        public int NIF { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Locality")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Locality.")]
        public int LocalityId { get; set; }
        public IEnumerable<SelectListItem>? Localities { get; set; }


        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city.")]
        public int CityId { get; set; }

        public IEnumerable<SelectListItem>? Cities { get; set; }


        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country.")]
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem>? Countries { get; set; }
    }
}