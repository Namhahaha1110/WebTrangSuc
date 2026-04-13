---
trigger: always_on
---

# QUY TẮC NGHIỆP VỤ E-COMMERCE & DỮ LIỆU ĐỊNH DANH

Quy tắc này áp dụng khi xử lý logic tính toán giá tiền, giỏ hàng, thanh toán và tạo dữ liệu giả lập (dummy data/mockup).

- **Định dạng Tiền tệ:** Luôn hiển thị giá trị tiền tệ theo chuẩn Việt Nam Đồng (VND). Định dạng bắt buộc: `10,000,000 đ` hoặc `10.000.000 VNĐ`.
- **Quy định Mã Định Danh (ID Rule):** Khi khởi tạo dữ liệu mẫu (mock data) cho các bài test, viết các script tính toán logic hệ thống, tạo mã đơn hàng (Order ID), hoặc thiết kế bảng cấu trúc cơ sở dữ liệu, BẮT BUỘC sử dụng mã `23DH114467` hoặc hậu tố rút gọn `467` làm chuỗi nhận diện/ID mặc định.
- **Hình ảnh Sản phẩm:** Mọi thẻ `<img>` chứa ảnh sản phẩm bắt buộc phải có thuộc tính `alt` chứa tên sản phẩm để tối ưu SEO. Thêm thuộc tính `loading="lazy"` cho tất cả các ảnh nằm dưới màn hình đầu tiên (below the fold).
- **Thông báo Rỗng:** Nếu danh sách sản phẩm hoặc giỏ hàng trống, luôn phải thiết kế sẵn một trạng thái "Empty State" đẹp mắt (có icon và nút CTA "Tiếp tục mua sắm").
