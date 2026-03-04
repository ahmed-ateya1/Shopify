using E_Shop.Application.Dtos.AccountDtos;
using E_Shop.Domain.Enums;
using E_Shop.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.UI.Controllers
{
    public class AccountController(UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager)
        : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList();
                return View(registerDTO);
            }
            var user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                FullName = registerDTO.FullName
            };
            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                var roleType = registerDTO.UserOption == UserOption.ADMIN ?
                    nameof(UserOption.ADMIN) : nameof(UserOption.USER);

                if (await roleManager.FindByNameAsync(roleType) == null)
                {
                    ApplicationRole applicationRole = new ApplicationRole() { Name = roleType };
                    var resultRole = await roleManager.CreateAsync(applicationRole);
                    if (!resultRole.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Failed Add Error");
                    }
                }
                var resultAddRoleToUser = await userManager.AddToRoleAsync(user, roleType);
                if (!resultAddRoleToUser.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed asign role to user");
                    return View(registerDTO);
                }
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(registerDTO);
        }
        [HttpGet]
        public IActionResult Login(string returnURL = null)
        {
            ViewBag.ReturnURL = returnURL;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDTO, string returnURL = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnURL = returnURL;
                return View(loginDTO);
            }
            var result = await signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user =
                    await userManager.FindByNameAsync(loginDTO.Email);
                if (user != null && await userManager.IsInRoleAsync(user, UserOption.ADMIN.ToString()))
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!String.IsNullOrEmpty(returnURL) && Url.IsLocalUrl(returnURL))
                {
                    return LocalRedirect(returnURL);
                }
                return RedirectToAction("Index", "Product");
            }


            ModelState.AddModelError(string.Empty, "Invaild username or password");
            ViewBag.ReturnURL = returnURL;
            return View(loginDTO);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> UniqueEmail(string Email)
        {
            if (await userManager.FindByEmailAsync(Email) == null)
                return Json(true);
            return Json(false);
        }
    }
}
