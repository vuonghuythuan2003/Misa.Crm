using MISA.Crm.Development.Middleware;

namespace MISA.Crm.Development.Extensions
{
    /// <summary>
    /// Extension methods cho Middleware
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Sử dụng Exception Middleware để xử lý các exception
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <returns>Application builder</returns>
        //  Giúp viết trong file Program.cs gọn gàng hơn khi đăng ký middleware.
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        {
            // Bản chất nó gọi hàm gốc của .NET
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
