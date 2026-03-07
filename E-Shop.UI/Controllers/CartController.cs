using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Models;
using E_Shop.Domain.RepositoryContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.UI.Controllers
{
    [Authorize]
    public class CartController(IShoppingCartService cartService,
        IProductService productService,
        IUserContext userContext) : Controller
    {
        private async Task<string> GetUserIdAsync()
        {
            var user = await userContext.GetCurrentUserAsync();
            return user?.Id.ToString() ?? string.Empty;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await GetUserIdAsync();
            var cart = await cartService.GetCartAsync(userId);

            // Refresh stock quantities for all items
            foreach (var item in cart.CartItems)
            {
                var product = await productService.GetProductByAsync(p => p.Id == item.ProductID);
                if (product != null)
                {
                    item.StockQuantity = product.StockQuantity;
                }
            }

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Guid productId, int quantity = 1)
        {
            var userId = await GetUserIdAsync();
            var product = await productService.GetProductByAsync(p => p.Id == productId);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index", "Catalog");
            }

            if (product.StockQuantity < quantity)
            {
                TempData["ErrorMessage"] = "Insufficient stock.";
                return RedirectToAction("Details", "Catalog", new { id = productId });
            }

            var cartItem = new CartItems
            {
                ProductID = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity,
                PictureUrl = product.ImageUrls?.FirstOrDefault() ?? string.Empty,
                BrandName = string.Empty,
                StockQuantity = product.StockQuantity
            };

            await cartService.AddToCartAsync(userId, cartItem);

            TempData["SuccessMessage"] = $"'{product.Name}' added to cart!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid productId, int quantity)
        {
            var userId = await GetUserIdAsync();
            var product = await productService.GetProductByAsync(p => p.Id == productId);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            if (quantity > product.StockQuantity)
            {
                TempData["ErrorMessage"] = $"Only {product.StockQuantity} items available in stock.";
                return RedirectToAction(nameof(Index));
            }

            await cartService.UpdateQuantityAsync(userId, productId, quantity);

            // Refresh stock quantity in cart
            var cart = await cartService.GetCartAsync(userId);
            var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductID == productId);
            if (cartItem != null)
            {
                cartItem.StockQuantity = product.StockQuantity;
                await cartService.UpdateQuantityAsync(userId, productId, quantity);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid productId)
        {
            var userId = await GetUserIdAsync();
            await cartService.RemoveFromCartAsync(userId, productId);
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = await GetUserIdAsync();
            var cart = await cartService.GetCartAsync(userId);
            return Json(new { count = cart.CartItems.Sum(i => i.Quantity) });
        }
    }
}
