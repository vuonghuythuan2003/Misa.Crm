using MISA.Core.DTOs.Requests;
using MISA.Core.DTOs.Responses;
using System;
using System.IO;

namespace MISA.Core.Interfaces.Services
{
    /// <summary>
    /// Interface service cho Customer
    /// Kế thừa CRUD cơ bản từ IBaseService (GetAll, GetById, Insert, Update, Delete, GetPaging)
    /// Thêm các nghiệp vụ riêng: GenerateCustomerCode, ImportFromCsv, ExportToCsv
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public interface ICustomerService : IBaseService<CustomerRequest, CustomerResponse>
    {
        #region Nghiệp vụ riêng

        /// <summary>
        /// Sinh mã khách hàng tự động theo định dạng KH + yyyyMM + 6 số tăng dần
        /// Ví dụ: KH202512000001
        /// </summary>
        /// <returns>Mã khách hàng mới</returns>
        string GenerateCustomerCode();

        /// <summary>
        /// Nhập khách hàng từ file CSV
        /// Các cột bắt buộc: FullName/CustomerName, Phone/CustomerPhoneNumber, Email/CustomerEmail, Address/CustomerShippingAddress/CustomerAddress, CustomerType
        /// Tự động sinh mã khách hàng (KH + yyyyMM + 6 chữ số), validate dữ liệu, kiểm tra trùng lặp (trong file và database)
        /// </summary>
        /// <param name="csvStream">Stream của file CSV</param>
        /// <returns>Kết quả nhập dữ liệu (số thành công, tổng lỗi, danh sách chi tiết lỗi theo dòng)</returns>
        ImportResponse ImportFromCsv(Stream csvStream);

        /// <summary>
        /// Xuất danh sách khách hàng ra file CSV
        /// Hỗ trợ lọc, sắp xếp thông qua PagingRequest (Keyword, CustomerName, SortColumn, SortDirection)
        /// </summary>
        /// <param name="exportRequest">Thông tin lọc và sắp xếp</param>
        /// <returns>Byte array chứa nội dung file CSV (UTF-8 with BOM)</returns>
        byte[] ExportToCsv(PagingRequest exportRequest);

        #endregion
    }
}
