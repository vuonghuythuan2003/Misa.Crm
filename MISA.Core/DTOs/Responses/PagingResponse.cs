using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Responses
{
    /// <summary>
    /// DTO cho response phân trang
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của item</typeparam>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class PagingResponse<T>
    {
        #region Property

        /// <summary>
        /// Danh sách dữ liệu của trang hiện tại
        /// </summary>
        public List<T> Data { get; set; }  

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Số trang hiện tại
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Số bản ghi trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Có trang trước không
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Có trang sau không
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public PagingResponse()
        {
        }

        /// <summary>
        /// Constructor với các tham số
        /// </summary>
        /// <param name="data">Danh sách dữ liệu</param>
        /// <param name="totalRecords">Tổng số bản ghi</param>
        /// <param name="pageNumber">Số trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi trên mỗi trang</param>
        public PagingResponse(List<T> data, int totalRecords, int pageNumber, int pageSize)
        {
            Data = data;
            TotalRecords = totalRecords;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        #endregion
    }
}
