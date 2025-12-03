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
    /// Interface repository cho Customer
    /// </summary>
    /// Created by: vuonghuythuan2003 - 02/12/2024
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        #region Kiểm tra tồn tại

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        bool IsPhoneNumberExist(string phoneNumber, Guid? excludeCustomerId = null);

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        bool IsEmailExist(string email, Guid? excludeCustomerId = null);

        /// <summary>
        /// Kiểm tra mã khách hàng đã tồn tại chưa
        /// </summary>
        bool IsCustomerCodeExist(string customerCode);

        /// <summary>
        /// Lấy mã khách hàng lớn nhất theo tiền tố
        /// </summary>
        string? GetMaxCustomerCode(string prefix);

        #endregion

        #region Phân trang

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang</param>
        /// <returns>Tuple chứa danh sách và tổng số bản ghi</returns>
        (List<Customer> Data, int TotalRecords) GetPaging(CustomerPagingRequest pagingRequest);

        #endregion

        #region Sắp xếp

        /// <summary>
        /// Lấy danh sách khách hàng có sắp xếp
        /// </summary>
        /// <param name="sortRequest">Thông tin sắp xếp</param>
        /// <returns>Danh sách khách hàng đã sắp xếp</returns>
        List<Customer> GetSorted(CustomerSortRequest sortRequest);

        #endregion

        #region Lọc nhanh

        /// <summary>
        /// Lọc nhanh khách hàng theo tên, email, số điện thoại
        /// </summary>
        /// <param name="filterRequest">Thông tin lọc</param>
        /// <returns>Danh sách khách hàng đã lọc</returns>
        List<Customer> QuickFilter(CustomerQuickFilterRequest filterRequest);

        #endregion

        #region Xuất CSV

        /// <summary>
        /// Lấy danh sách khách hàng để xuất CSV
        /// </summary>
        /// <param name="exportRequest">Thông tin lọc và sắp xếp</param>
        /// <returns>Danh sách khách hàng</returns>
        List<Customer> GetForExport(CustomerExportRequest exportRequest);

        #endregion

        #region Nhập CSV

        /// <summary>
        /// Thêm nhiều khách hàng cùng lúc (batch insert)
        /// </summary>
        /// <param name="customers">Danh sách khách hàng cần thêm</param>
        /// <returns>Số bản ghi đã thêm thành công</returns>
        int InsertMany(List<Customer> customers);

        #endregion
    }
}
