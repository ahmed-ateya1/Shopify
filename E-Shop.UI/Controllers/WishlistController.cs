using E_Shop.Application.Dtos;
using E_Shop.Application.Dtos.WishlistDtos;
using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Models;
using E_Shop.Domain.RepositoryContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.UI.Controllers
{
    [Authorize]
    public class WishlistController(IWishlistService wishlistService,
        IUserContext userContext) : Controller
    {
        public async Task<IActionResult> Index(int page = 1)
        {
            var user = await userContext.GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var pagination = new PaginationDto
            {
                PageIndex = page,
                PageSize = 12,
                SortBy = "AddedAt",
                SortDirection = "desc"
            };

            var wishlist = await wishlistService.GetWishlistAsync(
                w => w.UserId == user.Id, pagination);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = wishlist.TotalPages;

            return View(wishlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Guid productId, string? returnUrl)
        {
            var user = await userContext.GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            try
            {
                await wishlistService.AddToWishlistAsync(new WishlistAddRequest(user.Id, productId));
                TempData["SuccessMessage"] = "Added to wishlist!";
            }
            catch (InvalidOperationException)
            {
                TempData["InfoMessage"] = "Already in your wishlist.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Product not found.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid id)
        {
            await wishlistService.RemoveFromWishlistAsync(id);
            TempData["SuccessMessage"] = "Removed from wishlist.";
            return RedirectToAction(nameof(Index));
        }
    }
}
