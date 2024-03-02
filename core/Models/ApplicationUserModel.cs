using core.Models;
using core.Models.AccountModels;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace core.Models
{
    public class ApplicationUserModel : IdentityUser
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public GenderEnm? Gender { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastUpdatedDate { get; set;}

        public DateTime? JoiningDate { get; set; }

        public string? AssignedTrainer { get; set; }

        [DefaultValue(0)]
        public AvailabilityStatusEnum? AvailabilityStatus { get; set; }

        public string? PhotoPath { get; set; }




    }
}
