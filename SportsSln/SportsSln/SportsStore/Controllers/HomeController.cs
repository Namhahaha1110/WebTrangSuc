using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Linq;

namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoreRepository repository;
        public int PageSize = 4;

        public HomeController(IStoreRepository repo)
        {
            repository = repo;
        }

        // Lọc sản phẩm theo tên danh mục (category)
        public ViewResult Index(string? category, int productPage = 1)
        {
            var filteredProducts = repository.Products
                .Where(p => category == null || p.Category.Name == category)
                .OrderBy(p => p.ProductID);

            return View(new ProductsListViewModel
            {
                Products = filteredProducts
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemPerPage = PageSize,
                    TotalItems = category == null
                        ? repository.Products.Count()
                        : repository.Products.Count(p => p.Category.Name == category)
                },

                CurrentCategory = category
            });
        }

        // Test session (giữ nguyên)
        public IActionResult TestSession()
        {
            HttpContext.Session.SetString("TestKey", "HelloSession");
            var value = HttpContext.Session.GetString("TestKey");
            return Content($"Session value = {value}");
        }
    }
}
