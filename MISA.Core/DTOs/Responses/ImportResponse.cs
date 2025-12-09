using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Responses
{
    /// <summary>
    /// DTO cho response nhập dữ liệu từ file
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class ImportResponse
    {
        #region Property

        /// <summary>
        /// Tổng số dòng dữ liệu đã được đọc từ file
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// Số dòng dữ liệu đã được nhập thành công vào hệ thống
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Số dòng dữ liệu bị lỗi và không thể nhập vào hệ thống
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// Số dòng bị lỗi (alias cho ErrorCount để frontend đọc dễ hơn)
        /// </summary>
        public int FailCount => ErrorCount;

        /// <summary>
        /// Danh sách chi tiết các dòng bị lỗi, cung cấp thông tin cụ thể về từng lỗi.
        /// </summary>
        public List<ImportErrorDetail> Errors { get; set; } = new List<ImportErrorDetail>();

        #endregion
    }

    /// <summary>
    /// Chi tiết lỗi khi nhập dữ liệu
    /// </summary>
    public class ImportErrorDetail
    {
        /// <summary>
        /// Số thứ tự của dòng bị lỗi trong file nguồn
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Dữ liệu thô của dòng bị lỗi. Điều này giúp người dùng dễ dàng xác định dòng nào trong file gốc cần sửa.
        /// </summary>
        public string RowData { get; set; }

        /// <summary>
        /// Danh sách các thông báo lỗi chi tiết cho dòng này
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
