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
        /// Tổng số dòng trong file
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// Số dòng nhập thành công
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Số dòng lỗi
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// Danh sách các dòng lỗi với thông tin chi tiết
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
        /// Số dòng trong file (bắt đầu từ 1, không tính header)
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Dữ liệu của dòng bị lỗi
        /// </summary>
        public string RowData { get; set; }

        /// <summary>
        /// Danh sách các lỗi
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
