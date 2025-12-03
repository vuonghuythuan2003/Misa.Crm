using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Responses
{
    /// <summary>
    /// Response chuẩn cho tất cả API
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class ApiResponse<T>
    {
        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Thông tin meta (phân trang, ...)
        /// </summary>
        public MetaData? Meta { get; set; }

        /// <summary>
        /// Thông tin lỗi (null nếu thành công)
        /// </summary>
        public ErrorData? Error { get; set; }

        #region Constructors

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Constructor với data
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        public ApiResponse(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Constructor với data và meta
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="meta">Thông tin meta</param>
        public ApiResponse(T data, MetaData meta)
        {
            Data = data;
            Meta = meta;
        }

        #endregion

        #region Static Factory Methods

        /// <summary>
        /// Tạo response thành công với data
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <returns>ApiResponse</returns>
        public static ApiResponse<T> Success(T data)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Error = null
            };
        }

        /// <summary>
        /// Tạo response thành công với data và phân trang
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi mỗi trang</param>
        /// <param name="total">Tổng số bản ghi</param>
        /// <returns>ApiResponse</returns>
        public static ApiResponse<T> Success(T data, int page, int pageSize, int total)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Meta = new MetaData(page, pageSize, total),
                Error = null
            };
        }

        /// <summary>
        /// Tạo response lỗi
        /// </summary>
        /// <param name="errorCode">Mã lỗi</param>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="details">Chi tiết lỗi</param>
        /// <returns>ApiResponse</returns>
        public static ApiResponse<T> Fail(string errorCode, string message, object? details = null)
        {
            return new ApiResponse<T>
            {
                Data = default,
                Error = new ErrorData(errorCode, message, details)
            };
        }

        #endregion
    }

    /// <summary>
    /// Thông tin meta (phân trang, ...)
    /// </summary>
    public class MetaData
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Số bản ghi mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;

        /// <summary>
        /// Có trang trước không
        /// </summary>
        public bool HasPrevious => Page > 1;

        /// <summary>
        /// Có trang sau không
        /// </summary>
        public bool HasNext => Page < TotalPages;

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public MetaData()
        {
        }

        /// <summary>
        /// Constructor với tham số
        /// </summary>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi mỗi trang</param>
        /// <param name="total">Tổng số bản ghi</param>
        public MetaData(int page, int pageSize, int total)
        {
            Page = page;
            PageSize = pageSize;
            Total = total;
        }
    }

    /// <summary>
    /// Thông tin lỗi
    /// </summary>
    public class ErrorData
    {
        /// <summary>
        /// Mã lỗi
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Thông báo lỗi
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Chi tiết lỗi (validation errors, stack trace, ...)
        /// </summary>
        public object? Details { get; set; }

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public ErrorData()
        {
        }

        /// <summary>
        /// Constructor với tham số
        /// </summary>
        /// <param name="code">Mã lỗi</param>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="details">Chi tiết lỗi</param>
        public ErrorData(string code, string message, object? details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }
    }
}
