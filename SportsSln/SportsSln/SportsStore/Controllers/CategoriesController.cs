using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    [Route("admin/categories")]
    public class CategoriesController : Controller
    {
        private readonly StoreDbContext _context;

        public CategoriesController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        [HttpPost("create")]
        public IActionResult Create(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var category = new Category { Name = name };
                _context.Categories.Add(category);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var cat = _context.Categories.Find(id);
            if (cat != null)
            {
                _context.Categories.Remove(cat);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
