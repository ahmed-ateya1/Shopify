using E_Shop.Application.ServicesContract;
using E_Shop.Domain.Models;
using E_Shop.Domain.RepositoryContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.UI.Controllers
{
    [Authorize]
    public class CheckoutController(IShoppingCartService cartService,
        IOrderService orderService,
        IUnitOfWork unitOfWork,
        IUserContext userContext) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userContext.GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var cart = await cartService.GetCartAsync(user.Id.ToString());
            if (!cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Load user addresses
            var addresses = await unitOfWork.Repository<Address>()
                .GetAllAsync(a => a.UserId == user.Id);

            ViewBag.Cart = cart;
            ViewBag.Addresses = addresses ?? Enumerable.Empty<Address>();
            ViewBag.UserName = user.FullName;
            ViewBag.UserEmail = user.Email;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(Guid? addressId, string? country, string? city, string? street, string? zipCode)
        {
            var user = await userContext.GetCurrentUserAsync();
            if (user == null) return RedirectToAction("Login", "Account");

            var cart = await cartService.GetCartAsync(user.Id.ToString());
            if (!cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                Guid shippingAddressId;

                if (addressId.HasValue && addressId.Value != Guid.Empty)
                {
                    shippingAddressId = addressId.Value;
                }
                else
                {
                    // Create new address
                    if (string.IsNullOrWhiteSpace(country) || string.IsNullOrWhiteSpace(city)
                        || string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(zipCode))
                    {
                        TempData["ErrorMessage"] = "Please provide a complete shipping address.";
                        return RedirectToAction(nameof(Index));
                    }

                    var address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = country,
                        City = city,
                        Street = street,
                        ZipCode = zipCode,
                        UserId = user.Id,
                        IsDefault = false
                    };

                    // Check if user has no addresses, make this default
                    var existingAddresses = await unitOfWork.Repository<Address>()
                        .AnyAsync(a => a.UserId == user.Id);
                    if (!existingAddresses) address.IsDefault = true;

                    await unitOfWork.Repository<Address>().CreateAsync(address);
                    await unitOfWork.CompleteAsync();
                    shippingAddressId = address.Id;
                }

                // Atomic checkout: validate stock, create order, items, decrease stock
                var orderId = await orderService.CreateOrderAsync(user.Id, shippingAddressId, cart.CartItems);

                // Clear cart after successful order
                await cartService.ClearCartAsync(user.Id.ToString());

                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("Details", "Orders", new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
