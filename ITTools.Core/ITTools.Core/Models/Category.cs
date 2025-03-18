using System.ComponentModel.DataAnnotations;

namespace ITTools.Core.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<Tool> Tools { get; set; } = new();
    }
}