using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using SportsStore.Infrastructure;
using SportsStore.Models;
using System.Linq;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SportsStore.Pages
{
    public class CartModel : PageModel
    {
        private readonly IStoreRepository repository;
        private readonly IBankService bankService;
        private readonly UserManager<ApplicationUser> userManager;

        // Tránh trùng tên với class Cart
        public Cart ShoppingCart { get; set; }
        public string ReturnUrl { get; set; } = "/";
        public string PrefillName { get; private set; } = string.Empty;
        public string PrefillPhone { get; private set; } = string.Empty;
        public string PrefillEmail { get; private set; } = string.Empty;
        public string PrefillProvince { get; private set; } = string.Empty;
        public string PrefillDistrict { get; private set; } = string.Empty;
        public string PrefillWard { get; private set; } = string.Empty;
        public string PrefillAddressDetail { get; private set; } = string.Empty;

        public CartModel(IStoreRepository repo, Cart cartService, IBankService bankService, UserManager<ApplicationUser> userManager)
        {
            repository = repo;
            ShoppingCart = cartService;
            this.bankService = bankService;
            this.userManager = userManager;
        }

        public async Task OnGet(string? returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            PrefillName = user.FullName ?? string.Empty;
            PrefillPhone = user.PhoneNumber ?? string.Empty;
            PrefillEmail = user.Email ?? string.Empty;
            ParseAddressForPrefill(user.Address);
        }

        private void ParseAddressForPrefill(string? address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return;
            }

            var parts = address
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToArray();

            if (parts.Length >= 4)
            {
                PrefillProvince = parts[^1];
                PrefillDistrict = parts[^2];
                PrefillWard = parts[^3];
                PrefillAddressDetail = string.Join(", ", parts.Take(parts.Length - 3));
                return;
            }

            PrefillAddressDetail = address.Trim();
        }

        // Thêm sản phẩm
        public IActionResult OnPostAdd(long productId, int quantity = 1)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return new JsonResult(new { success = false, loginRequired = true, loginUrl = "/CustomerAccount/Login" });
            }

            var product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                ShoppingCart.AddItem(product, quantity);
            }

            return new JsonResult(new { success = true });
        }

        // Mua ngay
        public IActionResult OnPostBuyNow(long productId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Redirect("/CustomerAccount/Login");
            }

            var product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                ShoppingCart.AddItem(product, 1);
            }

            return RedirectToPage("/Cart");
        }

        // Cập nhật số lượng
        public JsonResult OnPostUpdateQuantity(long productId, int quantity)
        {
            try
            {
                if (quantity < 1)
                {
                    return new JsonResult(new { success = false, message = "Số lượng phải lớn hơn 0" }) { StatusCode = 400 };
                }

                var product = repository.Products.FirstOrDefault(p => p.ProductID == productId);

                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Sản phẩm không tồn tại" }) { StatusCode = 404 };
                }

                ShoppingCart.UpdateQuantity(product, quantity);

                return new JsonResult(new
                {
                    success = true,
                    total = ShoppingCart.ComputeTotalValue()
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
            }
        }

        // Thanh toán
        public IActionResult OnPostCheckout(string name, string phone, string email, string note,
                              string addressDetail, string ward, string district, string province,
                              string deliveryType, string store, string city, string payment)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Redirect("/CustomerAccount/Login");

            // Nếu chọn Bank, kiểm tra thanh toán
            if (payment == "Bank")
            {
                if (!bankService.CheckPayment(ShoppingCart.OrderId, ShoppingCart.ComputeTotalValue()))
                {
                    ModelState.AddModelError("PaymentError", "VUI LÒNG THANH TOÁN TRƯỚC KHI ĐẶT ĐƠN HOẶC CHỌN THANH TOÁN KHÁC");
                    return Page();
                }
            }


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = new Order
            {
                UserId = userId,
                Name = name,
                Phone = phone,
                Email = email,
                Note = note,
                DeliveryType = deliveryType,
                AddressDetail = addressDetail,
                Ward = ward,
                District = district,
                Province = province,
                Store = store,
                City = city,
                TotalPrice = ShoppingCart.ComputeTotalValue(),
                Payment = payment,
                CreatedDate = DateTime.Now
            };

            foreach (var line in ShoppingCart.Lines)
            {
                order.Lines.Add(new CartLine
                {
                    ProductID = line.Product.ProductID,
                    Quantity = line.Quantity,
                    Price = line.Product.Price
                });
            }

            var context = (StoreDbContext)HttpContext.RequestServices.GetService(typeof(StoreDbContext));
            context.Orders.Add(order);
            context.SaveChanges();

            ShoppingCart.Clear();

            return RedirectToPage("/Completed", new { orderId = order.OrderID });
        }
    }

    // ======= Service kiểm tra QR thanh toán =======
    public interface IBankService
    {
        bool CheckPayment(string orderId, decimal amount);
    }

    public class BankService : IBankService
    {
        // TODO: Thay bằng kiểm tra thực tế qua DB hoặc API ngân hàng
        public bool CheckPayment(string orderId, decimal amount)
        {
            // Tạm thời trả false => chưa thanh toán
            return false;
        }
    }
}
