using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exception
{
    /// <summary>
    /// Base exception cho toàn bộ ứng dụng
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class BaseException : System.Exception
    {
        /// <summary>
        /// Mã lỗi
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Thông báo lỗi
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Chi tiết lỗi (nếu có)
        /// </summary>
        public object? ErrorDetails { get; set; }

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public BaseException()
        {
        }

        /// <summary>
        /// Constructor với mã lỗi và thông báo
        /// </summary>
        /// <param name="errorCode">Mã lỗi</param>
        /// <param name="errorMessage">Thông báo lỗi</param>
        /// <param name="errorDetails">Chi tiết lỗi (nếu có)</param>
        public BaseException(string errorCode, string errorMessage, object? errorDetails = null)
            : base(errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ErrorDetails = errorDetails;
        }
    }
}
