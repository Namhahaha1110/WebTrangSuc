using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace SportsStore.Controllers
{
    public class CustomerAccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppIdentityDbContext _context; // 👈 Dùng để log DB

        public CustomerAccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppIdentityDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // -------------------- ĐĂNG KÝ --------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            Console.WriteLine($"[DEBUG] 🧩 Identity DB: {_context.Database.GetDbConnection().ConnectionString}");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Address = model.Address,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber, // ✅ Bổ sung số điện thoại
                    Role = "Customer"
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // ✅ Lưu thông tin vào session
                    HttpContext.Session.SetString("CustomerName", user.FullName ?? user.UserName);
                    HttpContext.Session.SetString("Role", "Customer");
                    await HttpContext.Session.CommitAsync();

                    Console.WriteLine($"[DEBUG] ✅ Đăng ký thành công: {user.UserName}");
                    return RedirectToAction("Index", "Products"); // 🔥 Chuyển sang danh sách sản phẩm
                }

                // Nếu lỗi — ghi log + hiển thị lỗi validation
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"[ERROR] {error.Code} - {error.Description}");
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }


        // -------------------- ĐĂNG NHẬP --------------------
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            Console.WriteLine("=== LOGIN POST CHẠY ===");
            Console.WriteLine($"[DEBUG] 🧩 Identity DB: {_context.Database.GetDbConnection().ConnectionString}");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Name);
                if (user == null)
                {
                    Console.WriteLine("[DEBUG] ❌ Không tìm thấy user");
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                Console.WriteLine($"[DEBUG] Kết quả SignIn: {result.Succeeded}");

                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("CustomerName", user.FullName ?? user.UserName);
                    HttpContext.Session.SetString("Role", user.Role ?? "Customer");
                    await HttpContext.Session.CommitAsync();

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        Console.WriteLine("[DEBUG] ✅ Admin đăng nhập thành công → /Admin");
                        return RedirectToPage("/Admin/Index");
                    }
                    else
                    {
                        Console.WriteLine("[DEBUG] ✅ Customer đăng nhập thành công → /Products");
                        return RedirectToAction("Index", "Products"); // 🔥 Chuyển sang trang sản phẩm
                    }
                }

                Console.WriteLine("[DEBUG] ❌ Sai mật khẩu hoặc không đăng nhập được");
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        // -------------------- ĐĂNG XUẤT --------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();

            // 🔥 Sau khi đăng xuất quay về danh sách sản phẩm
            return RedirectToAction("Index", "Products");
        }

        // -------------------- TÀI KHOẢN CỦA TÔI --------------------
        [HttpGet]
        public async Task<IActionResult> MyAccount()
        {
            // Lấy tên đăng nhập hiện tại
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                // Nếu chưa đăng nhập thì quay lại trang Login
                return RedirectToAction("Login", "CustomerAccount");
            }

            // Tìm user trong DB
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("Không tìm thấy tài khoản.");
            }

            // Trả về ViewModel kiểu mạnh để tránh lỗi runtime binder trong Razor View
            var model = new EditAccountViewModel
            {
                NameCus = user.FullName,
                PhoneCus = user.PhoneNumber,
                EmailCus = user.Email,
                UserName = user.UserName,
                Password = "********", // Không hiển thị thật
                IDCus = user.Id,
                Tier = user.Tier,
                TotalSpent = user.TotalSpent
            };

            return View(model);
        }
        // -------------------- CHỈNH SỬA TÀI KHOẢN --------------------
        [HttpGet]
        public async Task<IActionResult> EditAccount(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new EditAccountViewModel
            {
                IDCus = user.Id,
                NameCus = user.FullName,
                PhoneCus = user.PhoneNumber,
                EmailCus = user.Email,
                UserName = user.UserName,
                Password = "" // để trống cho người dùng nhập lại
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAccount(EditAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.IDCus);
            if (user == null)
                return NotFound();

            // Cập nhật thông tin
            user.FullName = model.NameCus;
            user.PhoneNumber = model.PhoneCus;
            user.Email = model.EmailCus;

            // Nếu có nhập mật khẩu mới
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPass = await _userManager.ResetPasswordAsync(user, token, model.Password);

                if (!resetPass.Succeeded)
                {
                    foreach (var err in resetPass.Errors)
                        ModelState.AddModelError("", err.Description);
                    return View(model);
                }
            }

            // Lưu thay đổi
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("MyAccount", "CustomerAccount");
            }

            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);

            return View(model);
        }

    }
}
