using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Core.DTOs.Requests;
using MISA.Core.DTOs.Responses;
using MISA.Core.Exception;
using MISA.Core.Interfaces.Services;

namespace MISA.Crm.Development.Controllers
{
    /// <summary>
    /// Controller xử lý API cho Customer
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : BaseController<CustomerRequest, CustomerResponse>
    {
        #region Declaration

        /// <summary>
        /// Customer Service
        /// </summary>
        private readonly ICustomerService _customerService;

        /// <summary>
        /// Cloudinary Service
        /// </summary>
        private readonly ICloudinaryService _cloudinaryService;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo CustomerController
        /// </summary>
        /// <param name="customerService">Customer Service</param>
        /// <param name="cloudinaryService">Cloudinary Service</param>
        public CustomerController(ICustomerService customerService, ICloudinaryService cloudinaryService) : base(customerService)
        {
            _customerService = customerService;
            _cloudinaryService = cloudinaryService;
        }

        #endregion

        #region Sinh mã khách hàng

        /// <summary>
        /// Sinh mã khách hàng tự động theo định dạng KH + yyyyMM + 6 số tăng dần
        /// Ví dụ: KH202512000001
        /// </summary>
        /// <returns>Mã khách hàng mới</returns>
        [HttpGet("NewCode")]
        public IActionResult GetNewCustomerCode()
        {
            string newCode = _customerService.GenerateCustomerCode();
            return Ok(ApiResponse<string>.Success(newCode));
        }

        #endregion

        #region Nhập CSV

        /// <summary>
        /// Nhập khách hàng từ file CSV
        /// Các cột bắt buộc: FullName/CustomerName, Phone/CustomerPhoneNumber, Email/CustomerEmail, Address/CustomerAddress, CustomerType
        /// Tự động sinh mã khách hàng (KH + yyyyMM + 6 chữ số), validate dữ liệu, kiểm tra trùng lặp
        /// </summary>
        /// <param name="file">File CSV chứa dữ liệu khách hàng (tối đa 5MB)</param>
        /// <returns>Kết quả nhập dữ liệu (số thành công, tổng lỗi, danh sách chi tiết lỗi theo dòng)</returns>
        [HttpPost("import")]
        public IActionResult ImportFromCsv(IFormFile file)
        {
            // Kiểm tra file
            if (file == null || file.Length == 0)
            {
                throw new ValidationException("file", "Vui lòng chọn file để tải lên.", true);
            }

            // Kiểm tra định dạng file
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension != ".csv")
            {
                throw new ValidationException(ErrorCode.UnsupportedFileFormat, "Chỉ hỗ trợ file định dạng CSV.", null);
            }

            // Kiểm tra kích thước file (giới hạn 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                throw new ValidationException(ErrorCode.FileSizeExceeded, "Kích thước file không được vượt quá 5MB.", null);
            }

            // Đọc và xử lý file
            using (Stream stream = file.OpenReadStream())
            {
                ImportResponse result = _customerService.ImportFromCsv(stream);
                return Ok(ApiResponse<ImportResponse>.Success(result));
            }
        }

        #endregion

        #region Xuất CSV

        /// <summary>
        /// Xuất danh sách khách hàng ra file CSV (có lọc và sắp xếp)
        /// </summary>
        /// <param name="exportRequest">Thông tin lọc và sắp xếp</param>
        /// <returns>File CSV</returns>
        [HttpGet("export")]
        public IActionResult ExportToCsv([FromQuery] PagingRequest exportRequest)
        {
            byte[] csvBytes = _customerService.ExportToCsv(exportRequest);
            string fileName = $"DanhSachKhachHang_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvBytes, "text/csv; charset=utf-8", fileName);
        }

        #endregion

        #region Tạo và cập nhật khách hàng

        /// <summary>
        /// Tạo mới khách hàng với upload ảnh đại diện
        /// </summary>
        /// <param name="file">File ảnh (optional)</param>
        /// <param name="request">Dữ liệu khách hàng (form-data)</param>
        /// <returns>Khách hàng vừa tạo</returns>
        [HttpPost("with-avatar")]
        public IActionResult CreateWithAvatar(IFormFile? file, [FromForm] CustomerRequest request)
        {
            // Nếu có file ảnh, upload lên Cloudinary
            if (file != null && file.Length > 0)
            {
                try
                {
                    string avatarUrl = _cloudinaryService.UploadImageAsync(file).GetAwaiter().GetResult();
                    // Lưu URL ảnh vào request
                    request.CustomerAvatarUrl = avatarUrl;
                }
                catch (System.Exception ex)
                {
                    throw new ValidationException("file", $"Lỗi upload ảnh: {ex.Message}", true);
                }
            }

            // Gọi service tạo mới khách hàng
            CustomerResponse result = _customerService.Insert(request);
            return StatusCode(201, ApiResponse<CustomerResponse>.Success(result));
        }

        /// <summary>
        /// Cập nhật khách hàng với upload ảnh đại diện mới
        /// </summary>
        /// <param name="id">ID khách hàng</param>
        /// <param name="file">File ảnh (optional)</param>
        /// <param name="request">Dữ liệu cập nhật (form-data)</param>
        /// <returns>Khách hàng đã cập nhật</returns>
        [HttpPut("{id}/with-avatar")]
        public IActionResult UpdateWithAvatar(Guid id, IFormFile? file, [FromForm] CustomerRequest request)
        {
            // Nếu có file ảnh mới, upload lên Cloudinary
            if (file != null && file.Length > 0)
            {
                try
                {
                    string avatarUrl = _cloudinaryService.UploadImageAsync(file).GetAwaiter().GetResult();
                    // Lưu URL ảnh vào request
                    request.CustomerAvatarUrl = avatarUrl;
                }
                catch (System.Exception ex)
                {
                    throw new ValidationException("file", $"Lỗi upload ảnh: {ex.Message}", true);
                }
            }

            // Gọi service cập nhật khách hàng
            CustomerResponse result = _customerService.Update(id, request);
            return Ok(ApiResponse<CustomerResponse>.Success(result));
        }

        #endregion

        #region Xóa hàng loạt

        /// <summary>
        /// Xóa mềm hàng loạt khách hàng
        /// </summary>
        /// <param name="ids">Danh sách ID khách hàng cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        [HttpPost("delete-many")]
        public IActionResult DeleteMany([FromBody] List<Guid> ids)
        {
            // Kiểm tra danh sách IDs
            if (ids == null || ids.Count == 0)
            {
                throw new ValidationException("ids", "Vui lòng chọn ít nhất một khách hàng để xóa.", true);
            }

            int affectedRows = _customerService.DeleteMany(ids);
            return Ok(ApiResponse<object>.Success(new { deletedCount = affectedRows }));
        }

        #endregion
    }
}
