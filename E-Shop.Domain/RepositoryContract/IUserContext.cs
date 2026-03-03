using E_Shop.Domain.Models.Identity;

namespace E_Shop.Domain.RepositoryContract
{
    public interface IUserContext
    {
        Task<ApplicationUser?> GetCurrentUserAsync();
    }
}
