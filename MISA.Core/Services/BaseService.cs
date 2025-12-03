using MISA.Core.DTOs.Requests;
using MISA.Core.DTOs.Responses;
using MISA.Core.Interfaces.Repositories;
using MISA.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Services
{
    /// <summary>
    /// Base service triển khai IBaseService với Request/Response DTO pattern
    /// </summary>
    /// <typeparam name="TEntity">Kiểu entity</typeparam>
    /// <typeparam name="TCreateRequest">Kiểu DTO cho tạo mới</typeparam>
    /// <typeparam name="TUpdateRequest">Kiểu DTO cho cập nhật</typeparam>
    /// <typeparam name="TResponse">Kiểu DTO trả về</typeparam>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class BaseService<TEntity, TCreateRequest, TUpdateRequest, TResponse>
        : IBaseService<TCreateRequest, TUpdateRequest, TResponse>
    {
        #region Declaration

        /// <summary>
        /// Repository để thao tác với database
        /// </summary>
        protected readonly IBaseRepository<TEntity> _baseRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo BaseService với repository
        /// </summary>
        /// <param name="baseRepository">Repository tương ứng</param>
        public BaseService(IBaseRepository<TEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        #endregion

        #region Method

        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách Response DTO</returns>
        public virtual List<TResponse> GetAll()
        {
            List<TEntity> entities = _baseRepository.GetAll();
            List<TResponse> responses = new List<TResponse>();
            foreach (TEntity entity in entities)
            {
                responses.Add(MapEntityToResponse(entity));
            }
            return responses;
        }

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Response DTO</returns>
        public virtual TResponse GetById(Guid id)
        {
            TEntity? entity = _baseRepository.GetById(id);
            if (entity == null)
            {
                throw new MISA.Core.Exception.NotFoundException(typeof(TEntity).Name, id);
            }
            return MapEntityToResponse(entity);
        }

        /// <summary>
        /// Thêm mới từ Request DTO
        /// </summary>
        /// <param name="request">DTO chứa thông tin cần thêm</param>
        /// <returns>Response DTO của bản ghi đã thêm</returns>
        public virtual TResponse Insert(TCreateRequest request)
        {
            // Validate dữ liệu trước khi thêm
            ValidateInsert(request);

            // Map từ Request DTO sang Entity
            TEntity entity = MapCreateRequestToEntity(request);

            // Thêm vào database
            TEntity insertedEntity = _baseRepository.Insert(entity);

            // Trả về Response DTO
            return MapEntityToResponse(insertedEntity);
        }

        /// <summary>
        /// Cập nhật từ Request DTO
        /// </summary>
        /// <param name="request">DTO chứa thông tin cần cập nhật</param>
        /// <returns>Response DTO của bản ghi đã cập nhật</returns>
        public virtual TResponse Update(TUpdateRequest request)
        {
            // Validate dữ liệu trước khi cập nhật
            ValidateUpdate(request);

            // Map từ Request DTO sang Entity
            TEntity entity = MapUpdateRequestToEntity(request);

            // Cập nhật vào database
            TEntity updatedEntity = _baseRepository.Update(entity);

            // Trả về Response DTO
            return MapEntityToResponse(updatedEntity);
        }

        /// <summary>
        /// Xóa mềm theo ID
        /// </summary>
        /// <param name="entityId">ID của entity cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public virtual int Delete(Guid entityId)
        {
            // Kiểm tra tồn tại trước khi xóa
            TEntity? entity = _baseRepository.GetById(entityId);
            if (entity == null)
            {
                throw new MISA.Core.Exception.NotFoundException(typeof(TEntity).Name, entityId);
            }

            return _baseRepository.SoftDelete(entityId);
        }

        /// <summary>
        /// Lấy danh sách bản ghi với phân trang và sắp xếp
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang và sắp xếp</param>
        /// <returns>Response DTO chứa danh sách và thông tin phân trang</returns>
        public virtual PagingResponse<TResponse> GetPaging(PagingRequest pagingRequest)
        {
            // Lấy dữ liệu phân trang từ repository
            var (entities, totalRecords) = _baseRepository.GetPaging(pagingRequest);
            
            // Map từ Entity sang Response DTO
            List<TResponse> responses = new List<TResponse>();
            foreach (TEntity entity in entities)
            {
                responses.Add(MapEntityToResponse(entity));
            }
            
            // Tạo response phân trang
            return new PagingResponse<TResponse>(responses, totalRecords, pagingRequest.PageNumber, pagingRequest.PageSize);
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Map từ CreateRequest DTO sang Entity (override ở lớp con)
        /// </summary>
        /// <param name="request">DTO tạo mới</param>
        /// <returns>Entity</returns>
        protected virtual TEntity MapCreateRequestToEntity(TCreateRequest request)
        {
            throw new NotImplementedException("Lớp con phải override method MapCreateRequestToEntity");
        }

        /// <summary>
        /// Map từ UpdateRequest DTO sang Entity (override ở lớp con)
        /// </summary>
        /// <param name="request">DTO cập nhật</param>
        /// <returns>Entity</returns>
        protected virtual TEntity MapUpdateRequestToEntity(TUpdateRequest request)
        {
            throw new NotImplementedException("Lớp con phải override method MapUpdateRequestToEntity");
        }

        /// <summary>
        /// Map từ Entity sang Response DTO (override ở lớp con)
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Response DTO</returns>
        protected virtual TResponse MapEntityToResponse(TEntity entity)
        {
            throw new NotImplementedException("Lớp con phải override method MapEntityToResponse");
        }

        /// <summary>
        /// Validate dữ liệu trước khi thêm mới (có thể override ở lớp con)
        /// </summary>
        /// <param name="request">DTO tạo mới</param>
        protected virtual void ValidateInsert(TCreateRequest request)
        {
            // Lớp con có thể override để thêm validation riêng
        }

        /// <summary>
        /// Validate dữ liệu trước khi cập nhật (có thể override ở lớp con)
        /// </summary>
        /// <param name="request">DTO cập nhật</param>
        protected virtual void ValidateUpdate(TUpdateRequest request)
        {
            // Lớp con có thể override để thêm validation riêng
        }

        #endregion
    }
}
