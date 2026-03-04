using E_Shop.Domain.Enums;

namespace E_Shop.Application.Dtos.OrderDtos
{
    public record OrderStatusUpdateRequest(Guid OrderId, OrderStatus NewStatus);
}
