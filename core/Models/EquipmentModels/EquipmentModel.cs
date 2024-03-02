using System.ComponentModel.DataAnnotations;

namespace core.Models.EquipmentModels
{
    public class EquipmentModel
    {
        [Key]
        public string? ID { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdatedDate { get; set; }

        public string? PhotoPath { get; set; }

       
    }
}
