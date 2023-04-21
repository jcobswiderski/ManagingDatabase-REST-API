using System.ComponentModel.DataAnnotations;

namespace Exercise3.Models.DTOs
{
    public class AnimalUpdate
    {
        [Required]
        [MinLength(5)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = null;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Area { get; set; } = string.Empty;
    }
}
