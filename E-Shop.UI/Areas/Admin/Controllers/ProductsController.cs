using E_Shop.Application.Dtos;
using E_Shop.Application.Dtos.ProductDtos;
using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class ProductsController(IProductService productService,
        ICategoryService categoryService) : Controller
    {
        public async Task<IActionResult> Index(string? q, int page = 1)
        {
            var pagination = new PaginationDto
            {
                PageIndex = page,
                PageSize = 10,
                SortBy = "CreatedAt",
                SortDirection = "desc"
            };

            System.Linq.Expressions.Expression<Func<Product, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(q))
            {
                filter = p => p.Name.Contains(q) || p.SKU.Contains(q);
            }

            var products = await productService.GetAllProductsAsync(filter, pagination);

            ViewBag.SearchQuery = q;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = products.TotalPages;

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductAddRequest request)
        {
            try
            {
                await productService.AddProductAsync(request);
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadCategories();
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var product = await productService.GetProductByAsync(p => p.Id == id);
                await LoadCategories();

                var updateRequest = new ProductUpdateRequest(
                    product.Id, product.Name, product.SKU,
                    product.Price, product.StockQuantity,
                    product.CategoryId, null, product.IsActive);

                ViewBag.CurrentImages = product.ImageUrls;
                return View(updateRequest);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductUpdateRequest request)
        {
            try
            {
                await productService.UpdateProductAsync(request);
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadCategories();
                return View(request);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await productService.DeleteProductAsync(id);
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories()
        {
            var categories = await categoryService.GetAllCategoriesByAsync();
            ViewBag.Categories = categories.Items;
        }
    }
}
