using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Controllers
{
    public class GioiThieuController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/GioiThieu/Index.cshtml");
        }
    }
}
