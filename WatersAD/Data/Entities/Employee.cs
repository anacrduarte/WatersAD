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
        [Display(Name = "Nº")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string HouseNumber { get; set; }

        [Required]
        public string Email { get; set; }

        public User User { get; set; }


        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "The NIF must be exactly 9 digits.")]
        public string NIF { get; set; }

        [Required]
        [Display(Name = "Nº telefone")]
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [Display(Name = "Código-Postal")]
        [Required]
        [MaxLength(4, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string PostalCode { get; set; }

        [Display(Name = "Código-Postal")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string RemainPostalCode { get; set; }

        public int LocalityId { get; set; }

        public Locality Locality { get; set; }

        [Display(Name = "Nome")]
        public string FullName
        {
            get
            {

                return $"{FirstName} {LastName}";
            }
        }
        [Display(Name = "Código-Postal")]
        public string FullPostalCode
        {
            get
            {
                return $"{PostalCode}-{RemainPostalCode}";
            }
        }
        [Display(Name = "Morada")]
        public string FullAdress
        {
            get
            {
                return $"{Address}, nº {HouseNumber}";
            }
        }

        public string OldEmail { get; set; }
    }
}
