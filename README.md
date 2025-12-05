# MISA CRM - Customer Relationship Management API

Hệ thống quản lý khách hàng (CRM) được xây dựng trên nền tảng .NET 8 với kiến trúc 3 tầng, cung cấp các API RESTful để quản lý thông tin khách hàng.

## 📋 Mục lục

- [Tính năng](#-tính-năng)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Cài đặt và chạy](#-cài-đặt-và-chạy)
- [API Endpoints](#-api-endpoints)
- [Cấu trúc Response](#-cấu-trúc-response)
- [Mã lỗi](#-mã-lỗi)
- [Tài liệu API (Swagger)](#-tài-liệu-api-swagger)

## ✨ Tính năng

- **CRUD Operations**: Thêm, sửa, xóa, lấy danh sách khách hàng
- **Phân trang (Pagination)**: Hỗ trợ phân trang cho danh sách khách hàng
- **Sắp xếp (Sorting)**: Sắp xếp theo các cột tùy chọn
- **Lọc nhanh (Quick Filter)**: Tìm kiếm theo tên, email, số điện thoại
- **Import CSV**: Nhập danh sách khách hàng từ file CSV
- **Export CSV**: Xuất danh sách khách hàng ra file CSV (hỗ trợ Unicode)
- **Auto-generate Code**: Tự động sinh mã khách hàng theo định dạng `KH{yyyyMM}{6 số}`
- **Validation**: Kiểm tra trùng lặp email, số điện thoại
- **Exception Handling**: Xử lý lỗi tập trung với response chuẩn

## 🛠 Công nghệ sử dụng

| Công nghệ | Phiên bản | Mục đích |
|-----------|-----------|----------|
| .NET | 8.0 | Framework chính |
| ASP.NET Core | 8.0 | Web API |
| MySQL | 8.0+ | Database |
| Dapper | 2.1.66 | Micro ORM |
| MySqlConnector | 2.5.0 | MySQL driver |
| Swashbuckle | 6.6.2 | Swagger/OpenAPI |

## 📁 Cấu trúc dự án

```
Misa.Crm/
├── MISA.Core/                          # Business Logic Layer
│   ├── DTOs/
│   │   ├── Requests/                   # Request DTOs
│   │   └── Responses/                  # Response DTOs (ApiResponse, CustomerResponse)
│   ├── Entities/
│   │   └── Customer.cs
│   ├── Enum/
│   │   └── CustomerType.cs
│   ├── Exception/                      # Custom Exceptions
│   │   ├── ErrorCode.cs
│   │   ├── BaseException.cs
│   │   ├── NotFoundException.cs
│   │   ├── ValidationException.cs
│   │   ├── DuplicateException.cs
│   │   └── BusinessException.cs
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   └── Services/
│   └── Services/
│       ├── BaseService.cs
│       └── CustomerService.cs
│
├── MISA.Infrastructure/                # Data Access Layer
│   └── Repositories/
│       ├── BaseRepository.cs
│       └── CustomerRepository.cs
│
├── Misa.Crm.Development/               # Presentation Layer (Web API)
│   ├── Controllers/
│   │   ├── BaseController.cs
│   │   └── CustomerController.cs
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs
│   ├── appsettings.json
│   └── Program.cs
│
└── README.md
```

## 💻 Yêu cầu hệ thống

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0+](https://dev.mysql.com/downloads/mysql/)
- IDE: Visual Studio 2022 / VS Code / Rider

## 🚀 Cài đặt và chạy

### 1. Clone repository

```bash
git clone <repository-url>
cd Misa.Crm
```

### 2. Tạo Database

Tạo database MySQL với tên `misa_crm_development_2025` và bảng `customer`:

```sql
CREATE DATABASE misa_crm_development_2025 
    CHARACTER SET utf8mb4 
    COLLATE utf8mb4_unicode_ci;

USE misa_crm_development_2025;

CREATE TABLE customer (
    customer_id CHAR(36) PRIMARY KEY,
    customer_type VARCHAR(20) NOT NULL,
    customer_code VARCHAR(20) NOT NULL UNIQUE,
    customer_name VARCHAR(255) NOT NULL,
    customer_phone_number VARCHAR(15) NOT NULL UNIQUE,
    customer_email VARCHAR(100) NOT NULL UNIQUE,
    customer_address VARCHAR(255) NOT NULL,
    customer_shipping_address VARCHAR(255),
    customer_tax_code VARCHAR(20),
    last_purchase_date DATETIME,
    purchased_item_code VARCHAR(100),
    purchased_item_name VARCHAR(100),
    is_deleted TINYINT(1) DEFAULT 0
);
```

### 3. Cấu hình connection string

Mở file `Misa.Crm.Development/appsettings.json` và cập nhật:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=misa_crm_development_2025;User=root;Password=YOUR_PASSWORD;"
  }
}
```

### 4. Chạy ứng dụng

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
cd Misa.Crm.Development
dotnet run
```

### 5. Truy cập

| URL | Mô tả |
|-----|-------|
| http://localhost:5246 | HTTP |
| https://localhost:7066 | HTTPS |
| http://localhost:5246/swagger | Swagger UI |

## 📡 API Endpoints

### Base URL: `/api/v1/Customer`

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/` | Lấy tất cả khách hàng |
| `GET` | `/{id}` | Lấy khách hàng theo ID |
| `GET` | `/paging` | Phân trang (pageNumber, pageSize, sortColumn, sortDirection, keyword, filters) |
| `GET` | `/NewCode` | Sinh mã khách hàng mới |
| `GET` | `/export` | Xuất file CSV (có lọc và sắp xếp) |
| `POST` | `/` | Thêm khách hàng |
| `POST` | `/import` | Nhập từ file CSV |
| `PUT` | `/{id}` | Cập nhật khách hàng |
| `DELETE` | `/{id}` | Xóa khách hàng (soft delete) |

### Import CSV Format

File CSV cần có các cột: `FullName`, `Phone`, `Email`, `Address`, `CustomerType`

```csv
FullName,Phone,Email,Address,CustomerType
Nguyễn Văn A,0901234567,email@test.com,123 ABC Q1 HCM,Individual
```

## 📦 Cấu trúc Response

### Thành công

```json
{
  "data": { ... },
  "meta": {
    "page": 1,
    "pageSize": 10,
    "total": 100,
    "totalPages": 10,
    "hasPrevious": false,
    "hasNext": true
  },
  "error": null
}
```

### Lỗi

```json
{
  "data": null,
  "meta": null,
  "error": {
    "code": "4004",
    "message": "Không tìm thấy khách hàng",
    "details": null
  }
}
```

## ❌ Mã lỗi

| Code | Mô tả |
|------|-------|
| 1001 | Lỗi server nội bộ |
| 3002 | Dữ liệu đã tồn tại |
| 3003 | Không tìm thấy dữ liệu |
| 4001 | Email đã tồn tại |
| 4002 | Số điện thoại đã tồn tại |
| 4003 | Mã khách hàng đã tồn tại |
| 4004 | Không tìm thấy khách hàng |
| 5001 | File không hỗ trợ |
| 5002 | File vượt quá 5MB |

## 📚 Tài liệu API (Swagger)

- **Swagger UI**: http://localhost:5246/swagger
- **OpenAPI JSON**: http://localhost:5246/swagger/v1/swagger.json

### Import vào Postman

1. Truy cập: http://localhost:5246/swagger/v1/swagger.json
2. Copy JSON
3. Postman → Import → Raw text → Paste → Import

## 👥 Tác giả

- **vuonghuythuan2003** - 12/2024
