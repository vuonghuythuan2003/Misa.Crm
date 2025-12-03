using Microsoft.AspNetCore.Http;
using MISA.Core.DTOs.Responses;
using MISA.Core.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MISA.Crm.Development.Middleware
{
    /// <summary>
    /// Middleware xử lý các exception từ toàn bộ ứng dụng
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class ExceptionMiddleware
    {
        #region Declaration

        /// <summary>
        /// Request delegate
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<ExceptionMiddleware> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo ExceptionMiddleware
        /// </summary>
        /// <param name="next">Request delegate</param>
        /// <param name="logger">Logger</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        #endregion

        #region Method

        /// <summary>
        /// Xử lý request và bắt exception
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>Task</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Xử lý exception và trả về response phù hợp
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="exception">Exception</param>
        /// <returns>Task</returns>
        private Task HandleExceptionAsync(HttpContext context, System.Exception exception)
        {
            context.Response.ContentType = "application/json";

            object response;
            int statusCode = StatusCodes.Status500InternalServerError;

            // Xử lý BaseException (custom exception)
            if (exception is BaseException baseException)
            {
                response = new
                {
                    data = (object?)null,
                    meta = (object?)null,
                    error = new
                    {
                        code = baseException.ErrorCode,
                        message = baseException.ErrorMessage,
                        details = baseException.ErrorDetails
                    }
                };

                // Xác định HTTP status code từ error code
                statusCode = GetStatusCode(baseException.ErrorCode);
            }
            // Xử lý validation error
            else if (exception is ArgumentNullException || exception is ArgumentException)
            {
                response = new
                {
                    data = (object?)null,
                    meta = (object?)null,
                    error = new
                    {
                        code = ErrorCode.ValidationError,
                        message = exception.Message,
                        details = (object?)null
                    }
                };
                statusCode = StatusCodes.Status400BadRequest;
            }
            // Xử lý các exception khác
            else
            {
                response = new
                {
                    data = (object?)null,
                    meta = (object?)null,
                    error = new
                    {
                        code = ErrorCode.InternalServerError,
                        message = "Lỗi server nội bộ. Vui lòng thử lại sau.",
                        details = (object?)null
                    }
                };
                statusCode = StatusCodes.Status500InternalServerError;
            }

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(response);
        }

        /// <summary>
        /// Lấy HTTP status code từ error code
        /// </summary>
        /// <param name="errorCode">Mã lỗi</param>
        /// <returns>HTTP status code</returns>
        private int GetStatusCode(string errorCode)
        {
            return errorCode switch
            {
                ErrorCode.ValidationError => StatusCodes.Status400BadRequest,
                ErrorCode.NotFound => StatusCodes.Status404NotFound,
                ErrorCode.DuplicateData => StatusCodes.Status400BadRequest,
                ErrorCode.DuplicateEmail => StatusCodes.Status400BadRequest,
                ErrorCode.DuplicatePhoneNumber => StatusCodes.Status400BadRequest,
                ErrorCode.DuplicateCustomerCode => StatusCodes.Status400BadRequest,
                ErrorCode.UnsupportedFileFormat => StatusCodes.Status400BadRequest,
                ErrorCode.FileSizeExceeded => StatusCodes.Status400BadRequest,
                ErrorCode.EmptyFile => StatusCodes.Status400BadRequest,
                ErrorCode.MissingRequiredColumns => StatusCodes.Status400BadRequest,
                ErrorCode.CustomerNotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        #endregion
    }
}
