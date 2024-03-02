using core.Models;
using core.Models.AccountModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Models.TrainerModels
{
    public class TrainerModel
    {
        [Key]
        public string? TrainerID { get; set; }
     
        [Required]
        public string? Name { get; set; }

        [Required]
        public GenderEnm? Gender { get; set; } 
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public DateTime? CreatedDate { get; set; }
        
        [Required]
        [Range(18,40)]
        public int Age { get; set; }

        public DateTime? JoiningDate { get; set; }

        public AvailabilityStatusEnum? AvailabilityStatus { get; set; } = AvailabilityStatusEnum.InActive;

        public string? PhotoPath { get; set; }

      


    }
}
