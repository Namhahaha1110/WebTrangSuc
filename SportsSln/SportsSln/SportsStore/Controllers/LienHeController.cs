using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Controllers
{
    public class LienHeController : Controller
    {
        // GET: Contact (hiển thị form)
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: Contact (nhận dữ liệu từ form)
        [HttpPost]
        public ActionResult Index(string chude, string tieude, string noidung, string hoten, string email, string sdt)
        {
            // Xử lý dữ liệu ở đây
            ViewBag.Message = "Bạn đã gửi thành công!";
            ViewBag.Data = $"Chủ đề: {chude}, Tiêu đề: {tieude}, Nội dung: {noidung}, Họ tên: {hoten}, Email: {email}, SĐT: {sdt}";

            return View();
        }
    }
}
