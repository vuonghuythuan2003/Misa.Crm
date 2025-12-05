# API Test Guide - Misa CRM Customer Management

## Base URL
```
http://localhost:5000/api/v1/customer
```

---

## 1. Sinh Mã Khách Hàng

### Request
```
GET /api/v1/customer/NewCode
```

### Response (200 OK)
```json
{
  "isSuccess": true,
  "data": "KH202512000001",
  "message": "Thành công"
}
```

---

## 2. Lấy Danh Sách Khách Hàng (Phân Trang)

### Request
```
GET /api/v1/customer/customer-paging?pageNumber=1&pageSize=10
```

### Response (200 OK)
```json
{
  "isSuccess": true,
  "data": [
    {
      "customerId": "550e8400-e29b-41d4-a716-446655440000",
      "customerCode": "KH202512000001",
      "customerName": "Nguyễn Văn A",
      "customerType": 1,
      "customerPhoneNumber": "0912345678",
      "customerEmail": "nguyenvana@example.com",
      "customerAddress": "123 Đường Nguyễn Huệ, TP.HCM",
      "customerShippingAddress": "123 Đường Nguyễn Huệ, TP.HCM",
      "customerTaxCode": "0123456789",
      "lastPurchaseDate": "2025-12-04T10:30:00",
      "purchasedItemCode": "ITEM001",
      "purchasedItemName": "Sản phẩm A",
      "customerAvatarUrl": "https://res.cloudinary.com/dvmyqjxzc/image/upload/v1234567890/misa-crm/customers/avatar123.jpg",
      "isDeleted": false
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalRecords": 1,
  "message": "Thành công"
}
```

---

## 3. Lấy Chi Tiết Khách Hàng

### Request
```
GET /api/v1/customer/{id}
```

### Example
```
GET /api/v1/customer/550e8400-e29b-41d4-a716-446655440000
```

### Response (200 OK)
```json
{
  "isSuccess": true,
  "data": {
    "customerId": "550e8400-e29b-41d4-a716-446655440000",
    "customerCode": "KH202512000001",
    "customerName": "Nguyễn Văn A",
    "customerType": 1,
    "customerPhoneNumber": "0912345678",
    "customerEmail": "nguyenvana@example.com",
    "customerAddress": "123 Đường Nguyễn Huệ, TP.HCM",
    "customerShippingAddress": "123 Đường Nguyễn Huệ, TP.HCM",
    "customerTaxCode": "0123456789",
    "lastPurchaseDate": "2025-12-04T10:30:00",
    "purchasedItemCode": "ITEM001",
    "purchasedItemName": "Sản phẩm A",
    "customerAvatarUrl": "https://res.cloudinary.com/dvmyqjxzc/image/upload/v1234567890/misa-crm/customers/avatar123.jpg",
    "isDeleted": false
  },
  "message": "Thành công"
}
```

---

## 4. Tạo Khách Hàng Mới (Với Upload Ảnh)

### Request
```
POST /api/v1/customer/with-avatar
Content-Type: multipart/form-data
```

### Body (Form Data)
```
customerCode: KH202512000002
customerName: Trần Thị B
customerType: 1
customerPhoneNumber: 0987654321
customerEmail: tranthib@example.com
customerAddress: 456 Đường Lê Lợi, TP.HCM
customerShippingAddress: 456 Đường Lê Lợi, TP.HCM
customerTaxCode: 9876543210
lastPurchaseDate: 2025-12-04T14:00:00
purchasedItemCode: ITEM002
purchasedItemName: Sản phẩm B
file: [chọn file ảnh từ máy tính]
```

### Response (201 Created)
```json
{
  "isSuccess": true,
  "data": {
    "customerId": "650e8400-e29b-41d4-a716-446655440001",
    "customerCode": "KH202512000002",
    "customerName": "Trần Thị B",
    "customerType": 1,
    "customerPhoneNumber": "0987654321",
    "customerEmail": "tranthib@example.com",
    "customerAddress": "456 Đường Lê Lợi, TP.HCM",
    "customerShippingAddress": "456 Đường Lê Lợi, TP.HCM",
    "customerTaxCode": "9876543210",
    "lastPurchaseDate": "2025-12-04T14:00:00",
    "purchasedItemCode": "ITEM002",
    "purchasedItemName": "Sản phẩm B",
    "customerAvatarUrl": "https://res.cloudinary.com/dvmyqjxzc/image/upload/c_fill,h_300,w_300,q_auto/misa-crm/customers/abc123def456.jpg",
    "isDeleted": false
  },
  "message": "Thành công"
}
```

### Without Image (Optional)
Nếu không muốn upload ảnh, bỏ qua trường `file`:

```
POST /api/v1/customer/with-avatar
Content-Type: multipart/form-data

customerCode: KH202512000002
customerName: Trần Thị B
customerType: 1
customerPhoneNumber: 0987654321
customerEmail: tranthib@example.com
customerAddress: 456 Đường Lê Lợi, TP.HCM
customerShippingAddress: 456 Đường Lê Lợi, TP.HCM
customerTaxCode: 9876543210
lastPurchaseDate: 2025-12-04T14:00:00
purchasedItemCode: ITEM002
purchasedItemName: Sản phẩm B
```

---

## 5. Cập Nhật Khách Hàng (Với Upload Ảnh Mới)

### Request
```
PUT /api/v1/customer/{id}/with-avatar
Content-Type: multipart/form-data
```

### Example URL
```
PUT /api/v1/customer/550e8400-e29b-41d4-a716-446655440000/with-avatar
```

### Body (Form Data)
```
customerCode: KH202512000001
customerName: Nguyễn Văn A (Updated)
customerType: 2
customerPhoneNumber: 0912345679
customerEmail: nguyenvana.updated@example.com
customerAddress: 789 Đường Pasteur, TP.HCM
customerShippingAddress: 789 Đường Pasteur, TP.HCM
customerTaxCode: 0123456790
lastPurchaseDate: 2025-12-04T15:30:00
purchasedItemCode: ITEM003
purchasedItemName: Sản phẩm C
file: [chọn file ảnh mới từ máy tính] (Optional)
```

### Response (200 OK)
```json
{
  "isSuccess": true,
  "data": {
    "customerId": "550e8400-e29b-41d4-a716-446655440000",
    "customerCode": "KH202512000001",
    "customerName": "Nguyễn Văn A (Updated)",
    "customerType": 2,
    "customerPhoneNumber": "0912345679",
    "customerEmail": "nguyenvana.updated@example.com",
    "customerAddress": "789 Đường Pasteur, TP.HCM",
    "customerShippingAddress": "789 Đường Pasteur, TP.HCM",
    "customerTaxCode": "0123456790",
    "lastPurchaseDate": "2025-12-04T15:30:00",
    "purchasedItemCode": "ITEM003",
    "purchasedItemName": "Sản phẩm C",
    "customerAvatarUrl": "https://res.cloudinary.com/dvmyqjxzc/image/upload/c_fill,h_300,w_300,q_auto/misa-crm/customers/new_avatar789.jpg",
    "isDeleted": false
  },
  "message": "Thành công"
}
```

---

## 6. Xóa Mềm Khách Hàng

### Request
```
DELETE /api/v1/customer/{id}
```

### Example
```
DELETE /api/v1/customer/550e8400-e29b-41d4-a716-446655440000
```

### Response (200 OK)
```json
{
  "isSuccess": true,
  "data": 1,
  "message": "Thành công"
}
```

---

## 7. Nhập CSV

### Request
```
POST /api/v1/customer/import
Content-Type: multipart/form-data
```

### Form Data
```
file: [chọn file CSV từ máy tính]
```

### CSV File Format
```
CustomerCode,CustomerName,CustomerType,CustomerPhoneNumber,CustomerEmail,CustomerAddress,CustomerShippingAddress,CustomerTaxCode
KH202512000003,Phạm Văn C,1,0912111111,phamvanc@example.com,111 Đường Trần Hưng Đạo,111 Đường Trần Hưng Đạo,1111111111
KH202512000004,Hoàng Thị D,2,0912222222,hoangthid@example.com,222 Đường Bến Thành,222 Đường Bến Thành,2222222222
KH202512000005,Vũ Văn E,1,0912333333,vuvanE@example.com,333 Đường Ngô Quyền,333 Đường Ngô Quyền,3333333333
```

### Response (200 OK)
```json
{
  "isSuccess": true,
  "data": {
    "totalRecords": 3,
    "successRecords": 3,
    "failureRecords": 0,
    "errors": []
  },
  "message": "Thành công"
}
```

### Response with Errors
```json
{
  "isSuccess": true,
  "data": {
    "totalRecords": 3,
    "successRecords": 2,
    "failureRecords": 1,
    "errors": [
      {
        "rowNumber": 2,
        "columnName": "CustomerPhoneNumber",
        "errorMessage": "Số điện thoại '0912222222' đã tồn tại trong hệ thống"
      }
    ]
  },
  "message": "Thành công"
}
```

---

## 8. Xuất CSV

### Request
```
GET /api/v1/customer/export?pageNumber=1&pageSize=100
```

### Response (200 OK)
File CSV sẽ được download với tên: `DanhSachKhachHang_20251204_143000.csv`

---

## Error Response Examples

### 400 Bad Request - Validation Error
```json
{
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR",
  "errorMessage": "Dữ liệu không hợp lệ",
  "details": [
    {
      "field": "CustomerPhoneNumber",
      "message": "Số điện thoại không đúng định dạng"
    },
    {
      "field": "CustomerEmail",
      "message": "Email không hợp lệ"
    }
  ]
}
```

### 404 Not Found
```json
{
  "isSuccess": false,
  "errorCode": "NOT_FOUND",
  "errorMessage": "Khách hàng với ID '550e8400-e29b-41d4-a716-446655440000' không tồn tại"
}
```

### 500 Internal Server Error - Upload Error
```json
{
  "isSuccess": false,
  "errorCode": "UPLOAD_ERROR",
  "errorMessage": "Lỗi upload ảnh: File size exceeds limit"
}
```

---

## Validation Rules

### CustomerPhoneNumber
- Bắt buộc
- Định dạng: 10 hoặc 11 số, bắt đầu bằng 0
- Regex: `^0\d{9,10}$`
- Phải unique (không trùng)

### CustomerEmail
- Bắt buộc
- Định dạng email hợp lệ
- Phải unique (không trùng)

### CustomerCode
- Bắt buộc
- Tự động sinh khi tạo mới nếu không cung cấp
- Format: `KH + yyyyMM + 6 số tăng dần` (VD: KH202512000001)

### CustomerName
- Bắt buộc
- Tối đa 255 ký tự

### CustomerType
- Bắt buộc
- Giá trị: 1 (Khách hàng cá nhân), 2 (Khách hàng doanh nghiệp)

### CustomerAddress
- Bắt buộc
- Tối đa 500 ký tự

### CustomerAvatarUrl
- Tùy chọn
- Tối đa 500 ký tự
- Tự động được sinh từ Cloudinary khi upload file

### File Upload (Avatar)
- Định dạng: JPG, PNG, GIF, WEBP
- Kích thước: Max 5MB
- Sẽ được crop thành 300x300px trên Cloudinary

---

## Test Data Templates

### Valid Customer Data (Tạo mới)
```json
{
  "customerCode": "KH202512000006",
  "customerName": "Lý Văn F",
  "customerType": 1,
  "customerPhoneNumber": "0912444444",
  "customerEmail": "lyvanf@example.com",
  "customerAddress": "444 Đường Nguyễn Hue",
  "customerShippingAddress": "444 Đường Nguyễn Hue",
  "customerTaxCode": "4444444444",
  "lastPurchaseDate": "2025-12-04T16:00:00",
  "purchasedItemCode": "ITEM004",
  "purchasedItemName": "Sản phẩm D"
}
```

### Valid Customer Data (Cập nhật)
```json
{
  "customerCode": "KH202512000006",
  "customerName": "Lý Văn F (Updated)",
  "customerType": 2,
  "customerPhoneNumber": "0912444445",
  "customerEmail": "lyvanf.updated@example.com",
  "customerAddress": "555 Đường Lý Tự Trọng",
  "customerShippingAddress": "555 Đường Lý Tự Trọng",
  "customerTaxCode": "5555555555",
  "lastPurchaseDate": "2025-12-04T17:00:00",
  "purchasedItemCode": "ITEM005",
  "purchasedItemName": "Sản phẩm E"
}
```

---

## Testing with Postman

1. Import các endpoint trên vào Postman
2. Sử dụng form-data cho endpoint `with-avatar`
3. Sử dụng raw JSON cho các endpoint khác
4. Cloudinary credentials đã được cấu hình trong appsettings.json
5. Database connection được cấu hình với MySQL

---

## Notes

- Tất cả timestamp sử dụng format ISO 8601: `yyyy-MM-ddTHH:mm:ss`
- ID sử dụng GUID format
- Avatar URL trả về từ Cloudinary sẽ có transformation: `c_fill,h_300,w_300,q_auto`
- Xóa là soft delete (đặt IsDeleted = true), không xóa thực sự khỏi DB
- Khi update mà không gửi ảnh mới, ảnh cũ sẽ được giữ lại
