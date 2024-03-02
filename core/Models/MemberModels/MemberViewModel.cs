using core.Models.AccountModels;
using System.ComponentModel.DataAnnotations;

namespace core.Models.MemberModels
{
    public class MemberViewModel
    {
        public MemberViewModel()
        {
            Roles = new List<MemberRolesViewModel>();
        }

        [Required]
        public string? MemberID { get; set; }

        [Display(Name = "Name")]
        public string? MemberName { get; set; }

        [Display(Name = "Email")]
        public string? MemberEmail { get; set; }
        
        [Display(Name = "Gender")]
        [Required]
        public GenderEnm? MemberGender { get; set; }

        [Display(Name = "Account Status")]
        [Required]
        public AvailabilityStatusEnum? AvailabilityStatus { get; set; }

        [Display(Name = "Apply Date")]
        public DateTime? ApplyDate { get; set; }

        [Display(Name = "Last Updated Date")]
        public DateTime? LastUpdatedDate { get; set; }

        [Display(Name = "Joining Date")]
        public DateTime? JoiningDate { get; set; }

        [Display(Name ="User Image")]
        public string? PhotoPath { get; set; }

        public List<MemberRolesViewModel>? Roles { get; set; }

    }
}
