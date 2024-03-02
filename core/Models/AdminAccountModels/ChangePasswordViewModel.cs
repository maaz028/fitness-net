using System.ComponentModel.DataAnnotations;

namespace core.Models.AdminAccountModels
{
    public class ChangePasswordViewModel
    {
        [Required,Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Required, Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required, Display(Name = "Confirm Password")]
        [Compare("NewPassword",ErrorMessage ="Confirm Password Does not match")]
        [DataType(DataType.Password)]
        public string? ConfirmNewPassword { get; set; }
    }
}
