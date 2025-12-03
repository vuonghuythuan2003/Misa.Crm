using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exception
{
    /// <summary>
    /// Exception cho các lỗi trong logic business
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class BusinessException : BaseException
    {
        /// <summary>
        /// Constructor với mã lỗi và thông báo
        /// </summary>
        /// <param name="errorCode">Mã lỗi</param>
        /// <param name="errorMessage">Thông báo lỗi</param>
        /// <param name="errorDetails">Chi tiết lỗi (nếu có)</param>
        public BusinessException(string errorCode, string errorMessage, object? errorDetails = null)
            : base(errorCode, errorMessage, errorDetails)
        {
        }

        /// <summary>
        /// Constructor với thông báo tùy chỉnh
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        public BusinessException(string message)
            : base(MISA.Core.Exception.ErrorCode.InternalServerError, message)
        {
        }
    }
}
