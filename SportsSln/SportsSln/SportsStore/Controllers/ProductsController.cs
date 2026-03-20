using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SportsStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IStoreRepository repository;
        public int PageSize = 12; // ✅ 12 sản phẩm mỗi trang

        public ProductsController(IStoreRepository repo)
        {
            repository = repo;
        }

        // ========== TRANG DANH SÁCH SẢN PHẨM ==========
        public ViewResult Index(string? category, int productPage = 1)
        {
            long? categoryId = null;

            // ✅ Lấy categoryId nếu có category
            if (!string.IsNullOrEmpty(category))
            {
                string normalized = category.Replace("-", " ").Trim().ToLower();

                categoryId = repository.Categories
                    .AsEnumerable()
                    .Where(c => Slugify(c.Name) == Slugify(normalized))
                    .Select(c => c.CategoryID)
                    .FirstOrDefault();

                if (categoryId == 0)
                    categoryId = null;
            }

            // ✅ Lấy danh sách sản phẩm theo danh mục (nếu có)
            var query = repository.Products
                .Include(p => p.Category)
                .Where(p => categoryId == null || p.CategoryID == categoryId)
                .OrderBy(p => p.ProductID);

            // ✅ Tính tổng và chia trang
            var totalItems = query.Count();
            var products = query
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // ✅ Gửi sang ViewModel
            var viewModel = new ProductsListViewModel
            {
                Products = products,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemPerPage = PageSize,
                    TotalItems = totalItems
                },
                CurrentCategory = categoryId.HasValue
                    ? repository.Categories.FirstOrDefault(c => c.CategoryID == categoryId)?.Name
                    : null
            };

            return View(viewModel);
        }

        // ========== TRANG CHI TIẾT ==========
        public IActionResult Details(long id)
        {
            var product = repository.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            // Lấy 4 sản phẩm khác (khác sản phẩm hiện tại)
            ViewBag.OtherProducts = repository.Products
                .Where(p => p.ProductID != id)
                .Take(4)
                .ToList();

            return View(product);
        }


        // ========== HÀM SLUG KHÔNG DẤU ==========
        private string Slugify(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            string normalized = input.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var ch in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(ch);
                }
            }

            string slug = builder.ToString().Normalize(NormalizationForm.FormC)
                .ToLower()
                .Replace("đ", "d")
                .Replace(" ", "-");

            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
            return slug;
        }
    }
}
