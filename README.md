

# MISA CRM Backend

Đây là dự án backend của hệ thống MISA CRM, xây dựng trên .NET 8, kiến trúc 3 lớp, cung cấp API RESTful phục vụ nghiệp vụ quản lý khách hàng.

## Cấu trúc thư mục

- `MISA.Core/` - Lớp nghiệp vụ, DTO, interface
- `MISA.Infrastructure/` - Lớp truy cập dữ liệu, repository
- `Misa.Crm.Development/` - Web API, controller, cấu hình, middleware
- `docs/` - Tài liệu, script SQL

## Tính năng chính

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


## Cài đặt & chạy

1. Cài đặt .NET 8 SDK và MySQL 8
2. Clone source về máy
3. Tạo database MySQL tên `misa_crm_development_2025` và bảng `customer` (xem script trong thư mục docs)
4. Sửa chuỗi kết nối trong `Misa.Crm.Development/appsettings.json`
5. Build & chạy:
  ```bash
  dotnet restore
  dotnet build
  cd Misa.Crm.Development
  dotnet run
  ```
6. Truy cập Swagger UI: http://localhost:5246/swagger


## API Endpoints mẫu

| Method | Endpoint                  | Mô tả                    |
|--------|---------------------------|--------------------------|
| GET    | /api/v1/Customer          | Lấy tất cả khách hàng    |
| GET    | /api/v1/Customer/{id}     | Lấy khách hàng theo ID   |
| POST   | /api/v1/Customer          | Thêm khách hàng          |
| PUT    | /api/v1/Customer/{id}     | Cập nhật khách hàng      |
| DELETE | /api/v1/Customer/{id}     | Xóa khách hàng           |
| POST   | /api/v1/Customer/import   | Import từ CSV            |
| GET    | /api/v1/Customer/export   | Export ra CSV            |


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

- Người phát triển: Vương Huy Thuận
- Email: vuonghuythuan1@gmail.com
