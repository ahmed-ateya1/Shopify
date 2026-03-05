namespace E_Shop.Application.ServicesContract
{
    public interface IShoppingCartService
    {
        Task<Cart> GetCartAsync(string userId, CancellationToken cancellationToken = default);
        Task AddToCartAsync(string userId, CartItems item, CancellationToken cancellationToken = default);
        Task RemoveFromCartAsync(string userId, Guid productId, CancellationToken cancellationToken = default);
        Task ClearCartAsync(string userId, CancellationToken cancellationToken = default);
    }
}
