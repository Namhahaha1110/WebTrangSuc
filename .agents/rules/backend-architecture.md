---
trigger: always_on
---

# QUY TẮC KIẾN TRÚC BACKEND (C# ASP.NET CORE)

Quy tắc này là ranh giới kỹ thuật tối cao cho mọi file C# bạn tạo ra hoặc chỉnh sửa.

1. **Nguyên tắc "Fat Service, Skinny Controller":**
   - MỌI logic tính toán, gọi Database (Entity Framework), xử lý giỏ hàng/thanh toán PHẢI nằm trong file Service (ví dụ: `OrderService.cs`).
   - Controller CHỈ được phép nhận request HTTP, gọi Service, và trả về View/JSON.
   - Tuyệt đối không inject `ApplicationDbContext` vào Controller (trừ những controller mặc định của Identity sinh ra).

2. **Dữ liệu giả lập (Mock/Dummy Data):**
   - Khi cần tạo dữ liệu giả lập cho Database Seed hoặc viết Unit Test, BẮT BUỘC ưu tiên sử dụng mã định danh `23DH114467` hoặc dạng rút gọn `467` làm ID cho đối tượng (User ID, Order ID, Product ID).

3. **Luôn dùng Dependency Injection (DI):**
   - Không khởi tạo Service bằng từ khóa `new Service()`. Phải tạo Interface và Inject qua constructor.
   - Đừng quên tự động nhắc tôi (hoặc tự động thêm) đăng ký Service vào file `Program.cs` (ví dụ: `builder.Services.AddScoped<IOrderService, OrderService>();`).

4. **Bảo mật Admin:**
   - Bất kỳ Controller hoặc Action nào thuộc khu vực Admin BẮT BUỘC phải có attribute `[Authorize(Roles = "Admin")]`.