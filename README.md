# 🛒 MediaStore

> E-commerce với sản phẩm là Books, CDs/LPs, DVDs — sử dụng framework ASP.NET Core 9 và kiến trúc MVC, có tích hợp cổng thanh toán VNPay, triển khai trên render, database sử dụng là Azure SQL database. Link deploy: 

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-ready-2496ED?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)

---

## 🛠 Tech Stack

| | |
|---|---|
| Framework | ASP.NET Core 9 MVC · C# 13 |
| Database | SQL Server + EF Core 9 |
| Payment | VNPay Gateway |
| PDF / Email | QuestPDF · MailKit |
| Auth | Cookie Authentication |
| Deploy | Docker → Render.com |

---

## 🚀 Getting Started

**Yêu cầu:** [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9), SQL Server

```bash
git clone https://github.com/<your-org>/ISD.VN.20242-14.git
cd ISD.VN.20242-14/MediaStore
```

Chạy ứng dụng:

```bash
dotnet ef database update   # áp migration
dotnet run                  # https://localhost:5001
```

**Docker:**

```bash
docker build -t mediastore:latest .
docker run -p 8080:8080 \
  -e ConnectionStrings__Aims="..." \
  -e VnPay__TmnCode="..." \
  -e VnPay__HashSecret="..." \
  mediastore:latest
```

---

## 📁 Cấu trúc thư mục

```
MediaStore/
├── Areas/
│   └── Admin/                  # Khu vực quản trị (Cập nhật trong tương lai)
├── Controllers/                # Controllers xử lý yêu cầu
├── Data/                       # Entity classes + AimsContext (EF Core)
├── Exception/                  # Custom exception types
├── Helpers/                    # VNPay library, SessionExtensions, MySetting (chứa các session keys)
├── Migrations/                 # Migration EF Core
├── Models/                     # Model dùng trong Views
├── Services/
│   ├── Email/                  # Gửi email về đơn hàng đã mua cho người dùng
│   ├── Invoices/               # Tạo, lưu hóa đơn
│   ├── Login/                  # 
│   ├── Order/                  # Tạo & quản lý đơn hàng
│   ├── Payment/                # Interface cho các phương thức thanh toán (hiện tại có vnpay)
│   ├── Province/               # Dữ liệu tỉnh/thành cho vận chuyển
│   ├── Session/                # Quản lý giỏ hàng qua Session
│   ├── Shipping/               # Tính phí vận chuyển
│   └── Transaction/            # Tạo, lưu giao dịch thanh toán
├── Subsystem/Payment/          # Factory pattern để sau này có thêm nhiều phương thức thanh toán khác mà không cần sửa code
├── Utilities/                  # PaginatedList helper
├── ViewComponents/             # Razor View Components để tái sử dụng
├── ViewModels/                 # DTO truyền dữ liệu vào Views
├── Views/                      # Razor View cho các Action trong Controller (trong đó có Shared là đặc biệt - templates cho các trang)
├── wwwroot/                    # Static assets (CSS, JS, images)
├── appsettings.json            # Config chung 
├── Dockerfile                  # Multi-stage Docker build
├── Program.cs                  # Đăng ký DI 
└── MediaStore.csproj           # Project file & NuGet packages
```

---

## ⚙️ Cấu hình

| File | Mô tả |
|---|---|
| `appsettings.json` | Config chung, không chứa secret |
| `appsettings.Development.json` | DB, SMTP, VNPay credentials | 

---

## 🚢 Deployment (Render.com)

Các xử lý đặc thù cho Render đã được cấu hình sẵn trong `Program.cs`:

- `ForwardedHeaders` đặt **đầu tiên** trong pipeline (HTTPS behind reverse proxy)
- Data Protection keys lưu vào SQL Server — không mất khi container restart
- `PORT` env var được đọc từ biến mối trường lưu trên render: `http://*:{port}`
- `db.Database.Migrate()` chạy tự động khi khởi động
- Sử dụng connection string (được cấu hình trong biến môi trường ở render) để kết nối với Azure SQL database 

---


