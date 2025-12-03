using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exception
{
    /// <summary>
    /// Danh sách các mã lỗi được sử dụng trong ứng dụng
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class ErrorCode
    {
        /// <summary>
        /// Lỗi server nội bộ (1001)
        /// </summary>
        public const string InternalServerError = "1001";

        /// <summary>
        /// Dữ liệu đã tồn tại (3002)
        /// </summary>
        public const string DuplicateData = "3002";

        /// <summary>
        /// Không tìm thấy dữ liệu (3003)
        /// </summary>
        public const string NotFound = "3003";

        /// <summary>
        /// Email đã tồn tại (4001)
        /// </summary>
        public const string DuplicateEmail = "4001";

        /// <summary>
        /// Số điện thoại đã tồn tại (4002)
        /// </summary>
        public const string DuplicatePhoneNumber = "4002";

        /// <summary>
        /// Mã khách hàng đã tồn tại (4003)
        /// </summary>
        public const string DuplicateCustomerCode = "4003";

        /// <summary>
        /// Không tìm thấy khách hàng (4004)
        /// </summary>
        public const string CustomerNotFound = "4004";

        /// <summary>
        /// File không hỗ trợ (5001)
        /// </summary>
        public const string UnsupportedFileFormat = "5001";

        /// <summary>
        /// File vượt quá 5MB (5002)
        /// </summary>
        public const string FileSizeExceeded = "5002";

        /// <summary>
        /// File CSV không có dữ liệu (5003)
        /// </summary>
        public const string EmptyFile = "5003";

        /// <summary>
        /// File CSV thiếu cột bắt buộc (5004)
        /// </summary>
        public const string MissingRequiredColumns = "5004";

        /// <summary>
        /// Validation error (4000)
        /// </summary>
        public const string ValidationError = "4000";
    }
}
