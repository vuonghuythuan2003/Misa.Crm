using MISA.Core.DTOs.Requests;
using MISA.Core.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Interfaces.Services
{
    /// <summary>
    /// Interface service cho Customer
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public interface ICustomerService : IBaseService<CustomerCreateRequest, CustomerUpdateRequest, CustomerResponse>
    {
        /// <summary>
        /// Sinh mã khách hàng tự động
        /// </summary>
        string GenerateCustomerCode();

        #region Phân trang

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang</param>
        /// <returns>Response chứa danh sách và thông tin phân trang</returns>
        PagingResponse<CustomerResponse> GetPaging(CustomerPagingRequest pagingRequest);

        #endregion

        #region Sắp xếp

        /// <summary>
        /// Lấy danh sách khách hàng có sắp xếp
        /// </summary>
        /// <param name="sortRequest">Thông tin sắp xếp</param>
        /// <returns>Danh sách khách hàng đã sắp xếp</returns>
        List<CustomerResponse> GetSorted(CustomerSortRequest sortRequest);

        #endregion

        #region Lọc nhanh

        /// <summary>
        /// Lọc nhanh khách hàng theo tên, email, số điện thoại
        /// </summary>
        /// <param name="filterRequest">Thông tin lọc</param>
        /// <returns>Danh sách khách hàng đã lọc</returns>
        List<CustomerResponse> QuickFilter(CustomerQuickFilterRequest filterRequest);

        #endregion

        #region Nhập CSV

        /// <summary>
        /// Nhập khách hàng từ file CSV
        /// </summary>
        /// <param name="csvStream">Stream của file CSV</param>
        /// <returns>Kết quả nhập dữ liệu</returns>
        ImportResponse ImportFromCsv(Stream csvStream);

        #endregion

        #region Xuất CSV

        /// <summary>
        /// Xuất danh sách khách hàng ra file CSV
        /// </summary>
        /// <param name="exportRequest">Thông tin lọc và sắp xếp</param>
        /// <returns>Byte array chứa nội dung file CSV</returns>
        byte[] ExportToCsv(CustomerExportRequest exportRequest);

        #endregion
    }
}
