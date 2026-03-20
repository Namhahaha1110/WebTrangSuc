using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsStore.Infrastructure;
using SportsStore.Models;
using System.Linq;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;

namespace SportsStore.Pages
{
    public class CartModel : PageModel
    {
        private readonly IStoreRepository repository;
        private readonly IBankService bankService;

        // Tránh trùng tên với class Cart
        public Cart ShoppingCart { get; set; }
        public string ReturnUrl { get; set; } = "/";

        public CartModel(IStoreRepository repo, Cart cartService, IBankService bankService)
        {
            repository = repo;
            ShoppingCart = cartService;
            this.bankService = bankService;
        }

        public void OnGet(string? returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
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
