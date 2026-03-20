using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SportsStore.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Admin123";

        public static async Task EnsurePopulated(IApplicationBuilder app)
        {
            // ✅ Tạo scope riêng để truy cập dịch vụ
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // ✅ Cập nhật cơ sở dữ liệu nếu có migration mới
            await context.Database.MigrateAsync();

            // ✅ Tạo role nếu chưa có
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ✅ Tạo user Admin mặc định nếu chưa có
            var admin = await userManager.FindByNameAsync(adminUser);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminUser,
                    Email = "admin@example.com",
                    FullName = "Administrator",
                    Address = "Head Office",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    Console.WriteLine($"[SeedData] Failed to create Admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ✅ Tạo user Customer mặc định nếu chưa có
            var customerUser = await userManager.FindByNameAsync("HoangNam");
            if (customerUser == null)
            {
                customerUser = new ApplicationUser
                {
                    UserName = "HoangNam",
                    Email = "hnam.11102005@gmail.com",
                    FullName = "Lê Hoàng Nam",
                    Address = "TP. Hồ Chí Minh",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(customerUser, "1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customerUser, "Customer");

                    // Verify the role was actually added
                    var roles_assigned = await userManager.GetRolesAsync(customerUser);
                    if (!roles_assigned.Contains("Customer"))
                    {
                        Console.WriteLine($"[SeedData] WARNING: Failed to assign Customer role to Customer1");
                    }
                }
                else
                {
                    Console.WriteLine($"[SeedData] Failed to create Customer: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else if (!await userManager.IsInRoleAsync(customerUser, "Customer"))
            {
                await userManager.AddToRoleAsync(customerUser, "Customer");
            }
        }
    }
}
