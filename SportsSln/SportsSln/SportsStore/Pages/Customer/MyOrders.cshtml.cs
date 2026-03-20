using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using SportsStore.Models;
using System.Security.Claims;

namespace SportsStore.Pages.Customer
{
    [Authorize]
    public class MyOrdersModel : PageModel
    {
        private readonly IOrderRepository _repository;

        public List<Order> CustomerOrders { get; set; } = new();

        public MyOrdersModel(IOrderRepository repository)
        {
            _repository = repository;
        }

        public void OnGet()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value
                ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(userEmail))
            {
                CustomerOrders = new List<Order>();
                return;
            }

            CustomerOrders = _repository.Orders
                .Where(o =>
                    (!string.IsNullOrEmpty(userId) && o.UserId == userId)
                    || (!string.IsNullOrEmpty(userEmail) && string.IsNullOrEmpty(o.UserId) && o.Email == userEmail))
                .OrderByDescending(o => o.CreatedDate)
                .ToList();
        }
    }
}
