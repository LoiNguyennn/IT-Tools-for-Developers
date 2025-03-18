using System.ComponentModel.DataAnnotations;

namespace ITTools.Core.Models
{
    public class Tool
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsPremium { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Favorite> Favorites { get; set; } = new();
    }
}