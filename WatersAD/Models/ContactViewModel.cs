using System.ComponentModel.DataAnnotations;

namespace WatersAD.Models
{
    public class ContactViewModel
    {
        public string FullName { get; set; }


        [EmailAddress(ErrorMessage = "Por favor, insira um endereço de email válido.")]
        public string Email { get; set; }

        public string Message { get; set; }
    }
}
