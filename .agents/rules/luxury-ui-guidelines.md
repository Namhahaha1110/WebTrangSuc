---
trigger: always_on
---

# QUY TẮC FRONTEND & THIẾT KẾ LUXURY (LIQUID GLASS)

Quy tắc này áp dụng khi thao tác với các file `.css`, `.js` (frontend) và cấu trúc giao diện HTML.

- **Không dùng Inline CSS:** Tuyệt đối không viết CSS trực tiếp vào thẻ HTML (ví dụ: `style="..."`). Thay vào đó, sử dụng các class của Bootstrap 5 hoặc class tự định nghĩa.
- **Sử dụng CSS Tokens:** Khi cần định dạng màu sắc hoặc typography, BẮT BUỘC phải gọi các biến toàn cục (ví dụ: `var(--color-primary)`, `var(--color-accent-light)`, `var(--font-heading)`) thay vì dùng mã HEX cứng.
- **Hiệu ứng Glassmorphism:** Khi tạo các component nổi (như Navbar, Card sản phẩm, Modal), ưu tiên sử dụng hiệu ứng làm mờ nền: `backdrop-filter: blur(16px) saturate(180%);` kết hợp với `background: var(--color-surface);`.
- **Kích thước chạm (Touch Target):** Mọi nút bấm (button), icon giỏ hàng, user, icon đóng mở phải có kích thước tối thiểu `44x44px` để tối ưu cho thiết bị di động.
- **Icon:** Không sử dụng Emoji. Ưu tiên sử dụng SVG Icon mỏng (stroke-width: 1.5).