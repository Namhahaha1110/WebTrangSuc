# WebTrangSuc - Hướng dẫn chạy dự án

## 1) Tổng quan dự án

Workspace hiện tại gồm:
- Ứng dụng web chính ASP.NET Core: [SportsSln/SportsSln/SportsStore](SportsSln/SportsSln/SportsStore)
- SQL Server Docker: [docker-compose.yml](docker-compose.yml)
- Script tạo CSDL nghiệp vụ: [SportsStoreDB_v2.txt](SportsStoreDB_v2.txt)
- Script tạo CSDL Identity: [SportsStoreIdentityDB_v2.txt](SportsStoreIdentityDB_v2.txt)

Ứng dụng web chính sử dụng .NET 6 và chạy mặc định tại:
- http://localhost:5001

## 2) Yêu cầu môi trường

Cần cài đặt:
- .NET SDK 6.0.428
- Docker Desktop

Kiểm tra nhanh:

```powershell
dotnet --info
docker --version
docker compose version
```

## 3) Các chuỗi kết nối đang được dùng

File cấu hình: [SportsSln/SportsSln/SportsStore/appsettings.json](SportsSln/SportsSln/SportsStore/appsettings.json)

Giá trị hiện tại:
- Server: localhost,1433
- User: sa
- Password: YourStrong@Passw0rd
- DB business: SportsStoreDB_v2
- DB identity: SportsStoreIdentityDB_v2

## 4) Bước 1 - Khởi động SQL Server bằng Docker

Chạy trong thư mục gốc workspace:

```powershell
cd D:\WebTrangSuc
docker compose up -d
docker ps
```

Kết quả mong đợi: container sportsstore-sql có trạng thái Up và map cổng 1433.

Dừng SQL khi cần:

```powershell
docker compose down
```

## 5) Bước 2 - Tạo 2 cơ sở dữ liệu bằng script (Trong trường hợp cần xem Diagram, nếu không thì không cần làm bước này)

1. Mở SSMS 2018
2. Connect với thông tin:
- Server name: localhost,1433
- Authentication: SQL Server Authentication
- Login: sa
- Password: YourStrong@Passw0rd
3. Chạy lần lượt 2 script:
- [SportsStoreDB_v2.txt](SportsStoreDB_v2.txt)
- [SportsStoreIdentityDB_v2.txt](SportsStoreIdentityDB_v2.txt)
4. Refresh nút Databases, đảm bảo thấy:
- SportsStoreDB_v2
- SportsStoreIdentityDB_v2

Lưu ý:
- Các script hiện tại có đoạn DROP DATABASE nếu tồn tại, nên sẽ xóa dữ liệu cũ.
- Chỉ chạy lại script khi bạn chấp nhận reset dữ liệu.

## 6) Bước 3 - Restore và build

```powershell
cd D:\WebTrangSuc\SportsSln\SportsSln\SportsStore
dotnet restore
dotnet build
```

Nếu bạn đang ở thư mục gốc D:\WebTrangSuc thì restore theo solution:

```powershell
dotnet restore .\SportsSln\SportsSln\SportsSln.sln
```

## 7) Bước 4 - Chạy web

```powershell
cd D:\WebTrangSuc\SportsSln\SportsSln\SportsStore
dotnet run
```

Mở trình duyệt:
- http://localhost:5001

Dừng ứng dụng:
- Nhấn Ctrl + C tại terminal đang chạy.

## 8) Tài khoản admin mẫu trong code

Dự án có seed tài khoản admin mặc định trong [SportsSln/SportsSln/SportsStore/Models/IdentitySeedData.cs](SportsSln/SportsSln/SportsStore/Models/IdentitySeedData.cs):
- Username: Admin
- Password: Admin123
- Email: admin@example.com

## 9) Giải thích nhanh về 2 database

Dự án tách 2 DB để dễ quản lý:
- SportsStoreDB_v2: dữ liệu nghiệp vụ (sản phẩm, đơn hàng, giỏ hàng,...)
- SportsStoreIdentityDB_v2: dữ liệu đăng nhập và phân quyền (AspNetUsers, AspNetRoles,...)

## 10) Lỗi thường gặp và cách xử lý

### Lỗi: MSB1003 Specify a project or solution file

Nguyên nhân:
- Chạy dotnet restore ở thư mục không có .sln/.csproj.

Cách sửa:
- Di chuyển vào [SportsSln/SportsSln/SportsStore](SportsSln/SportsSln/SportsStore) rồi chạy dotnet restore
- Hoặc trỏ trực tiếp đến file solution: dotnet restore .\SportsSln\SportsSln\SportsSln.sln

### Cảnh báo: NU1603 EPPlus 6.0.0 not found, resolved 6.0.3

Ý nghĩa:
- Đây là cảnh báo, không phải lỗi build.
- Dự án vẫn có thể chạy nếu không gặp xung đột API.

Có thể bỏ qua tạm thời, hoặc cập nhật version EPPlus trong [SportsSln/SportsSln/SportsStore/SportsStore.csproj](SportsSln/SportsSln/SportsStore/SportsStore.csproj).

### Không kết nối được SQL Server

Kiểm tra:
- docker ps có container sportsstore-sql đang Up không
- Cổng 1433 có bị chiếm bởi process khác không
- Dùng đúng login sa / password trong appsettings

### Không thấy database trong SSMS

Kiểm tra:
- Đã chạy 2 file script SQL chưa
- Đã refresh danh sách Databases chưa
- Đang connect đúng server localhost,1433 chưa

## 11) Danh sách lệnh nhanh (copy-paste)

```powershell
cd D:\WebTrangSuc
docker compose up -d

cd D:\WebTrangSuc\SportsSln\SportsSln\SportsStore
dotnet restore
dotnet build
dotnet run
```

## 12) Hướng dẫn ngrok để lấy Public URL cho demo VNPAY

Mục tiêu của phần này:
- Tạo public URL map vào web local đang chạy trên cổng 5001
- Gán URL đó vào CallbackUrl để VNPAY gọi về đúng endpoint

### 12.1) Đăng ký tài khoản ngrok

1. Mở trang: https://dashboard.ngrok.com/signup
2. Đăng ký tài khoản (Google/GitHub/email đều được)
3. Sau khi đăng nhập, vào trang lấy token: https://dashboard.ngrok.com/get-started/your-authtoken
4. Lưu lại authtoken để cấu hình trên máy

### 12.2) Cài ngrok trên macOS

Có 2 cách phổ biến:

Cách A - Homebrew:

```bash
brew install ngrok/ngrok/ngrok
ngrok version
```

Cách B - Tải file zip từ trang chủ:

1. Vào https://ngrok.com/download
2. Tải bản macOS phù hợp (Apple Silicon hoặc Intel)
3. Giải nén và đặt file ngrok vào PATH
4. Kiểm tra:

```bash
ngrok version
```

### 12.3) Cài ngrok trên Windows

Có 2 cách phổ biến:

Cách A - Winget:

```powershell
winget install ngrok.ngrok
ngrok version
```

Cách B - Tải file zip:

1. Vào https://ngrok.com/download
2. Tải bản Windows, giải nén
3. Mở terminal tại thư mục chứa ngrok.exe (hoặc thêm vào PATH)
4. Kiểm tra:

```powershell
ngrok version
```

### 12.4) Cấu hình authtoken trên máy (macOS và Windows)

1. Truy cập vào dashboard.ngrok.com và đăng nhập bằng tài khoản bạn đã tạo.
2. Ở menu bên trái, chọn Getting Started > Your Authtoken.
3. Bạn sẽ thấy một đoạn mã rất dài. Copy đoạn mã đó.
4. Mở Terminal và gõ lệnh sau (nhớ thay bằng mã thật của bạn):

```bash
ngrok config add-authtoken YOUR_NGROK_AUTHTOKEN
```

Kiểm tra file config đã tạo:
- macOS: ~/.config/ngrok/ngrok.yml
- Windows: %USERPROFILE%\\AppData\\Local\\ngrok\\ngrok.yml

### 12.5) Mở tunnel cho web local

Đảm bảo web đang chạy trước:

```powershell
cd D:\WebTrangSuc\SportsSln\SportsSln\SportsStore
dotnet run
```

Sau đó mở terminal khác và chạy ngrok:

```bash
ngrok http http://localhost:5001
```

Bạn sẽ nhận được URL dạng:
- https://xxxxx.ngrok-free.dev

### 12.6) Cập nhật CallbackUrl để demo VNPAY

Cập nhật trường CallbackUrl trong:
- [SportsSln/SportsSln/SportsStore/appsettings.Development.json](SportsSln/SportsSln/SportsStore/appsettings.Development.json)
- [SportsSln/SportsSln/SportsStore/appsettings.json](SportsSln/SportsSln/SportsStore/appsettings.json)

Giá trị mẫu:

```json
"VNPAY": {
  "CallbackUrl": "https://xxxxx.ngrok-free.dev/api/payment/vnpay-return"
}
```

Sau khi đổi CallbackUrl:
1. Stop app đang chạy
2. Chạy lại dotnet run
3. Tạo đơn hàng mới và thanh toán lại (không dùng link VNPAY cũ)

### 12.7) Checklist nhanh khi test bị lỗi

- Tunnel ngrok còn sống không (không đổi domain)
- CallbackUrl trong appsettings có đúng domain ngrok hiện tại không
- Endpoint return có đuôi /api/payment/vnpay-return
- Đã restart app sau khi sửa config chưa
- Lưu ý: Khi tắt ngrok và mở lại thì phải cập nhật lại CallbackUrl mới
