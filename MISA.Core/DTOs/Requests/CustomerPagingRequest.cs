using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Requests
{
    /// <summary>
    /// DTO cho request phân trang khách hàng
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class CustomerPagingRequest
    {
        #region Property

        /// <summary>
        /// Số trang hiện tại (bắt đầu từ 1)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số bản ghi trên mỗi trang
        /// </summary>
        [Range(1, 100, ErrorMessage = "Số bản ghi trên mỗi trang phải từ 1 đến 100")]
        public int PageSize { get; set; } = 10;

        #endregion
    }
}
