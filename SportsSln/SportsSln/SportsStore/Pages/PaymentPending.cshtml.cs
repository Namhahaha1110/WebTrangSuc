using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsStore.Models;
using System.Security.Claims;

namespace SportsStore.Pages
{
    [Authorize]
    public class PaymentPendingModel : PageModel
    {
        private readonly IOrderRepository orderRepository;

        public PaymentPendingModel(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public Order? CurrentOrder { get; private set; }
        public string ApiOrderCode { get; private set; } = string.Empty;
        public decimal TotalAmount { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public IActionResult OnGet(int orderId, string? status, string? code)
        {
            if (!TryLoadOrder(orderId, out var order))
            {
                return RedirectToPage("/Customer/MyOrders");
            }

            var loadedOrder = order!;

            CurrentOrder = loadedOrder;
            ApiOrderCode = $"ORDER-{loadedOrder.OrderID}";
            TotalAmount = loadedOrder.TotalPrice;

            if (loadedOrder.Payment == "VNPAY_PAID" || loadedOrder.Payment == "BANK_PAID")
            {
                return RedirectToPage("/Completed", new { orderId = loadedOrder.OrderID });
            }

            if (string.Equals(status, "failed", StringComparison.OrdinalIgnoreCase))
            {
                Message = $"Thanh toán chưa thành công (mã: {code ?? "N/A"}). Bạn có thể thanh toán lại bằng VNPAY.";
            }
            else
            {
                Message = "Đơn hàng đang chờ thanh toán online qua VNPAY.";
            }

            return Page();
        }

        private bool TryLoadOrder(int orderId, out Order? order)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            order = orderRepository.Orders.FirstOrDefault(o => o.OrderID == orderId && o.UserId == userId);
            return order != null;
        }
    }
}
