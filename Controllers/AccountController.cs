using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using minesweeper.Models.Db;
using Microsoft.AspNetCore.Identity;

namespace minesweeper.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(details.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    var result = await signInManager.PasswordSignInAsync(user, details.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(LoginModel.Email), "Invalid user or password");
            }
            return View(details);
        }

        public IActionResult Sign() => View();
        [HttpPost]
        public async Task<IActionResult> Sign(SignModel details)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = details.Name,
                    Email = details.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, details.Password);
                if (result.Succeeded) return RedirectToAction("Login");
                result.Errors.ToList().ForEach(error => ModelState.AddModelError("", error.Description));
            }
            return View(details);
        }
    }
}
