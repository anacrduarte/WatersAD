using System.ComponentModel.DataAnnotations;

namespace WatersAD.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string ImageUrl { get; set; }

        public IFormFile ImageFile { get; set; }



    }
}
