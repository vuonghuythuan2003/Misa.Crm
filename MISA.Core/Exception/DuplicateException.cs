using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exception
{
    /// <summary>
    /// Exception khi phát hiện dữ liệu trùng lặp
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class DuplicateException : BaseException
    {
        /// <summary>
        /// Constructor với tên trường và giá trị trùng
        /// </summary>
        /// <param name="fieldName">Tên trường dữ liệu (VD: "Email", "Số điện thoại")</param>
        /// <param name="value">Giá trị trùng lặp</param>
        public DuplicateException(string fieldName, string value)
            : base(GetErrorCode(fieldName), $"{fieldName} '{value}' đã tồn tại trong hệ thống")
        {
        }

        /// <summary>
        /// Lấy mã lỗi phù hợp với trường dữ liệu
        /// </summary>
        /// <param name="fieldName">Tên trường dữ liệu</param>
        /// <returns>Mã lỗi tương ứng</returns>
        private static string GetErrorCode(string fieldName)
        {
            return fieldName switch
            {
                "Email" => MISA.Core.Exception.ErrorCode.DuplicateEmail,
                "Số điện thoại" => MISA.Core.Exception.ErrorCode.DuplicatePhoneNumber,
                "Mã khách hàng" => MISA.Core.Exception.ErrorCode.DuplicateCustomerCode,
                _ => MISA.Core.Exception.ErrorCode.DuplicateData
            };
        }
    }
}
