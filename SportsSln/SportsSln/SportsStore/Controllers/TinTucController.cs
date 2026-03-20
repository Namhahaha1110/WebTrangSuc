using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Controllers
{
    public class TintucController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}