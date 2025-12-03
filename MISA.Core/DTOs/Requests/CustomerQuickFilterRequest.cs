using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Requests
{
    /// <summary>
    /// DTO cho request lọc nhanh khách hàng (chỉ lọc, không phân trang)
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class CustomerQuickFilterRequest
    {
        #region Property

        /// <summary>
        /// Lọc theo tên khách hàng
        /// </summary>
        public string? CustomerName { get; set; }

        /// <summary>
        /// Lọc theo email khách hàng
        /// </summary>
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// Lọc theo số điện thoại khách hàng
        /// </summary>
        public string? CustomerPhoneNumber { get; set; }

        /// <summary>
        /// Từ khóa tìm kiếm chung
        /// </summary>
        public string? Keyword { get; set; }

        #endregion
    }
}
