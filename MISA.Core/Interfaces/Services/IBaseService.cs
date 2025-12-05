using MISA.Core.DTOs.Requests;
using MISA.Core.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Services
{
    /// <summary>
    /// Interface base cho các service sử dụng Request/Response DTO pattern
    /// Cung cấp các operation CRUD cơ bản và GetPaging tổng hợp (kết hợp phân trang + sắp xếp + lọc + tìm kiếm)
    /// </summary>
    /// <typeparam name="TRequest">Kiểu DTO cho tạo mới và cập nhật</typeparam>
    /// <typeparam name="TResponse">Kiểu DTO trả về</typeparam>
    /// Created by: vuonghuythuan2003 - 02/12/2024
    public interface IBaseService<TRequest, TResponse>
    {
        #region CRUD Cơ bản

        /// <summary>
        /// Lấy tất cả bản ghi (thường dùng cho Dropdown/Combobox nhỏ)
        /// </summary>
        /// <returns>Danh sách Response DTO</returns>
        List<TResponse> GetAll();

        /// <summary>
        /// Lấy chi tiết bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Response DTO hoặc exception nếu không tìm thấy</returns>
        TResponse GetById(Guid id);

        /// <summary>
        /// Thêm mới bản ghi
        /// </summary>
        /// <param name="request">DTO chứa thông tin cần thêm</param>
        /// <returns>Response DTO của bản ghi đã thêm</returns>
        TResponse Insert(TRequest request);

        /// <summary>
        /// Cập nhật bản ghi
        /// </summary>
        /// <param name="id">ID của entity cần cập nhật</param>
        /// <param name="request">DTO chứa thông tin cần cập nhật</param>
        /// <returns>Response DTO của bản ghi đã cập nhật</returns>
        TResponse Update(Guid id, TRequest request);

        /// <summary>
        /// Xóa mềm bản ghi (soft delete)
        /// </summary>
        /// <param name="id">ID của entity cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        int Delete(Guid id);

        /// <summary>
        /// Xóa mềm hàng loạt bản ghi (soft delete many)
        /// </summary>
        /// <param name="ids">Danh sách ID của entities cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        int DeleteMany(List<Guid> ids);

        #endregion

        #region Lấy danh sách tổng hợp

        /// <summary>
        /// Lấy danh sách bản ghi với hỗ trợ phân trang, sắp xếp, lọc và tìm kiếm từ khóa
        /// PagingRequest chứa:
        /// - PageNumber, PageSize: Phân trang
        /// - SortColumn, SortDirection: Sắp xếp
        /// - Keyword: Tìm kiếm từ khóa chung (tìm trên nhiều trường)
        /// - Các trường lọc khác: CustomerName, CustomerEmail, CustomerPhoneNumber, CustomerType (nếu có)
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang, sắp xếp, lọc và tìm kiếm</param>
        /// <returns>Response DTO chứa danh sách và thông tin phân trang</returns>
        PagingResponse<TResponse> GetPaging(PagingRequest pagingRequest);

        #endregion
    }
}
