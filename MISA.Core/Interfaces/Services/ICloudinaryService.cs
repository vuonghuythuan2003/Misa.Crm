using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MISA.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho dịch vụ Cloudinary upload ảnh
    /// </summary>
    /// Created by: vuonghuythuan2003 - 04/12/2024
    public interface ICloudinaryService
    {
        /// <summary>
        /// Upload ảnh lên Cloudinary
        /// </summary>
        /// <param name="file">File ảnh cần upload</param>
        /// <returns>URL của ảnh sau khi upload</returns>
        Task<string> UploadImageAsync(IFormFile file);

        /// <summary>
        /// Xóa ảnh trên Cloudinary
        /// </summary>
        /// <param name="publicId">Public ID của ảnh trên Cloudinary</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại</returns>
        Task<bool> DeleteImageAsync(string publicId);
    }
}
