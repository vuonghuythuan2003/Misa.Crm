using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exception
{
    /// <summary>
    /// Exception khi validation dữ liệu thất bại (bao gồm trùng dữ liệu, không tồn tại, định dạng sai, etc)
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class ValidationException : BaseException
    {
        /// <summary>
        /// Constructor với message tùy chỉnh
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        public ValidationException(string message)
            : base(MISA.Core.Exception.ErrorCode.ValidationError, message)
        {
        }

        /// <summary>
        /// Constructor với tên trường và thông báo lỗi (dùng cho validation field cụ thể)
        /// </summary>
        /// <param name="fieldName">Tên trường dữ liệu</param>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="useFieldFormat">Nếu true, message sẽ được format với fieldName</param>
        public ValidationException(string fieldName, string message, bool useFieldFormat)
            : base(MISA.Core.Exception.ErrorCode.ValidationError, 
                useFieldFormat ? $"Lỗi validation cho trường '{fieldName}': {message}" : message)
        {
        }

        /// <summary>
        /// Constructor với mã lỗi tùy chỉnh và thông báo (cho các trường hợp đặc biệt như file)
        /// </summary>
        /// <param name="errorCode">Mã lỗi (VD: ErrorCode.UnsupportedFileFormat)</param>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="errorDetails">Chi tiết lỗi (nếu có)</param>
        public ValidationException(string errorCode, string message, object? errorDetails)
            : base(errorCode, message, errorDetails)
        {
        }
    }
}
