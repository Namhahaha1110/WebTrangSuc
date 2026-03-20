using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using System.Linq;

namespace SportsStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepo;

        public OrderController(IOrderRepository orderRepository)
        {
            orderRepo = orderRepository;
        }

        // ============================
        // 1. DANH SÁCH ĐƠN HÀNG
        // ============================
        public IActionResult Index()
        {
            var orders = orderRepo.Orders
                .OrderByDescending(o => o.CreatedDate)
                .ToList();

            return View(orders);
        }

        // ============================
        // 2. CHI TIẾT ĐƠN HÀNG
        // ============================
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var order = orderRepo.Orders
                .FirstOrDefault(o => o.OrderID == id.Value);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // ============================
        // 3. TẠO ĐƠN HÀNG (GET)
        // ============================
        public IActionResult Create()
        {
            return View();
        }

        // ============================
        // 4. TẠO ĐƠN HÀNG (POST)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                order.CreatedDate = DateTime.Now;

                // Lưu đơn hàng
                orderRepo.SaveOrder(order);

                // 👉 Redirect tới Razor Page Completed
                return RedirectToPage("/Order/Completed", new { orderId = order.OrderID });
            }

            return View(order);
        }

        // ============================
        // 5. EDIT ĐƠN HÀNG (GET)
        // ============================
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var order = orderRepo.Orders
                .FirstOrDefault(o => o.OrderID == id.Value);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // ============================
        // 6. EDIT ĐƠN HÀNG (POST)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Order order)
        {
            if (id != order.OrderID)
                return NotFound();

            if (ModelState.IsValid)
            {
                orderRepo.SaveOrder(order);
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // ============================
        // 7. XÓA ĐƠN HÀNG (GET)
        // ============================
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var order = orderRepo.Orders
                .FirstOrDefault(o => o.OrderID == id.Value);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // ============================
        // 8. XÓA ĐƠN HÀNG (POST)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = orderRepo.Orders.FirstOrDefault(o => o.OrderID == id);

            if (order == null)
                return NotFound();

            orderRepo.SaveOrder(order);

            return RedirectToAction(nameof(Index));
        }
    }
}
