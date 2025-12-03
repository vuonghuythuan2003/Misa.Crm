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
    /// </summary>
    /// <typeparam name="TCreateRequest">Kiểu DTO cho tạo mới</typeparam>
    /// <typeparam name="TUpdateRequest">Kiểu DTO cho cập nhật</typeparam>
    /// <typeparam name="TResponse">Kiểu DTO trả về</typeparam>
    /// Created by: vuonghuythuan2003 - 02/12/2024
    public interface IBaseService<TCreateRequest, TUpdateRequest, TResponse>
    {
        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách Response DTO</returns>
        List<TResponse> GetAll();

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Response DTO</returns>
        TResponse GetById(Guid id);

        /// <summary>
        /// Thêm mới từ Request DTO
        /// </summary>
        /// <param name="request">DTO chứa thông tin cần thêm</param>
        /// <returns>Response DTO của bản ghi đã thêm</returns>
        TResponse Insert(TCreateRequest request);

        /// <summary>
        /// Cập nhật từ Request DTO
        /// </summary>
        /// <param name="request">DTO chứa thông tin cần cập nhật</param>
        /// <returns>Response DTO của bản ghi đã cập nhật</returns>
        TResponse Update(TUpdateRequest request);

        /// <summary>
        /// Xóa mềm theo ID
        /// </summary>
        /// <param name="entityId">ID của entity cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        int Delete(Guid entityId);

        /// <summary>
        /// Lấy danh sách bản ghi với phân trang và sắp xếp
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang và sắp xếp</param>
        /// <returns>Response DTO chứa danh sách và thông tin phân trang</returns>
        PagingResponse<TResponse> GetPaging(PagingRequest pagingRequest);
    }
}
