﻿using System.ComponentModel.DataAnnotations;

namespace WatersAD.Models
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Deve inserir um email válido.")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string UserName { get; set; }

        [Display(Name = "Palavra-passe")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "O campo palavra-passe é obrigatório.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "O campo {0} deve ter entre {2} e {1} carateres.")]
        public string Password { get; set; }

        [Display(Name = "Confirmar palavra-passe")]
        [Compare("Password", ErrorMessage = "A nova palavra-passe e a confirmação não são iguais.")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "O campo confirmar palavra-passe é obrigatório.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "O campo {0} deve ter entre {2} e {1} carateres..")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Token { get; set; }
    }
}
