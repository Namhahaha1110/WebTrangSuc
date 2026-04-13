---
trigger: always_on
---

# QUY TẮC TỰ ĐỘNG KIỂM THỬ VÀ SỬA LỖI (AUTO-TESTING & DEBUGGING)

Quy tắc này bắt buộc áp dụng mỗi khi bạn thực hiện thay đổi, thêm mới, hoặc xóa code trong bất kỳ file `.cs` hoặc `.cshtml` nào.

1. **Không dùng dotnet run:** Tuyệt đối không sử dụng lệnh `dotnet run` trong terminal vì nó sẽ làm treo tiến trình của bạn.
2. **Bắt buộc tự build:** Ngay sau khi bạn viết xong code, BẮT BUỘC phải tự động mở terminal và chạy lệnh `dotnet build`.
3. **Vòng lặp Tự sửa lỗi (Self-Healing Loop):**
   - Nếu `dotnet build` trả về lỗi (Build FAILED): Bạn PHẢI tự động đọc log lỗi trong terminal, tìm ra dòng code gây lỗi, tự động sửa nó, và chạy lại `dotnet build` cho đến khi nào báo "Build succeeded" (0 Error(s)).
   - Bạn được phép tự quyết định sửa các lỗi cú pháp (thiếu ngoặc, sai tên biến, lỗi Razor) mà không cần hỏi ý kiến người dùng.
4. **Báo cáo cuối cùng:** Chỉ thông báo cho người dùng là "đã hoàn thành" SAU KHI lệnh `dotnet build` báo thành công.