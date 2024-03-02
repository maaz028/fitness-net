using core.Models.AccountModels;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace core.Models.AdminAccountModels
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
            PhotoPath = string.Empty;
        }
        [Required]
        public string? ID { get; set; }

        [Required]
        public string? Name { get; set; }
        
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        
        [Required]
        public GenderEnm? Gender { get; set; }

        [Required]
        [Display(Name = "Account Status")]
        public AvailabilityStatusEnum? AccountStatus { get; set; }


        [Display(Name = "Apply Date")]
        public DateTime? ApplyDate { get; set; }

 
        [Display(Name = "Last Updated Date")]
        public DateTime? LastUpdatedDate { get; set; }

 
        [Display(Name = "Joining Date")]
        public DateTime? JoiningDate { get; set; }

        public IFormFile? Photo { get; set; }
        public string? PhotoPath { get; set; }
            
    }
}
