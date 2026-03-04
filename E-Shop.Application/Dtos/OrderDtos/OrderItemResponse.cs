namespace E_Shop.Application.Dtos.OrderDtos
{
    public record OrderItemResponse(
        Guid Id,
        string ProductName,
        string? ProductImageUrl,
        int Quantity,
        decimal UnitPrice,
        decimal LineTotal);
}
