using E_Shop.Application.Dtos;
using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Enums;
using E_Shop.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class OrdersController(IOrderService orderService) : Controller
    {
        public async Task<IActionResult> Index(string? status, int page = 1)
        {
            var pagination = new PaginationDto
            {
                PageIndex = page,
                PageSize = 10,
                SortBy = "OrderDate",
                SortDirection = "desc"
            };

            System.Linq.Expressions.Expression<Func<Order, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                filter = o => o.Status == orderStatus;
            }

            var orders = await orderService.GetAllOrdersAsync(filter, pagination);

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = orders.TotalPages;

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var order = await orderService.GetOrderByIdAsync(id);
                return View(order);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid orderId, OrderStatus newStatus)
        {
            try
            {
                await orderService.UpdateOrderStatusAsync(orderId, newStatus);
                TempData["SuccessMessage"] = "Order status updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}
