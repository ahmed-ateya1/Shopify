using E_Shop.Application.Dtos.OrderDtos;
using E_Shop.Domain.Enums;
using E_Shop.Domain.Models;

namespace E_Shop.Application.ServicesContract
{
    public interface IOrderService
    {
        Task<PaginatedResponse<OrderResponse>> GetAllOrdersAsync(
            Expression<Func<Order, bool>>? filter = null,
            PaginationDto? pagination = null);

        Task<OrderResponse> GetOrderByIdAsync(Guid id);

        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
    }
}
