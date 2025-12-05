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

        #region Xuất CSV

        /// <summary>
        /// Lấy danh sách khách hàng để xuất CSV
        /// </summary>
        /// <param name="exportRequest">Thông tin lọc và sắp xếp</param>
        /// <returns>Danh sách khách hàng</returns>
        List<Customer> GetForExport(PagingRequest exportRequest);

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
