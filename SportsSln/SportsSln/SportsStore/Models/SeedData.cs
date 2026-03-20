using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;

namespace SportsStore.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

            // 🔹 Tự động migrate nếu có thay đổi database
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            // --- Seed Categories ---
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Dây chuyền" },
                    new Category { Name = "Mặt dây chuyền" },
                    new Category { Name = "Bông tai" },
                    new Category { Name = "Lắc" },
                    new Category { Name = "Charm" },
                    new Category { Name = "Vòng tay" },
                    new Category { Name = "Kiềng" }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges(); // ✅ Lưu trước để có CategoryID thực tế
            }

            // ✅ Sau khi SaveChanges, đọc lại ID thật từ DB
            var categoriesDict = context.Categories
                .AsNoTracking()
                .ToDictionary(c => c.Name, c => c.CategoryID);

            // --- Seed Products ---
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    // Dây chuyền
                    new Product { Name = "Dây chuyền vàng 18K", Description = "Dây chuyền vàng 18K sang trọng", CategoryID = categoriesDict["Dây chuyền"], Price = 8500000, Quantity = 5, ImageUrl = "daychuyen1.jpg" },
                    new Product { Name = "Dây chuyền bạc Ý", Description = "Dây chuyền bạc Ý cao cấp", CategoryID = categoriesDict["Dây chuyền"], Price = 1200000, Quantity = 12, ImageUrl = "daychuyen2.jpg" },
                    new Product { Name = "Dây chuyền kim cương", Description = "Dây chuyền đính kim cương tự nhiên", CategoryID = categoriesDict["Dây chuyền"], Price = 24500000, Quantity = 2, ImageUrl = "daychuyen3.jpg" },
                    new Product { Name = "Dây chuyền ngọc trai", Description = "Dây chuyền ngọc trai trắng tinh tế", CategoryID = categoriesDict["Dây chuyền"], Price = 5600000, Quantity = 8, ImageUrl = "daychuyen4.jpg" },
                    new Product { Name = "Dây chuyền trái tim", Description = "Thiết kế mặt trái tim thời trang", CategoryID = categoriesDict["Dây chuyền"], Price = 2100000, Quantity = 0, ImageUrl = "daychuyen5.jpg" },
                    new Product { Name = "Dây chuyền pha lê", Description = "Dây chuyền pha lê Swarovski", CategoryID = categoriesDict["Dây chuyền"], Price = 3400000, Quantity = 15, ImageUrl = "daychuyen6.jpg" },

                    // Mặt dây chuyền
                    new Product { Name = "Mặt dây chuyền Bạc đính đá hình nơ", Description = "Mặt dây chuyền Bạc đính đá hình nơ", CategoryID = categoriesDict["Mặt dây chuyền"], Price = 650000, Quantity = 20, ImageUrl = "matday1.jpg" },
                    new Product { Name = "Mặt dây chuyền Bạc đính đá hình đuôi cá", Description = "Mặt dây chuyền Bạc đính đá hình đuôi cá", CategoryID = categoriesDict["Mặt dây chuyền"], Price = 1800000, Quantity = 0, ImageUrl = "matday2.jpg" },
                    new Product { Name = "Mặt dây chuyền Vàng Trắng 10K Đính đá ", Description = "Mặt dây chuyền Vàng Trắng 10K Đính đá ", CategoryID = categoriesDict["Mặt dây chuyền"], Price = 3200000, Quantity = 7, ImageUrl = "matday3.jpg" },
                    new Product { Name = "Mặt dây chuyền Kim cương Vàng Trắng ", Description = "Mặt dây chuyền Kim cương Vàng Trắng ", CategoryID = categoriesDict["Mặt dây chuyền"], Price = 8900000, Quantity = 3, ImageUrl = "matday4.jpg" },
                    new Product { Name = "Mặt dây chuyền Bạc đính đá ", Description = "Mặt dây chuyền Bạc đính đá ", CategoryID = categoriesDict["Mặt dây chuyền"], Price = 750000, Quantity = 18, ImageUrl = "matday5.jpg" },

                    // Bông tai
                    new Product { Name = "Bông tai vàng 24K", Description = "Bông tai vàng tròn trơn 24K", CategoryID = categoriesDict["Bông tai"], Price = 4500000, Quantity = 6, ImageUrl = "bongtai1.jpg" },
                    new Product { Name = "Bông tai ngọc trai", Description = "Bông tai ngọc trai tự nhiên", CategoryID = categoriesDict["Bông tai"], Price = 2900000, Quantity = 10, ImageUrl = "bongtai2.jpg" },
                    new Product { Name = "Bông tai kim cương", Description = "Bông tai đính kim cương 18K", CategoryID = categoriesDict["Bông tai"], Price = 15800000, Quantity = 1, ImageUrl = "bongtai3.jpg" },
                    new Product { Name = "Bông tai bạc hoa hồng", Description = "Bông tai bạc hình hoa hồng tinh xảo", CategoryID = categoriesDict["Bông tai"], Price = 850000, Quantity = 14, ImageUrl = "bongtai4.jpg" },
                    new Product { Name = "Bông tai giọt nước", Description = "Thiết kế giọt nước thanh lịch", CategoryID = categoriesDict["Bông tai"], Price = 1300000, Quantity = 0, ImageUrl = "bongtai5.jpg" },

                    // Lắc
                    new Product { Name = "Lắc vàng nữ", Description = "Lắc vàng 18K kiểu Ý sang trọng", CategoryID = categoriesDict["Lắc"], Price = 8900000, Quantity = 4, ImageUrl = "lac1.jpg" },
                    new Product { Name = "Lắc bạc trái tim", Description = "Lắc bạc đính charm hình trái tim", CategoryID = categoriesDict["Lắc"], Price = 1250000, Quantity = 9, ImageUrl = "lac2.jpg" },
                    new Product { Name = "Lắc phong thủy đá mã não", Description = "Lắc tay đá mã não đỏ tự nhiên", CategoryID = categoriesDict["Lắc"], Price = 1600000, Quantity = 11, ImageUrl = "lac3.jpg" },
                    new Product { Name = "Lắc vàng hồng", Description = "Lắc vàng hồng 14K đính đá trắng", CategoryID = categoriesDict["Lắc"], Price = 5200000, Quantity = 0, ImageUrl = "lac4.jpg" },
                    new Product { Name = "Lắc tay đôi", Description = "Cặp lắc tay bạc đôi khắc tên", CategoryID = categoriesDict["Lắc"], Price = 1550000, Quantity = 16, ImageUrl = "lac5.jpg" },

                    // Charm
                    new Product { Name = "Charm trái tim đỏ", Description = "Charm bạc hình trái tim đỏ", CategoryID = categoriesDict["Charm"], Price = 350000, Quantity = 25, ImageUrl = "charm1.jpg" },
                    new Product { Name = "Charm ngôi sao", Description = "Charm bạc ngôi sao đính đá", CategoryID = categoriesDict["Charm"], Price = 420000, Quantity = 22, ImageUrl = "charm2.jpg" },
                    new Product { Name = "Charm vàng hình mèo thần tài", Description = "Charm vàng 18K hình mèo thần tài", CategoryID = categoriesDict["Charm"], Price = 2800000, Quantity = 0, ImageUrl = "charm3.jpg" },
                    new Product { Name = "Charm bướm vàng", Description = "Charm hình bướm đính đá", CategoryID = categoriesDict["Charm"], Price = 950000, Quantity = 13, ImageUrl = "charm4.jpg" },
                    new Product { Name = "Charm hoa hồng", Description = "Charm bạc hình hoa hồng", CategoryID = categoriesDict["Charm"], Price = 680000, Quantity = 19, ImageUrl = "charm5.jpg" }
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
