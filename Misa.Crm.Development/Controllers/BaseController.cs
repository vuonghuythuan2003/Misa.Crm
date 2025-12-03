using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Core.DTOs.Requests;
using MISA.Core.DTOs.Responses;
using MISA.Core.Exception;
using MISA.Core.Interfaces.Services;

namespace MISA.Crm.Development.Controllers
{
    /// <summary>
    /// Base Controller cho các API CRUD
    /// </summary>
    /// <typeparam name="TCreateRequest">Kiểu DTO cho tạo mới</typeparam>
    /// <typeparam name="TUpdateRequest">Kiểu DTO cho cập nhật</typeparam>
    /// <typeparam name="TResponse">Kiểu DTO trả về</typeparam>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    [ApiController]
    public class BaseController<TCreateRequest, TUpdateRequest, TResponse> : ControllerBase
    {
        #region Declaration

        /// <summary>
        /// Service xử lý nghiệp vụ
        /// </summary>
        protected readonly IBaseService<TCreateRequest, TUpdateRequest, TResponse> _baseService;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo BaseController với service
        /// </summary>
        /// <param name="baseService">Service tương ứng</param>
        public BaseController(IBaseService<TCreateRequest, TUpdateRequest, TResponse> baseService)
        {
            _baseService = baseService;
        }

        #endregion

        #region Method

        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách Response DTO</returns>
        [HttpGet]
        public virtual IActionResult GetAll()
        {
            List<TResponse> result = _baseService.GetAll();
            return Ok(ApiResponse<List<TResponse>>.Success(result));
        }

        /// <summary>
        /// Lấy danh sách bản ghi với phân trang và sắp xếp
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang và sắp xếp</param>
        /// <returns>Danh sách Response DTO với thông tin phân trang</returns>
        [HttpGet("paging")]
        public virtual IActionResult GetPaging([FromQuery] PagingRequest pagingRequest)
        {
            PagingResponse<TResponse> result = _baseService.GetPaging(pagingRequest);
            return Ok(ApiResponse<List<TResponse>>.Success(
                result.Data, 
                result.PageNumber, 
                result.PageSize, 
                result.TotalRecords));
        }

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Response DTO</returns>
        [HttpGet("{id}")]
        public virtual IActionResult GetById(Guid id)
        {
            TResponse result = _baseService.GetById(id);
            return Ok(ApiResponse<TResponse>.Success(result));
        }

        /// <summary>
        /// Thêm mới bản ghi
        /// </summary>
        /// <param name="request">DTO chứa thông tin cần thêm</param>
        /// <returns>Response DTO của bản ghi đã thêm</returns>
        [HttpPost]
        public virtual IActionResult Insert([FromBody] TCreateRequest request)
        {
            TResponse result = _baseService.Insert(request);
            return StatusCode(201, ApiResponse<TResponse>.Success(result));
        }

        /// <summary>
        /// Cập nhật bản ghi
        /// </summary>
        /// <param name="request">DTO chứa thông tin cần cập nhật</param>
        /// <returns>Response DTO của bản ghi đã cập nhật</returns>
        [HttpPut]
        public virtual IActionResult Update([FromBody] TUpdateRequest request)
        {
            TResponse result = _baseService.Update(request);
            return Ok(ApiResponse<TResponse>.Success(result));
        }

        /// <summary>
        /// Xóa mềm bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của entity cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        [HttpDelete("{id}")]
        public virtual IActionResult Delete(Guid id)
        {
            int result = _baseService.Delete(id);
            return Ok(ApiResponse<int>.Success(result));
        }

        #endregion
    }
}
