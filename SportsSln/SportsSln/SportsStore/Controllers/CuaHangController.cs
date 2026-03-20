using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Controllers
{
    public class CuahangController : Controller
    {
        // Trang hiển thị danh sách cửa hàng
        public IActionResult Index()
        {
            return View();
        }
    }
}
