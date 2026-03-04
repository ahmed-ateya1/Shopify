using E_Shop.Application.Dtos;
using E_Shop.Application.ServicesContract;
using E_Shop.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace E_Shop.UI.Controllers
{
    public class HomeController(IProductService productService,
        ICategoryService categoryService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var featuredProducts = await productService.GetAllProductsAsync(
                p => p.IsActive,
                new PaginationDto { PageIndex = 1, PageSize = 8, SortBy = "CreatedAt", SortDirection = "desc" });

            var categories = await categoryService.GetAllCategoriesByAsync(
                pagination: new PaginationDto { PageIndex = 1, PageSize = 8, SortBy = "Name", SortDirection = "asc" });

            ViewBag.FeaturedProducts = featuredProducts.Items;
            ViewBag.Categories = categories.Items;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
