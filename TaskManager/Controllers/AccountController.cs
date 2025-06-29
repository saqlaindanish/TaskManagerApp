using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;

namespace TaskManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return PartialView("_RegistrationPartialView", new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Optionally, you can sign in the user after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "User") });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return PartialView("_RegistrationPartialView", model);
            }

            return PartialView("_RegistrationPartialView", model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return PartialView("_LoginPartialView", new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
               
                if (result.Succeeded)
                {
                    return Json(new {success = true, redirectUrl = Url.Action("Dashboard","User")});
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return PartialView("_LoginPartialView", model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
