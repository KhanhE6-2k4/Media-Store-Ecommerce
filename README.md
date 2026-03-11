<div align="center">

# 🛒 Media Store

> E-commerce với sản phẩm là Books, CDs/LPs, DVDs — sử dụng framework ASP.NET Core 9 và kiến trúc MVC, có tích hợp cổng thanh toán VNPay, triển khai trên render, database sử dụng là Azure SQL database. Link website: https://media-store-81kx.onrender.com

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![HTML5](https://img.shields.io/badge/HTML5-E34F26?logo=html5&logoColor=white)](https://developer.mozilla.org/docs/Web/HTML)
[![CSS3](https://img.shields.io/badge/CSS3-1572B6?logo=css3&logoColor=white)](https://developer.mozilla.org/docs/Web/CSS)
[![JavaScript](https://img.shields.io/badge/JavaScript-ES6-F7DF1E?logo=javascript&logoColor=black)](https://developer.mozilla.org/docs/Web/JavaScript)
[![Azure SQL](https://img.shields.io/badge/Azure%20SQL-Database-0078D4?logo=microsoftazure)](https://azure.microsoft.com/products/azure-sql/)
[![Docker](https://img.shields.io/badge/Docker-ready-2496ED?logo=docker)](https://www.docker.com/)
[![DockerHub](https://img.shields.io/badge/DockerHub-image-2496ED?logo=docker)](https://hub.docker.com/)
[![Render](https://img.shields.io/badge/Deploy-Render-46E3B7?logo=render)](https://render.com/)
[![CI/CD](https://img.shields.io/badge/CI/CD-GitHub%20Actions-2088FF?logo=githubactions)](https://github.com/features/actions)

</div>

---

## 🛠 Tech Stack

| | |
|---|---|
| Framework | ASP.NET Core 9 MVC · C# 13 |
| UI | HTML, CSS, JS |
| Database | SQL Server + EF Core 9 . Azure SQL database |
| Payment | VNPay Gateway |
| Email | MailKit |
| Storage | Session storage |
| Auth | Cookie Authentication |
| Deploy | Docker → Render |

---

## 🚀 Getting Started

**Yêu cầu:** [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9), SQL Server


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

## 🚢 Deployment (Render + Azure)

Các xử lý đặc thù cho Render đã được cấu hình sẵn trong `Program.cs`:

- `ForwardedHeaders` đặt **đầu tiên** trong pipeline (HTTPS behind reverse proxy)
- Data Protection keys lưu vào SQL Server — không mất khi container restart
- `PORT` env var được đọc từ biến mối trường lưu trên render: `http://*:{port}`
- `db.Database.Migrate()` chạy tự động khi khởi động
- Sử dụng connection string (được cấu hình trong biến môi trường ở render) để kết nối với Azure SQL database 

---

## 📜 License

This project is licensed under the **MIT License**.

