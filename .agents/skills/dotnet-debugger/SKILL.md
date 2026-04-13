---
name: dotnet-debugger
description: Sử dụng kỹ năng này khi có lỗi xảy ra trong quá trình biên dịch (build errors), lỗi runtime, màn hình vàng (Yellow Screen of Death) của ASP.NET, hoặc khi người dùng yêu cầu "hãy fix lỗi này giúp tôi". Chuyên gia này rành rẽ về Entity Framework, cú pháp Razor, và vòng đời của .NET Core.
---

# CHUYÊN GIA GỠ LỖI .NET CORE & RAZOR

Khi được kích hoạt để gỡ lỗi, bạn hãy thực hiện theo quy trình sau:

## 1. Phân tích Terminal Log
- Đọc kỹ thông báo lỗi. Chú ý đến đường dẫn file và số dòng (Ví dụ: `Views/Home/Index.cshtml(15,22)`).
- Phân loại lỗi: Lỗi C# Syntax (thiếu chấm phẩy), lỗi Razor (thiếu `@`, mở thẻ HTML trong khối C# sai cách), hay lỗi Logic (Null Reference).

## 2. Các lỗi phổ biến cần check đầu tiên:
- **Lỗi thẻ đóng/mở HTML trong Razor:** Nếu dùng `@if` hoặc `@foreach`, đảm bảo HTML bên trong được bao bọc đúng cách, đôi khi cần dùng thẻ `<text>` để tránh lỗi parser.
- **Model Null:** Quên check `if (Model != null)` trước khi truy xuất dữ liệu trên View.
- **Tag Helpers:** Chưa có `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers` trong file `_ViewImports.cshtml`.

## 3. Hành động
- Giải thích ngắn gọn (1 câu) nguyên nhân gây lỗi cho người dùng.
- Thực hiện sửa code.
- CHẠY LẠI `dotnet build` để xác nhận đã hết lỗi.