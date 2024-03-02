using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace core.Models.AccountModels.ViewModels
{
    public class RoleViewModel
    {
        public string? ID { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }

        public IEnumerable<IdentityRole>? Roles { get; set; }
    }
}
