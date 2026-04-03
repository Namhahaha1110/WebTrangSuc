using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using Microsoft.OpenApi.Models;
using SportsStore.Pages;


var builder = WebApplication.CreateBuilder(args);

// ------------------ DỊCH VỤ ------------------

// MVC + Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

// Cart session
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));

// Blazor Server
builder.Services.AddServerSideBlazor();
builder.Services.AddAuthorizationCore();

// --- Database cho sản phẩm ---
builder.Services.AddDbContextFactory<StoreDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("SportsStoreConnection")));

// Repository EF
builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
builder.Configuration.GetConnectionString("DefaultConnection");

// --- Database cho Identity ---
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

// Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".SportsStore.Auth";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

// Swagger (API)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IBotService, SimpleBotService>();
builder.Services.AddSignalR();


// ------------------ BUILD APP ------------------
var app = builder.Build();

// ------------------ SWAGGER DEV ONLY ------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Static files
app.UseStaticFiles();

// Routing
app.UseRouting();

// Session
app.UseSession();

// Authentication + Authorization
app.UseAuthentication();
app.UseAuthorization();

// ------------------ ROUTING MVC ------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "gioithieu",
    pattern: "GioiThieu",
    defaults: new { controller = "GioiThieu", action = "Index" });

app.MapControllerRoute(
    name: "cuahang",
    pattern: "CuaHang",
    defaults: new { controller = "CuaHang", action = "Index" });

app.MapControllerRoute(
    name: "quanhecodong",
    pattern: "QuanHeCoDong",
    defaults: new { controller = "QuanHeCoDong", action = "Index" });

app.MapControllerRoute(
    name: "tintuc",
    pattern: "TinTuc",
    defaults: new { controller = "TinTuc", action = "Index" });

app.MapHub<ChatHub>("/chathub");

// Razor Pages + Blazor
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/admin/{*path:nonfile}", "/Admin/Index");

// ------------------ SEED DỮ LIỆU ------------------
SeedData.EnsurePopulated(app);
await IdentitySeedData.EnsurePopulated(app);

app.Run();
