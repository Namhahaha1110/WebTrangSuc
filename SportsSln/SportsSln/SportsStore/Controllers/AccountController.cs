using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System;
using System.Threading.Tasks;

namespace SportsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Login
        public ViewResult Login(string returnUrl = null)
        {
            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            var user = await _userManager.FindByNameAsync(loginModel.Name);
            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(loginModel);
            }

            // Lấy role trước khi login
            var roles = await _userManager.GetRolesAsync(user);

            // Nếu là khách hàng cố login tài khoản Admin
            bool isCustomerTryingAdmin = !roles.Contains("Admin") &&
                                         (!string.IsNullOrEmpty(loginModel.ReturnUrl) && loginModel.ReturnUrl.StartsWith("/Admin"));
            if (isCustomerTryingAdmin)
            {
                // Hiển thị thông báo thân thiện
                ModelState.AddModelError("", "Đây là tài khoản Admin, vui lòng không truy cập.");
                return View(loginModel);
            }

            await _signInManager.SignOutAsync();
            var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(loginModel);
            }

            // Redirect theo role
            if (roles.Contains("Admin"))
            {
                return Redirect("/Admin");
            }
            else
            {
                return RedirectToAction("Login", "CustomerAccount");

            }
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // hoặc HttpContext.SignOutAsync()
            return RedirectToAction("Login", "Account");
        }

    }
}


