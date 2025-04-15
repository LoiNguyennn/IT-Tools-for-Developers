using Microsoft.AspNetCore.Identity;

namespace ITTools.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsPremium { get; set; } // Custom field for premium status
        public List<Favorite> Favorites { get; set; } = new(); // Navigation property
    }
}
