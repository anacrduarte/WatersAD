using System.ComponentModel.DataAnnotations;

namespace WatersAD.Models
{
    public class RecoverPasswordViewModel
    {

        [Display(Name = "Email")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress(ErrorMessage = "Deve inserir um email válido.")]
        public string? Email { get; set; }
    }
}
