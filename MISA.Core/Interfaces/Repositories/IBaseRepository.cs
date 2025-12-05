using MISA.Core.DTOs.Requests;
using MISA.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface base cho các repository
    /// </summary>
    /// <typeparam name="T">Kiểu entity</typeparam>
    /// Created by: vuonghuythuan2003 - 02/12/2024
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// Lấy tất cả bản ghi (không bao gồm bản ghi đã xóa mềm)
        /// </summary>
        /// <returns>Danh sách entity</returns>
        List<T> GetAll();

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Entity tìm được hoặc null</returns>
        T? GetById(Guid id);

        /// <summary>
        /// Thêm mới entity
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Entity đã thêm</returns>
        T Insert(T entity);

        /// <summary>
        /// Cập nhật entity
        /// </summary>
        /// <param name="entity">Entity cần cập nhật</param>
        /// <returns>Entity đã cập nhật</returns>
        T Update(T entity);

        /// <summary>
        /// Xóa mềm entity theo ID (đánh dấu IsDeleted = true)
        /// </summary>
        /// <param name="entityId">ID của entity cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        int SoftDelete(Guid entityId);

        /// <summary>
        /// Xóa mềm hàng loạt entities (đánh dấu IsDeleted = true cho nhiều bản ghi)
        /// </summary>
        /// <param name="entityIds">Danh sách ID của entities cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        int SoftDeleteMany(List<Guid> entityIds);

        /// <summary>
        /// Lấy danh sách bản ghi với phân trang và sắp xếp
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang và sắp xếp</param>
        /// <returns>Tuple chứa danh sách entity và tổng số bản ghi</returns>
        (List<T> Data, int TotalRecords) GetPaging(PagingRequest pagingRequest);
    }
}
