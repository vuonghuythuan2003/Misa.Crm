
# MISA CRM - Customer Relationship Management API

Hệ thống quản lý khách hàng (CRM) xây dựng trên .NET 8, kiến trúc 3 lớp, cung cấp API RESTful cho nghiệp vụ khách hàng.

## Tính năng chính

- Thêm, sửa, xóa, lấy danh sách khách hàng
- Phân trang, sắp xếp, lọc nhanh
- Import/Export CSV
- Sinh mã khách hàng tự động
- Kiểm tra trùng email, số điện thoại
- Xử lý lỗi tập trung

## Công nghệ sử dụng

- .NET 8.0 (ASP.NET Core Web API)
- MySQL 8.0+
- Dapper, MySqlConnector
- Swashbuckle (Swagger UI)

## Cài đặt nhanh

1. Clone source về máy
2. Tạo database MySQL tên `misa_crm_development_2025` và bảng `customer` (xem file script trong thư mục docs hoặc hướng dẫn chi tiết trong tài liệu)
3. Sửa chuỗi kết nối trong `Misa.Crm.Development/appsettings.json`
4. Build & chạy:
   ```bash
   dotnet restore
   dotnet build
   cd Misa.Crm.Development
   dotnet run
   ```
5. Truy cập Swagger UI: http://localhost:5246/swagger

## API Endpoints mẫu

| Method | Endpoint           | Mô tả                  |
|--------|--------------------|------------------------|
| GET    | /api/v1/Customer   | Lấy tất cả khách hàng  |
| GET    | /api/v1/Customer/{id} | Lấy khách hàng theo ID |
| POST   | /api/v1/Customer   | Thêm khách hàng        |
| PUT    | /api/v1/Customer/{id} | Cập nhật khách hàng    |
| DELETE | /api/v1/Customer/{id} | Xóa khách hàng         |
| POST   | /api/v1/Customer/import | Import từ CSV         |
| GET    | /api/v1/Customer/export | Export ra CSV         |

## Định dạng file Import CSV

```
FullName,Phone,Email,CustomerShippingAddress,CustomerType
Nguyen Van A,0901234567,email@test.com,123 ABC Q1 HCM,Individual
```

## Cấu trúc response mẫu

```json
{
  "data": { ... },
  "meta": { ... },
  "error": null
}
```

## Liên hệ

Tác giả: vuonghuythuan2003 (12/2024)
