using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MISA.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MISA.Core.Services
{
    /// <summary>
    /// Service xử lý upload ảnh lên Cloudinary
    /// </summary>
    /// Created by: vuonghuythuan2003 - 04/12/2024
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudinarySettings = configuration.GetSection("Cloudinary");
            var account = new Account(
                cloudinarySettings["CloudName"],
                cloudinarySettings["ApiKey"],
                cloudinarySettings["ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Upload ảnh lên Cloudinary
        /// </summary>
        /// <param name="file">File ảnh cần upload</param>
        /// <returns>URL của ảnh sau khi upload</returns>
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File ảnh không được để trống");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "misa-crm/customers",
                        PublicId = Guid.NewGuid().ToString(),
                        Transformation = new Transformation()
                            .Width(300)
                            .Height(300)
                            .Crop("fill")
                            .Quality("auto")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        throw new System.Exception($"Lỗi upload ảnh: {uploadResult.Error.Message}");
                    }

                    return uploadResult.SecureUrl.ToString();
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Lỗi upload ảnh lên Cloudinary: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa ảnh trên Cloudinary
        /// </summary>
        /// <param name="publicId">Public ID của ảnh trên Cloudinary</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại</returns>
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return false;
            }

            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result.Result == "ok";
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi xóa ảnh: {ex.Message}");
                return false;
            }
        }
    }
}
