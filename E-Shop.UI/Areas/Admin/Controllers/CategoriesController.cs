using E_Shop.Application.Dtos;
using E_Shop.Application.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class CategoriesController(ICategoryService categoryService) : Controller
    {
        public async Task<IActionResult> Index(int page = 1)
        {
            var pagination = new PaginationDto { PageIndex = page, PageSize = 10, SortBy = "Name", SortDirection = "asc" };
            var categories = await categoryService.GetAllCategoriesByAsync(pagination: pagination);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = categories.TotalPages;

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var allCategories = await categoryService.GetAllCategoriesByAsync();
            ViewBag.ParentCategories = allCategories.Items;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAddRequest request)
        {
            try
            {
                await categoryService.AddCategoryAsync(request);
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var allCategories = await categoryService.GetAllCategoriesByAsync();
                ViewBag.ParentCategories = allCategories.Items;
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var category = await categoryService.GetCategoryByAsync(c => c.Id == id);
                var allCategories = await categoryService.GetAllCategoriesByAsync();
                ViewBag.ParentCategories = allCategories.Items.Where(c => c.Id != id);

                var updateRequest = new CategoryUpdateRequest(category.Id, category.Name, null);
                return View(updateRequest);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryUpdateRequest request)
        {
            try
            {
                await categoryService.UpdateCategoryAsync(request);
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var allCategories = await categoryService.GetAllCategoriesByAsync();
                ViewBag.ParentCategories = allCategories.Items.Where(c => c.Id != request.Id);
                return View(request);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await categoryService.DeleteCategoryAsync(id);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
