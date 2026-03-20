using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Controllers
{
    public class TrangChuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
