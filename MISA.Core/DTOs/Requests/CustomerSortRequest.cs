using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Requests
{
    /// <summary>
    /// DTO cho request sắp xếp khách hàng
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class CustomerSortRequest
    {
        #region Property

        /// <summary>
        /// Tên cột cần sắp xếp
        /// </summary>
        public string? SortColumn { get; set; }

        /// <summary>
        /// Hướng sắp xếp: ASC (tăng dần) hoặc DESC (giảm dần)
        /// </summary>
        public string SortDirection { get; set; } = "ASC";

        #endregion
    }
}
