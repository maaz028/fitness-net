using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace core.Models.AccountModels.ViewModels
{
    public class RegisterViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage ="Name field is required")]
        [NotNull]
        public string? Name { get; set; }

        [Required]
        [NotNull]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public GenderEnm? Gender { get; set; }

        [Required]
        [NotNull]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [NotNull]
        [Compare("Password", ErrorMessage = "Password does not match")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string? PasswordConfirmation { get; set; }
    }
}
