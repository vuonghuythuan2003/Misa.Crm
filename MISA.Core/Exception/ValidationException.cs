using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exception
{
    /// <summary>
    /// Exception khi validation dữ liệu thất bại
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class ValidationException : BaseException
    {
        /// <summary>
        /// Constructor với tên trường và thông báo lỗi
        /// </summary>
        /// <param name="fieldName">Tên trường dữ liệu</param>
        /// <param name="message">Thông báo lỗi</param>
        public ValidationException(string fieldName, string message)
            : base(MISA.Core.Exception.ErrorCode.ValidationError, $"Lỗi validation cho trường '{fieldName}': {message}")
        {
        }

        /// <summary>
        /// Constructor với message tùy chỉnh
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        public ValidationException(string message)
            : base(MISA.Core.Exception.ErrorCode.ValidationError, message)
        {
        }
    }
}
