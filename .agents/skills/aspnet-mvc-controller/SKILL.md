---
name: aspnet-mvc-controller
description: Sử dụng kỹ năng này khi cần xử lý logic backend, tạo hoặc chỉnh sửa Controllers trong ASP.NET MVC. Kỹ năng này bao gồm xử lý các luồng HTTP GET/POST, mapping dữ liệu từ Model sang ViewModel, kiểm tra ModelState (Validation), và truyền dữ liệu an toàn ra Razor View.
---

# CHUYÊN GIA ĐIỀU PHỐI ASP.NET CORE MVC (CONTROLLER LAYER)

Khi bạn được yêu cầu tạo hoặc chỉnh sửa Controller, hãy tuân thủ nghiêm ngặt các quy tắc sau:

## 1. Nguyên tắc "Skinny Controller"
- **Không chứa Business Logic:** Tuyệt đối không gọi trực tiếp `ApplicationDbContext` trong Controller. Tất cả các thao tác đọc/ghi cơ sở dữ liệu hoặc tính toán phải được gọi thông qua Interface của Service Layer (được inject qua Constructor).
- **Trách nhiệm duy nhất:** Controller chỉ làm 3 việc: Nhận HTTP Request -> Gọi Service xử lý -> Trả về HTTP Response (View, Redirect, hoặc JSON).

## 2. Quy chuẩn Routing và Http Attributes
- Sử dụng rõ ràng các attribute `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]` cho các action.
- Với các API hoặc AJAX calls, sử dụng `[Route("api/[controller]")]` và trả về `IActionResult` với các hàm như `Ok()`, `BadRequest()`, `NotFound()`.
- Cấu hình `[ValidateAntiForgeryToken]` cho TẤT CẢ các HttpPost action nhận dữ liệu từ Form để chống tấn công CSRF.

## 3. Xử lý Dữ liệu đầu vào (Validation)
- BẮT BUỘC phải kiểm tra `if (!ModelState.IsValid)` ở đầu mỗi HttpPost action. Nếu dữ liệu không hợp lệ, trả lại chính View đó kèm theo model để hiển thị lỗi.

## 4. Pattern Mẫu (Standard CRUD Controller)

```csharp
using Microsoft.AspNetCore.Mvc;

public class ProductAdminController : Controller
{
    private readonly IProductService _productService;

    // Dependency Injection
    public ProductAdminController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var products = await _productService.GetPagedProductsAsync(page, 10);
        return View(products);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Trả lại form kèm thông báo lỗi
        }

        try
        {
            await _productService.CreateProductAsync(model);
            TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
            return View(model);
        }
    }
}