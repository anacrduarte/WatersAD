using System.ComponentModel.DataAnnotations;

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
        public string Email { get; set; }

        public User? User { get; set; }


        [Required]
        public int NIF { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Name")]
        public string? FullName
        {
            get
            {

                return $"{FirstName} {LastName}";
            }
        }
    }
}
