namespace E_Shop.Application.Dtos.WishlistDtos
{
    public record WishlistAddRequest(
        Guid UserId,
        Guid ProductId
    );
}
