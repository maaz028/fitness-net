using core.Models.AccountModels;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace core.Models.TrainerModels
{
    public class TrainerViewModel
    {
        public string? ID { get; set; }

        [Required]
        public string? Name { get; set; }
        
        [Required]
        [EmailAddress]      
        public string? Email { get; set; }

        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Age field is required")]       
        [Range(18, 40)]
        public int? Age { get; set; }

        [Required]
        public GenderEnm? Gender { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? JoiningDate { get; set; }

        public AvailabilityStatusEnum? AvailabilityStatus { get; set; } 

        public string? PhotoPath { get; set; }

        public IFormFile? Photo { get; set; }

    }
}
