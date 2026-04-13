---
trigger: always_on
---

# QUY TẮC CÚ PHÁP ASP.NET RAZOR & MVC

Quy tắc này áp dụng khi thao tác với các file `.cshtml` và logic C# liên quan đến View.

- **Ưu tiên Tag Helpers:** Khi tạo các liên kết hoặc form, không sử dụng thẻ HTML thuần (`href="/Controller/Action"` hoặc `<form action="...">`). BẮT BUỘC sử dụng ASP.NET Tag Helpers như `asp-controller="..."`, `asp-action="..."`, `asp-route-id="..."`.
- **Phân tách Logic:** Tuyệt đối KHÔNG viết các khối mã C# xử lý logic phức tạp (truy vấn DB, tính toán thuật toán) trực tiếp trong file `.cshtml` bằng `@ { ... }`. Mọi logic phải được xử lý ở Controller và truyền sang View thông qua `ViewModel` hoặc `ViewBag/ViewData`.
- **Partial Views:** Nếu một khối giao diện (như Product Card, Sidebar, Pagination) được lặp lại nhiều lần, hãy chủ động đề xuất tách nó ra thành file `_PartialView.cshtml` riêng biệt.
- **Bảo mật Form:** Mọi thẻ `<form>` có method POST mặc định phải có (hoặc tự động sinh) AntiForgeryToken. Không tự ý tắt tính năng này.