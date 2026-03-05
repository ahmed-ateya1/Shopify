namespace E_Shop.Application.Dtos.WishlistDtos
{
    public record WishlistResponse(
        Guid Id,
        DateTime AddedAt,
        Guid UserId,
        Guid ProductId,
        string ProductName,
        decimal ProductPrice,
        IEnumerable<string> ProductImageUrls
    );
}
