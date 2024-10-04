using System.ComponentModel.DataAnnotations;
using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class ChangeUserEmailViewModel
    {
        public string FullName { get; set; }
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }

        [Display(Name = "Novo email")]
        public string Email { get; set; }

        [Display(Name = "Email actual")]
        public string OldEmail { get; set; }


    }
}
