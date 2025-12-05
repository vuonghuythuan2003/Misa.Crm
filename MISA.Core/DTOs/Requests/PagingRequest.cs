using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Requests
{
    /// <summary>
    /// DTO cho request phân trang, sắp xếp, lọc và tìm kiếm
    /// Hỗ trợ: phân trang, sắp xếp, lọc theo nhiều điều kiện, tìm kiếm từ khóa
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class PagingRequest
    {
        #region Phân trang

        /// <summary>
        /// Số trang hiện tại (bắt đầu từ 1)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số bản ghi trên mỗi trang (1-100)
        /// </summary>
        [Range(1, 100, ErrorMessage = "Số bản ghi trên mỗi trang phải từ 1 đến 100")]
        public int PageSize { get; set; } = 10;

        #endregion

        #region Sắp xếp

        /// <summary>
        /// Tên cột cần sắp xếp
        /// Ví dụ: CustomerName, CustomerEmail, LastPurchaseDate
        /// </summary>
        public string? SortColumn { get; set; }

        /// <summary>
        /// Hướng sắp xếp: ASC (tăng dần) hoặc DESC (giảm dần)
        /// Mặc định: ASC
        /// </summary>
        public string SortDirection { get; set; } = "ASC";

        #endregion

        #region Tìm kiếm & Lọc

        /// <summary>
        /// Từ khóa tìm kiếm chung (tùy chọn)
        /// Sẽ tìm trên nhiều trường: tên, email, số điện thoại, mã
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Lọc theo tên khách hàng (tùy chọn, LIKE query)
        /// </summary>
        public string? CustomerName { get; set; }

        /// <summary>
        /// Lọc theo email khách hàng (tùy chọn, LIKE query)
        /// </summary>
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// Lọc theo số điện thoại khách hàng (tùy chọn, LIKE query)
        /// </summary>
        public string? CustomerPhoneNumber { get; set; }

        /// <summary>
        /// Lọc theo loại khách hàng (tùy chọn)
        /// Ví dụ: "Cá nhân", "Công ty"
        /// </summary>
        public string? CustomerType { get; set; }

        #endregion
    }
}
