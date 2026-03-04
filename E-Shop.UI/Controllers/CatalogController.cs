using E_Shop.Application.Dtos;
using E_Shop.Application.Dtos.ProductDtos;
using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace E_Shop.UI.Controllers
{
    public class CatalogController(IProductService productService,
        ICategoryService categoryService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(Guid? categoryId, string? q, string? sort, int page = 1)
        {
            const int pageSize = 12;

            Expression<Func<Product, bool>>? filter = null;

            if (categoryId.HasValue && !string.IsNullOrWhiteSpace(q))
            {
                filter = p => p.CategoryId == categoryId.Value
                    && p.IsActive
                    && p.Name.Contains(q);
            }
            else if (categoryId.HasValue)
            {
                filter = p => p.CategoryId == categoryId.Value && p.IsActive;
            }
            else if (!string.IsNullOrWhiteSpace(q))
            {
                filter = p => p.IsActive && p.Name.Contains(q);
            }
            else
            {
                filter = p => p.IsActive;
            }

            string sortBy = "CreatedAt";
            string sortDir = "desc";
            switch (sort)
            {
                case "price_asc":
                    sortBy = "Price"; sortDir = "asc"; break;
                case "price_desc":
                    sortBy = "Price"; sortDir = "desc"; break;
                case "name_asc":
                    sortBy = "Name"; sortDir = "asc"; break;
                case "name_desc":
                    sortBy = "Name"; sortDir = "desc"; break;
                case "newest":
                default:
                    sortBy = "CreatedAt"; sortDir = "desc"; break;
            }

            var pagination = new PaginationDto
            {
                PageIndex = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDir
            };

            var products = await productService.GetAllProductsAsync(filter, pagination);

            var categories = await categoryService.GetAllCategoriesByAsync();

            ViewBag.Categories = categories.Items;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SearchQuery = q;
            ViewBag.CurrentSort = sort ?? "newest";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = products.TotalPages;

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var product = await productService.GetProductByAsync(p => p.Id == id);
                
                var relatedProducts = await productService.GetAllProductsAsync(
                    p => p.CategoryId == product.CategoryId && p.Id != id && p.IsActive,
                    new PaginationDto { PageIndex = 1, PageSize = 4, SortBy = "CreatedAt", SortDirection = "desc" });

                ViewBag.RelatedProducts = relatedProducts.Items;

                return View(product);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
