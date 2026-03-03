using E_Shop.Domain.Models.Identity;
using E_Shop.Domain.RepositoryContract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace E_Shop.Infrastructure.Repositories
{
    public class UserContext(
       IUnitOfWork unitOfWork,
       IHttpContextAccessor httpContextAccessor,
       ILogger<UserContext> logger
       )
       : IUserContext
    {
        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var userId = httpContextAccessor.HttpContext.User
                        .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("No user is authenticated.");
                return null;
            }

            var user = await unitOfWork.Repository<ApplicationUser>()
                .GetByAsync(x => x.Id.ToString() == userId);

            return user;

        }
    }
}
