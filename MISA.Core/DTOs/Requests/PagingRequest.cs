using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Requests
{
    /// <summary>
    /// DTO cho request phân trang và sắp xếp
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class PagingRequest
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

        /// <summary>
        /// Tên cột cần sắp xếp
        /// </summary>
        public string? SortColumn { get; set; }

        /// <summary>
        /// Hướng sắp xếp: ASC (tăng dần) hoặc DESC (giảm dần)
        /// </summary>
        public string SortDirection { get; set; } = "ASC";

        /// <summary>
        /// Từ khóa tìm kiếm (tùy chọn)
        /// </summary>
        public string? Keyword { get; set; }

        #endregion
    }
}
