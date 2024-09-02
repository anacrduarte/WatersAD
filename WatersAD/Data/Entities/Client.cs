using System.ComponentModel.DataAnnotations;

namespace WatersAD.Data.Entities
{
    public class Client : IEntity
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


        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public User User { get; set; }

        [Required]
        public int NIF { get; set; }

        [Required]
        public int PhoneNumber { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }

                return $"https://localhost:44334{ImageUrl.Substring(1)}";
            }
        }
    }
}
