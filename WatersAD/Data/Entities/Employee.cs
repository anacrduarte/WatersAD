﻿using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class Employee : IEntity
    {
        public int Id { get; set; }

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
        [Display(Name = "Nº")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string? HouseNumber { get; set; }

        [Required]
        public string Email { get; set; }

        public User? User { get; set; }


        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "The NIF must be exactly 9 digits.")]
        public int NIF { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [Display(Name = "Codigo-Postal")]
        [Required]
        [MaxLength(4, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string? PostalCode { get; set; }

        [Display(Name = "Codigo-Postal")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string? RemainPostalCode { get; set; }

        [Display(Name = "Name")]
        public string? FullName
        {
            get
            {

                return $"{FirstName} {LastName}";
            }
        }

        public string FullPostalCode
        {
            get
            {
                return $"{PostalCode}-{RemainPostalCode}";
            }
        }
    }
}
