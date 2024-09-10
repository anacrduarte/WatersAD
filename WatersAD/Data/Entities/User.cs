using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WatersAD.Enum;

namespace WatersAD.Data.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string? LastName { get; set; }

        
        [Display(Name = "Type User")]
        public UserType UserType { get; set; }


        [Required]
        [MaxLength(200, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string? Address { get; set; }


        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        public string? ImageFullPath
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
