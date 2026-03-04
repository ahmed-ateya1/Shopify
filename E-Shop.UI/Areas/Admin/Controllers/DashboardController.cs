using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class DashboardController(IProductService productService,
        ICategoryService categoryService,
        IOrderService orderService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await productService.GetAllProductsAsync();
            var categories = await categoryService.GetAllCategoriesByAsync();
            var orders = await orderService.GetAllOrdersAsync();

            ViewBag.ProductCount = products.TotalCount;
            ViewBag.CategoryCount = categories.TotalCount;
            ViewBag.OrderCount = orders.TotalCount;
            ViewBag.Revenue = orders.Items.Sum(o => o.TotalAmount);
            ViewBag.RecentOrders = orders.Items.Take(5);

            return View();
        }
    }
}
