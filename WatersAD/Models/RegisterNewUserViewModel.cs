using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WatersAD.Enum;

namespace WatersAD.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }


        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string? Address { get; set; }

        [Display(Name = "Type User")]
        public IEnumerable<SelectListItem>? Roles { get; set; }

        [Required]
        public string SelectedRole { get; set; }
    }


}
