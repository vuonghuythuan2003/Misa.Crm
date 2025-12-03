using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Exception
{
    /// <summary>
    /// Exception khi không tìm thấy dữ liệu
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class NotFoundException : BaseException
    {
        /// <summary>
        /// Constructor với tên entity và ID
        /// </summary>
        /// <param name="entityName">Tên entity (VD: "Khách hàng")</param>
        /// <param name="id">ID của entity</param>
        public NotFoundException(string entityName, Guid id)
            : base(MISA.Core.Exception.ErrorCode.NotFound, $"Không tìm thấy {entityName} với ID: {id}")
        {
        }

        /// <summary>
        /// Constructor với message tùy chỉnh
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        public NotFoundException(string message)
            : base(MISA.Core.Exception.ErrorCode.NotFound, message)
        {
        }
    }
}
