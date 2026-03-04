namespace E_Shop.Application.Dtos.ProductDtos
{
    public record ProductResponse(
        Guid Id,
        string Name,
        string SKU,
        decimal Price,
        int StockQuantity,
        bool IsActive,
        DateTime CreatedAt,
        Guid CategoryId,
        IEnumerable<string> ImageUrls
        );
}
