# THÔNG TIN DỰ ÁN: WEB TRANG SỨC (JEWELRY E-COMMERCE)

Bạn là trợ lý AI (Agent) chính cho dự án "WebTrangSuc". Nhiệm vụ hiện tại của dự án là ĐẠI TU TOÀN BỘ GIAO DIỆN (UI/UX) để mang lại cảm giác sang trọng, tinh tế và tối ưu chuyển đổi mua hàng.

## 1. QUY TẮC ĐIỀU PHỐI KỸ NĂNG (AGENT ROUTER)
Tất cả các kỹ năng chuyên sâu của bạn được lưu trữ tại thư mục `/.agent/skills/`. Tùy thuộc vào yêu cầu của người dùng, BẮT BUỘC phải đọc và áp dụng các file `SKILL.md` tương ứng trước khi viết code:

* **Khi nhận yêu cầu về thiết kế, màu sắc, font chữ, hoặc bố cục:** Nạp kỹ năng tại `/.agent/.skills/ui-ux-pro-max/SKILL.md` (hoặc thư mục ui-ux tương ứng). Ưu tiên phong cách Minimalist (tối giản), Elegant (thanh lịch).
* **Khi nhận yêu cầu cắt HTML/CSS hoặc xây dựng component:** Nạp kỹ năng Frontend tại `/.agent/.skills/frontend-design/SKILL.md`.
* **Khi làm việc với hình ảnh sản phẩm hoặc tốc độ trang:** Nạp kỹ năng tối ưu hóa để đảm bảo ảnh không làm chậm trang web tại `/.agent/.skills/web-performance-optimization/SKILL.md`.

## 2. QUY TẮC CỐT LÕI CỦA GIAO DIỆN TRANG SỨC (UI/UX CONSTRAINTS)
Dù sử dụng skill nào, bạn phải luôn tuân thủ các nguyên tắc thiết kế sau cho dự án này:
* **Không gian trắng (White Space):** Sử dụng rộng rãi margin và padding. Sản phẩm trang sức cần không gian thở để nổi bật. Không nhồi nhét nội dung.
* **Màu sắc (Color Palette):** Hạn chế dùng các màu quá sặc sỡ (neon, primary blue/red). Ưu tiên các tone màu trung tính (trắng, đen, xám nhạt, be) kết hợp với các điểm nhấn màu kim loại (Vàng gold #D4AF37, Bạc, hoặc Rose Gold).
* **Typography:** Sử dụng các font chữ Serif (có chân) cho Tiêu đề để tạo sự cổ điển, sang trọng; và font Sans-serif (không chân) cho phần mô tả để dễ đọc.
* **Hình ảnh là trung tâm:** Mọi component giới thiệu sản phẩm phải làm nổi bật hình ảnh. Cung cấp các tỷ lệ ảnh khung hình vuông (1:1) hoặc chữ nhật dọc (4:5) chuẩn cho e-commerce.
* **Mobile-First:** Giao diện trên điện thoại phải hoàn hảo. Đảm bảo các nút bấm (Add to Cart, Checkout) to, rõ ràng và dễ bấm trên màn hình cảm ứng.

## 3. QUY TẮC LẬP TRÌNH VÀ GIAO TIẾP
* **Ngôn ngữ:** Luôn viết bình luận code (comments) và giải thích bằng Tiếng Việt.
* **Bảo vệ Code cũ:** Khi làm lại giao diện, không tự ý xóa các logic xử lý dữ liệu (JS/Backend) đã có sẵn trừ khi được yêu cầu. Chỉ tập trung vào thay đổi cấu trúc HTML/CSS/UI Component.
* **Kiểm tra từng phần:** Khi tạo xong một trang (Ví dụ: Trang chủ, Trang chi tiết sản phẩm), hãy yêu cầu tôi kiểm tra lại trước khi chuyển sang trang khác.