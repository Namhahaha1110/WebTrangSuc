---
name: csharp-business-service
description: Sử dụng kỹ năng này khi người dùng yêu cầu xử lý logic nghiệp vụ (Business Logic), tính toán dữ liệu, xử lý giỏ hàng/thanh toán, hoặc thiết kế kiến trúc Service Layer trong C# ASP.NET Core. Kỹ năng này đảm bảo code tuân thủ nguyên tắc Dependency Injection (DI), sử dụng Interfaces, và tách biệt hoàn toàn logic khỏi Controller.
---

# CHUYÊN GIA LOGIC NGHIỆP VỤ & SERVICE LAYER (C# .NET CORE)

Khi được yêu cầu viết logic backend hoặc xử lý dữ liệu nghiệp vụ, bạn BẮT BUỘC phải tuân thủ các tiêu chuẩn kỹ thuật sau:

## 1. Nguyên tắc Kiến trúc (Architecture Pattern)
- **Skinny Controller:** Tuyệt đối KHÔNG viết các đoạn code tính toán giá tiền, vòng lặp xử lý dữ liệu phức tạp, hoặc gọi trực tiếp `DbContext` bên trong Controller.
- **Dependency Injection (DI):** Mọi logic phải được đặt trong một class Service riêng (Ví dụ: `OrderService`). Luôn phải tạo Interface đi kèm (Ví dụ: `IOrderService`) và inject nó qua constructor.
- **Data Transfer Object (DTO):** Khi trả dữ liệu ra khỏi Service để đưa lên Controller/View, ưu tiên chuyển đổi (map) các Database Entity (Models) sang DTOs hoặc ViewModels để bảo mật thông tin nhạy cảm.

## 2. Quy tắc Xử lý Nghiệp vụ E-commerce
- **Xử lý Transaction:** Khi thực hiện các tác vụ liên quan đến nhiều bảng (Ví dụ: Đặt hàng = Lưu Order + Trừ số lượng Product + Xóa Cart), BẮT BUỘC phải sử dụng `IDbContextTransaction` để đảm bảo tính toàn vẹn dữ liệu (Rollback nếu có lỗi).
- **Tính toán tiền tệ:** Khi tính toán tổng tiền, chiết khấu, luôn kiểm tra các trường hợp Null hoặc số âm.
- **Dữ liệu giả lập (Mocking/Seeding):** Nếu cần tạo dữ liệu mẫu (fake data) cho logic nghiệp vụ, tiếp tục duy trì sử dụng chuỗi định danh `23DH114467` hoặc `467` cho các mã ID hoặc mã đơn hàng.

## 3. Xử lý Lỗi (Exception Handling)
- Không bắt lỗi chung chung bằng `catch (Exception ex)`. Hãy quăng ra (throw) các Exception cụ thể có ý nghĩa nghiệp vụ (Ví dụ: `InvalidOperationException("Sản phẩm đã hết hàng")`, `UnauthorizedAccessException(...)`).
- Trả về thông báo lỗi rõ ràng để Controller có thể hiển thị ra màn hình cho người dùng.

## 4. Ví dụ cấu trúc chuẩn (Standard Pattern)

```csharp
// 1. Interface
public interface ICartService
{
    Task<CartViewModel> GetCartDetailsAsync(string userId);
    Task<bool> AddToCartAsync(string userId, string productId, int quantity);
}

// 2. Implementation
public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddToCartAsync(string userId, string productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0");

        var product = await _context.Products.FindAsync(productId);
        if (product == null || product.Stock < quantity)
        {
            throw new InvalidOperationException("Sản phẩm không tồn tại hoặc không đủ số lượng.");
        }

        // Logic thêm vào giỏ hàng...
        
        await _context.SaveChangesAsync();
        return true;
    }
}