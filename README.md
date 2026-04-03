# WebTrangSuc - Huong dan chay du an chi tiet

Tai lieu nay huong dan chay du an tu dau tren Windows, dung voi cau truc hien tai trong workspace.

## 1) Tong quan du an

Workspace hien tai gom:
- Ung dung web chinh ASP.NET Core: [SportsSln/SportsSln/SportsStore](SportsSln/SportsSln/SportsStore)
- SQL Server Docker: [docker-compose.yml](docker-compose.yml)
- Script tao CSDL nghiep vu: [SportsStoreDB_v2.txt](SportsStoreDB_v2.txt)
- Script tao CSDL Identity: [SportsStoreIdentityDB_v2.txt](SportsStoreIdentityDB_v2.txt)

Ung dung web chinh su dung .NET 6 va chay mac dinh tai:
- http://localhost:5001

## 2) Yeu cau moi truong

Can cai dat:
- .NET SDK 6.0.428
- Docker Desktop

Kiem tra nhanh:

```powershell
dotnet --info
docker --version
docker compose version
```

## 3) Cac chuoi ket noi dang duoc dung

File cau hinh: [SportsSln/SportsSln/SportsStore/appsettings.json](SportsSln/SportsSln/SportsStore/appsettings.json)

Gia tri hien tai:
- Server: localhost,1433
- User: sa
- Password: YourStrong@Passw0rd
- DB business: SportsStoreDB_v2
- DB identity: SportsStoreIdentityDB_v2


## 4) Buoc 1 - Khoi dong SQL Server bang Docker

Chay trong thu muc goc workspace:

```powershell
cd D:\WebTrangSuc
docker compose up -d
docker ps
```

Ket qua mong doi: container sportsstore-sql co trang thai Up va map cong 1433.

Dung SQL khi can:

```powershell
docker compose down
```

## 5) Buoc 2 - Tao 2 co so du lieu bang script (Trong truong hop can xem Diagram, neu khong thi khong can lam buoc nay)

1. Mo SSMS 2018
2. Connect voi thong tin:
- Server name: localhost,1433
- Authentication: SQL Server Authentication
- Login: sa
- Password: YourStrong@Passw0rd
3. Chay lan luot 2 script:
- [SportsStoreDB_v2.txt](SportsStoreDB_v2.txt)
- [SportsStoreIdentityDB_v2.txt](SportsStoreIdentityDB_v2.txt)
4. Refresh nut Databases, dam bao thay:
- SportsStoreDB_v2
- SportsStoreIdentityDB_v2

Luu y:
- Cac script hien tai co doan DROP DATABASE neu ton tai, nen se xoa du lieu cu.
- Chi chay lai script khi ban chap nhan reset du lieu.

## 6) Buoc 3 - Restore va build

```powershell
cd D:\WebTrangSuc\SportsSln\SportsSln\SportsStore
dotnet restore
dotnet build
```

Neu ban dang o thu muc goc D:\WebTrangSuc thi restore theo solution:

```powershell
dotnet restore .\SportsSln\SportsSln\SportsSln.sln
```

## 7) Buoc 4 - Chay web

```powershell
cd D:\WebTrangSuc\SportsSln\SportsSln\SportsStore
dotnet run
```

Mo trinh duyet:
- http://localhost:5001

Dung ung dung:
- Nhan Ctrl + C tai terminal dang chay.

## 8) Tai khoan admin mau trong code

Du an co seed tai khoan admin mac dinh trong [SportsSln/SportsSln/SportsStore/Models/IdentitySeedData.cs](SportsSln/SportsSln/SportsStore/Models/IdentitySeedData.cs):
- Username: Admin
- Password: Admin123
- Email: admin@example.com


## 9) Giai thich nhanh ve 2 database

Du an tach 2 DB de de quan ly:
- SportsStoreDB_v2: du lieu nghiep vu (san pham, don hang, gio hang,...)
- SportsStoreIdentityDB_v2: du lieu dang nhap va phan quyen (AspNetUsers, AspNetRoles,...)

## 10) Loi thuong gap va cach xu ly

### Loi: MSB1003 Specify a project or solution file

Nguyen nhan:
- Chay dotnet restore o thu muc khong co .sln/.csproj.

Cach sua:
- Di chuyen vao [SportsSln/SportsSln/SportsStore](SportsSln/SportsSln/SportsStore) roi chay dotnet restore
- Hoac tro truc tiep den file solution: dotnet restore .\SportsSln\SportsSln\SportsSln.sln

### Canh bao: NU1603 EPPlus 6.0.0 not found, resolved 6.0.3

Y nghia:
- Day la canh bao, khong phai loi build.
- Du an van co the chay neu khong gap xung dot API.

Co the bo qua tam thoi, hoac cap nhat version EPPlus trong [SportsSln/SportsSln/SportsStore/SportsStore.csproj](SportsSln/SportsSln/SportsStore/SportsStore.csproj).

### Khong ket noi duoc SQL Server

Kiem tra:
- docker ps co container sportsstore-sql dang Up khong
- Cong 1433 co bi chiem boi process khac khong
- Dung dung login sa / password trong appsettings

### Khong thay database trong SSMS

Kiem tra:
- Da chay 2 file script SQL chua
- Da refresh danh sach Databases chua
- Dang connect dung server localhost,1433 chua

## 11) Danh sach lenh nhanh (copy-paste)

```powershell
cd D:\WebTrangSuc
docker compose up -d

cd D:\WebTrangSuc\SportsSln\SportsSln\SportsStore
dotnet restore
dotnet build
dotnet run
```

## 12) Ghi chu

- Bang __EFMigrationsHistory la bang he thong cua EF Core de ghi nhan migration da ap dung.
- Neu ban chay script SQL thu cong, can dam bao schema khop voi code hien tai de tranh loi runtime.
