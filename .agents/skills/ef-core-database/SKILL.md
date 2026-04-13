---
name: ef-core-database
description: Sử dụng kỹ năng này khi có yêu cầu liên quan đến Cơ sở dữ liệu (Database), thiết kế Entity Models, hoặc viết các câu truy vấn LINQ bằng Entity Framework Core. Kỹ năng này đảm bảo việc thực thi các tác vụ CRUD được tối ưu hiệu suất, tránh lỗi N+1 query, và xử lý quan hệ dữ liệu chuẩn xác.
---

# CHUYÊN GIA CƠ SỞ DỮ LIỆU & ENTITY FRAMEWORK CORE

Khi làm việc với DbContext, Models, hoặc viết các câu truy vấn LINQ, hãy áp dụng các nguyên tắc tối ưu hiệu suất sau:

## 1. Thiết kế Entity & DbContext
- Sử dụng Data Annotations (ví dụ: `[Required]`, `[MaxLength]`) hoặc Fluent API trong `OnModelCreating` để định nghĩa cấu trúc bảng chính xác.
- Đặt tên các bảng bằng danh từ số nhiều (ví dụ: `Products`, `Categories`).

## 2. Tối ưu hóa Truy vấn (LINQ Performance)
- **Tuyệt đối tránh lỗi N+1 Query:** Khi cần lấy dữ liệu từ các bảng liên kết (Foreign Keys), BẮT BUỘC phải sử dụng `.Include()` và `.ThenInclude()`. Đừng để EF Core tự động Lazy Load từng dòng trong vòng lặp.
- **Sử dụng AsNoTracking:** Đối với các truy vấn CHỈ ĐỌC (Read-only) dùng để hiển thị lên View, luôn thêm `.AsNoTracking()` vào chuỗi LINQ để giải phóng bộ nhớ, giúp web chạy nhanh hơn.
- **Chỉ Select những gì cần thiết:** Nếu View chỉ cần Tên và Giá sản phẩm, hãy dùng `.Select(p => new { p.Name, p.Price })` hoặc map thẳng ra DTO thay vì kéo toàn bộ cột trong bảng về.

## 3. Thao tác Bất đồng bộ (Async/Await)
- Tất cả các thao tác với Database phải là bất đồng bộ. Sử dụng `.ToListAsync()`, `.FirstOrDefaultAsync()`, `.SaveChangesAsync()`. Không bao giờ dùng `.ToList()` hoặc `.FirstOrDefault()` trong các hàm xử lý dữ liệu web.

## 4. Pattern Mẫu (Standard Repository/Service Query)

```csharp
using Microsoft.EntityFrameworkCore;

public async Task<PagedResult<ProductViewModel>> GetPagedProductsAsync(int pageIndex, int pageSize)
{
    var query = _context.Products
        .Include(p => p.Category)      // Giải quyết N+1 query
        .AsNoTracking()                // Tối ưu hiệu suất chỉ đọc
        .Where(p => p.IsActive);       // Bộ lọc cơ bản

    var totalRecords = await query.CountAsync();

    var items = await query
        .OrderByDescending(p => p.CreatedDate)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new ProductViewModel 
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryName = p.Category.Name
        })
        .ToListAsync();

    return new PagedResult<ProductViewModel>(items, totalRecords, pageIndex, pageSize);
}