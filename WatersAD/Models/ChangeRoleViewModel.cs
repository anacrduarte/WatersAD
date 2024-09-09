using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WatersAD.Data.Entities;
using WatersAD.Enum;

namespace WatersAD.Models
{
    public class ChangeRoleViewModel
    {
        public int? UserId { get; set; }

        public string? UserName { get; set; }

        public UserType? CurrentRole { get; set; }

        public string SelectedRole { get; set; }

        public IEnumerable<User>? User { get; set; }


        [Display(Name = "Type User")]
        public IEnumerable<SelectListItem>? Roles { get; set; }

    }
}
