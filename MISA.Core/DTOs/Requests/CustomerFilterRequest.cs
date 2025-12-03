using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Requests
{
    /// <summary>
    /// DTO cho request lọc nhanh khách hàng
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class CustomerFilterRequest : PagingRequest
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

        #endregion
    }
}
