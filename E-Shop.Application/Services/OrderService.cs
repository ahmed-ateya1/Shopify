using E_Shop.Application.Dtos;
using E_Shop.Application.Dtos.OrderDtos;
using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Enums;
using E_Shop.Domain.Models;
using E_Shop.Domain.RepositoryContract;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace E_Shop.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork,
        ILogger<OrderService> logger) : IOrderService
    {
        public async Task<PaginatedResponse<OrderResponse>> GetAllOrdersAsync(
            Expression<Func<Order, bool>>? filter = null,
            PaginationDto? pagination = null)
        {
            pagination ??= new PaginationDto { PageIndex = 1, PageSize = 10, SortBy = "OrderDate", SortDirection = "desc" };

            var orders = await unitOfWork.Repository<Order>()
                .GetAllAsync(filter,
                    includeProperties: "User,ShippingAddress,OrderItems,OrderItems.Product,OrderItems.Product.Images",
                    sortBy: pagination.SortBy,
                    sortDirection: pagination.SortDirection,
                    pageIndex: pagination.PageIndex,
                    pageSize: pagination.PageSize);

            var totalCount = await unitOfWork.Repository<Order>().CountAsync(filter);

            if (orders == null || !orders.Any())
                return new PaginatedResponse<OrderResponse>();

            var orderResponses = orders.Select(o => new OrderResponse(
                o.Id,
                o.Status,
                o.OrderDate,
                o.TotalAmount,
                o.UserId,
                o.User?.FullName ?? "N/A",
                o.User?.Email ?? "N/A",
                o.ShippingAddress != null
                    ? $"{o.ShippingAddress.Street}, {o.ShippingAddress.City}, {o.ShippingAddress.Country} {o.ShippingAddress.ZipCode}"
                    : "N/A",
                o.OrderItems.Select(oi => new OrderItemResponse(
                    oi.Id,
                    oi.Product?.Name ?? "Unknown",
                    oi.Product?.Images?.FirstOrDefault()?.ImageUrl,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.LineTotal))
            ));

            return new PaginatedResponse<OrderResponse>
            {
                Items = orderResponses,
                TotalCount = totalCount,
                PageIndex = pagination.PageIndex,
                PageSize = pagination.PageSize
            };
        }

        public async Task<OrderResponse> GetOrderByIdAsync(Guid id)
        {
            var order = await unitOfWork.Repository<Order>()
                .GetByAsync(o => o.Id == id,
                    includeProperties: "User,ShippingAddress,OrderItems,OrderItems.Product,OrderItems.Product.Images");

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            return new OrderResponse(
                order.Id,
                order.Status,
                order.OrderDate,
                order.TotalAmount,
                order.UserId,
                order.User?.FullName ?? "N/A",
                order.User?.Email ?? "N/A",
                order.ShippingAddress != null
                    ? $"{order.ShippingAddress.Street}, {order.ShippingAddress.City}, {order.ShippingAddress.Country} {order.ShippingAddress.ZipCode}"
                    : "N/A",
                order.OrderItems.Select(oi => new OrderItemResponse(
                    oi.Id,
                    oi.Product?.Name ?? "Unknown",
                    oi.Product?.Images?.FirstOrDefault()?.ImageUrl,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.LineTotal))
            );
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await unitOfWork.Repository<Order>()
                .GetByAsync(o => o.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                order.Status = newStatus;
                await unitOfWork.Repository<Order>().UpdateAsync(order);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Failed to update order status.");
                throw;
            }
        }

        public async Task<Guid> CreateOrderAsync(Guid userId, Guid shippingAddressId, List<CartItems> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Validate stock for all items
                foreach (var item in cartItems)
                {
                    var product = await unitOfWork.Repository<Product>()
                        .GetByAsync(p => p.Id == item.ProductID);

                    if (product == null)
                        throw new InvalidOperationException($"Product '{item.ProductName}' no longer exists.");

                    if (product.StockQuantity < item.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}, Requested: {item.Quantity}.");
                }

                // 2. Create Order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ShippingAddressId = shippingAddressId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = cartItems.Sum(i => i.Price * i.Quantity)
                };

                await unitOfWork.Repository<Order>().CreateAsync(order);

                // 3. Create OrderItems + 4. Decrease stock
                foreach (var item in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    };

                    await unitOfWork.Repository<OrderItem>().CreateAsync(orderItem);

                    var product = await unitOfWork.Repository<Product>()
                        .GetByAsync(p => p.Id == item.ProductID);

                    product.StockQuantity -= item.Quantity;
                    await unitOfWork.Repository<Product>().UpdateAsync(product);
                }

                await transaction.CommitAsync();
                return order.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Checkout transaction failed.");
                throw;
            }
        }
    }
}
