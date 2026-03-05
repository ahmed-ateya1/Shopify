using E_Shop.Application.Dtos.WishlistDtos;

namespace E_Shop.Application.ServicesContract
{
    public interface IWishlistService
    {
        Task<WishlistResponse> AddToWishlistAsync(WishlistAddRequest request);
        Task<bool> RemoveFromWishlistAsync(Guid wishlistId);
        Task<PaginatedResponse<WishlistResponse>> GetWishlistAsync(
            Expression<Func<Wishlist, bool>>? predicate = null,
            PaginationDto? pagination = null);

        Task<WishlistResponse> GetWishlistByAsync(Expression<Func<Wishlist, bool>> predicate);

    }
}
