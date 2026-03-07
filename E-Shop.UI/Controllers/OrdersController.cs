using E_Shop.Application.Dtos;
using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Models;
using E_Shop.Domain.RepositoryContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.UI.Controllers
{
    [Authorize]
    public class OrdersController(IOrderService orderService,
        IUserContext userContext) : Controller
    {
        public async Task<IActionResult> Index(string? status, int page = 1)
        {
            var user = await userContext.GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var pagination = new PaginationDto
            {
                PageIndex = page,
                PageSize = 10,
                SortBy = "OrderDate",
                SortDirection = "desc"
            };

            System.Linq.Expressions.Expression<Func<Order, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Enums.OrderStatus>(status, out var orderStatus))
            {
                filter = o => o.UserId == user.Id && o.Status == orderStatus;
            }
            else
            {
                filter = o => o.UserId == user.Id;
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
            var user = await userContext.GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            try
            {
                var order = await orderService.GetOrderByIdAsync(id);

                // Ensure user can only see their own orders
                if (order.UserId != user.Id)
                    return Forbid();

                return View(order);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
