using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace core.Models.EquipmentModels
{
    public class EquipmentViewModel
    {
        public string? ID { get; set; }
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastUpdatedDate { get; set; } = DateTime.Now;

        public IFormFile? Photo { get; set; }

        public string? PhotoPath { get; set; }
    }
}
