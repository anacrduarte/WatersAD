﻿using System.ComponentModel.DataAnnotations;

namespace WatersAD.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string Confirm { get; set; }
    }
}
