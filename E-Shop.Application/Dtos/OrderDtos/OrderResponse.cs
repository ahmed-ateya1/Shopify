using E_Shop.Domain.Enums;

namespace E_Shop.Application.Dtos.OrderDtos
{
    public record OrderResponse(
        Guid Id,
        OrderStatus Status,
        DateTime OrderDate,
        decimal TotalAmount,
        Guid UserId,
        string UserName,
        string UserEmail,
        string ShippingAddress,
        IEnumerable<OrderItemResponse> Items);
}
