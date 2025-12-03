using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Core.DTOs.Requests;
using MISA.Core.DTOs.Responses;
using MISA.Core.Exception;
using MISA.Core.Interfaces.Services;
using System.IO;

namespace MISA.Crm.Development.Controllers
{
    /// <summary>
    /// Controller xử lý API cho Customer
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : BaseController<CustomerCreateRequest, CustomerUpdateRequest, CustomerResponse>
    {
        #region Declaration

        /// <summary>
        /// Customer Service
        /// </summary>
        private readonly ICustomerService _customerService;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo CustomerController
        /// </summary>
        /// <param name="customerService">Customer Service</param>
        public CustomerController(ICustomerService customerService) : base(customerService)
        {
            _customerService = customerService;
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

        #region Phân trang

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang (PageNumber, PageSize)</param>
        /// <returns>Danh sách khách hàng với thông tin phân trang</returns>
        [HttpGet("customer-paging")]
        public IActionResult GetCustomerPaging([FromQuery] CustomerPagingRequest pagingRequest)
        {
            PagingResponse<CustomerResponse> result = _customerService.GetPaging(pagingRequest);
            return Ok(ApiResponse<List<CustomerResponse>>.Success(
                result.Data,
                result.PageNumber,
                result.PageSize,
                result.TotalRecords));
        }

        #endregion

        #region Sắp xếp

        /// <summary>
        /// Lấy danh sách khách hàng có sắp xếp theo cột
        /// </summary>
        /// <param name="sortRequest">Thông tin sắp xếp (SortColumn, SortDirection)</param>
        /// <returns>Danh sách khách hàng đã sắp xếp</returns>
        [HttpGet("sort")]
        public IActionResult GetSorted([FromQuery] CustomerSortRequest sortRequest)
        {
            List<CustomerResponse> result = _customerService.GetSorted(sortRequest);
            return Ok(ApiResponse<List<CustomerResponse>>.Success(result));
        }

        #endregion

        #region Lọc nhanh

        /// <summary>
        /// Lọc nhanh khách hàng theo tên, email, số điện thoại
        /// </summary>
        /// <param name="filterRequest">Thông tin lọc nhanh (CustomerName, CustomerEmail, CustomerPhoneNumber, Keyword)</param>
        /// <returns>Danh sách khách hàng thỏa điều kiện lọc</returns>
        [HttpGet("quick-filter")]
        public IActionResult QuickFilter([FromQuery] CustomerQuickFilterRequest filterRequest)
        {
            List<CustomerResponse> result = _customerService.QuickFilter(filterRequest);
            return Ok(ApiResponse<List<CustomerResponse>>.Success(result));
        }

        #endregion

        #region Nhập CSV

        /// <summary>
        /// Nhập khách hàng từ file CSV
        /// </summary>
        /// <param name="file">File CSV chứa dữ liệu khách hàng</param>
        /// <returns>Kết quả nhập dữ liệu</returns>
        [HttpPost("import")]
        public IActionResult ImportFromCsv(IFormFile file)
        {
            // Kiểm tra file
            if (file == null || file.Length == 0)
            {
                throw new ValidationException("file", "Vui lòng chọn file để tải lên.");
            }

            // Kiểm tra định dạng file
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension != ".csv")
            {
                throw new BusinessException(ErrorCode.UnsupportedFileFormat, "Chỉ hỗ trợ file định dạng CSV.");
            }

            // Kiểm tra kích thước file (giới hạn 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                throw new BusinessException(ErrorCode.FileSizeExceeded, "Kích thước file không được vượt quá 5MB.");
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
        public IActionResult ExportToCsv([FromQuery] CustomerExportRequest exportRequest)
        {
            byte[] csvBytes = _customerService.ExportToCsv(exportRequest);
            string fileName = $"DanhSachKhachHang_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvBytes, "text/csv; charset=utf-8", fileName);
        }

        #endregion
    }
}
