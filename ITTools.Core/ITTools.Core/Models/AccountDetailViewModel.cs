using ITTools.Core.Models;
namespace ITTools.Core.Models
{
    public class AccountDetailViewModel
    {
        public required ApplicationUser User { get; init; }
        public required List<Tool> FavoriteTools { get; init; }
        public bool IsPremium { get; init; }
    }

}
