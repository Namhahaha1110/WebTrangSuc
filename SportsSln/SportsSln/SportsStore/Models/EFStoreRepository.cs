using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class EFStoreRepository : IStoreRepository
    {
        private readonly StoreDbContext _context;

        public EFStoreRepository(StoreDbContext ctx)
        {
            _context = ctx;
        }

        public IQueryable<Product> Products => _context.Products;
        public IQueryable<Category> Categories => _context.Categories;
        public IQueryable<Banner> Banners => _context.Banners;

        // ---------------- SẢN PHẨM ----------------

        public async Task CreateProduct(Product p)
        {
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
        }

        public async Task SaveProduct(Product p)
        {
            var existing = await _context.Products
                .FirstOrDefaultAsync(x => x.ProductID == p.ProductID);

            if (existing != null)
            {
                existing.Name = p.Name;
                existing.Description = p.Description;
                existing.Price = p.Price;
                existing.CategoryID = p.CategoryID;
                existing.ImageUrl = p.ImageUrl;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProduct(Product p)
        {
            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
        }

        // ---------------- PHÂN LOẠI ----------------

        public async Task CreateCategory(Category c)
        {
            _context.Categories.Add(c);
            await _context.SaveChangesAsync();
        }

        public async Task SaveCategory(Category c)
        {
            var existing = await _context.Categories
                .FirstOrDefaultAsync(x => x.CategoryID == c.CategoryID);

            if (existing != null)
            {
                existing.Name = c.Name;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCategory(Category c)
        {
            _context.Categories.Remove(c);
            await _context.SaveChangesAsync();
        }

        // ---------------- BANNER ----------------

        public async Task CreateBanner(Banner b)
        {
            _context.Banners.Add(b);
            await _context.SaveChangesAsync();
        }

        public async Task SaveBanner(Banner b)
        {
            var existing = await _context.Banners
                .FirstOrDefaultAsync(x => x.BannerID == b.BannerID);

            if (existing != null)
            {
                existing.Title = b.Title;
                existing.ImageUrl = b.ImageUrl;
                existing.RedirectUrl = b.RedirectUrl;
                existing.DisplayOrder = b.DisplayOrder;
                existing.FocusX = b.FocusX;
                existing.FocusY = b.FocusY;
                existing.IsActive = b.IsActive;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBanner(Banner b)
        {
            _context.Banners.Remove(b);
            await _context.SaveChangesAsync();
        }

    }
}
