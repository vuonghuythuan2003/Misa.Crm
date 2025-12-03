using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.Core.DTOs.Requests;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Infrastructure.Repositories
{
    /// <summary>
    /// Repository xử lý dữ liệu Customer
    /// </summary>
    /// Created by: vuonghuythuan2003 - 02/12/2024
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        #region Constructor

        /// <summary>
        /// Khởi tạo CustomerRepository
        /// </summary>
        /// <param name="config">Cấu hình ứng dụng</param>
        public CustomerRepository(IConfiguration config) : base(config)
        {
        }

        #endregion

        #region Kiểm tra trùng lặp

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần kiểm tra</param>
        /// <param name="excludeCustomerId">ID khách hàng cần loại trừ (dùng khi update)</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        public bool IsPhoneNumberExist(string phoneNumber, Guid? excludeCustomerId = null)
        {
            string sqlCommand = "SELECT COUNT(*) FROM customer WHERE customer_phone_number = @PhoneNumber AND is_deleted = 0";
            
            if (excludeCustomerId.HasValue)
            {
                sqlCommand += " AND customer_id != @ExcludeId";
                return dbConnection.ExecuteScalar<int>(sqlCommand, new { PhoneNumber = phoneNumber, ExcludeId = excludeCustomerId.Value }) > 0;
            }
            
            return dbConnection.ExecuteScalar<int>(sqlCommand, new { PhoneNumber = phoneNumber }) > 0;
        }

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="excludeCustomerId">ID khách hàng cần loại trừ (dùng khi update)</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        public bool IsEmailExist(string email, Guid? excludeCustomerId = null)
        {
            string sqlCommand = "SELECT COUNT(*) FROM customer WHERE customer_email = @Email AND is_deleted = 0";
            
            if (excludeCustomerId.HasValue)
            {
                sqlCommand += " AND customer_id != @ExcludeId";
                return dbConnection.ExecuteScalar<int>(sqlCommand, new { Email = email, ExcludeId = excludeCustomerId.Value }) > 0;
            }
            
            return dbConnection.ExecuteScalar<int>(sqlCommand, new { Email = email }) > 0;
        }

        /// <summary>
        /// Kiểm tra mã khách hàng đã tồn tại chưa
        /// </summary>
        /// <param name="customerCode">Mã khách hàng cần kiểm tra</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        public bool IsCustomerCodeExist(string customerCode)
        {
            string sqlCommand = "SELECT COUNT(*) FROM customer WHERE customer_code = @CustomerCode AND is_deleted = 0";
            return dbConnection.ExecuteScalar<int>(sqlCommand, new { CustomerCode = customerCode }) > 0;
        }
    
        /// <summary>
        /// Lấy mã khách hàng lớn nhất theo tiền tố (VD: lấy max của KH202512...)
        /// </summary>
        /// <param name="prefix">Tiền tố mã khách hàng (VD: KH202512)</param>
        /// <returns>Mã khách hàng lớn nhất hoặc null nếu chưa có</returns>
        public string? GetMaxCustomerCode(string prefix)
        {
            string sqlCommand = @"SELECT customer_code 
                                  FROM customer 
                                  WHERE customer_code LIKE @Prefix 
                                  AND is_deleted = 0 
                                  ORDER BY customer_code DESC 
                                  LIMIT 1";
            
            return dbConnection.QueryFirstOrDefault<string?>(sqlCommand, new { Prefix = prefix + "%" });
        }

        #endregion

        #region Phân trang

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang</param>
        /// <returns>Tuple chứa danh sách khách hàng và tổng số bản ghi</returns>
        public (List<Customer> Data, int TotalRecords) GetPaging(CustomerPagingRequest pagingRequest)
        {
            DynamicParameters parameters = new DynamicParameters();

            // Tính offset cho phân trang
            int offset = (pagingRequest.PageNumber - 1) * pagingRequest.PageSize;
            parameters.Add("PageSize", pagingRequest.PageSize);
            parameters.Add("Offset", offset);

            // Query lấy dữ liệu phân trang
            string sqlData = @"SELECT * FROM customer 
                               WHERE is_deleted = 0 
                               ORDER BY customer_id DESC 
                               LIMIT @PageSize OFFSET @Offset";

            // Query đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM customer WHERE is_deleted = 0";

            List<Customer> data = dbConnection.Query<Customer>(sqlData, parameters).ToList();
            int totalRecords = dbConnection.ExecuteScalar<int>(sqlCount, parameters);

            return (data, totalRecords);
        }

        #endregion

        #region Sắp xếp

        /// <summary>
        /// Lấy danh sách khách hàng có sắp xếp
        /// </summary>
        /// <param name="sortRequest">Thông tin sắp xếp</param>
        /// <returns>Danh sách khách hàng đã sắp xếp</returns>
        public List<Customer> GetSorted(CustomerSortRequest sortRequest)
        {
            // Xây dựng ORDER BY
            string orderByClause = BuildOrderByClause(sortRequest.SortColumn ?? "", sortRequest.SortDirection);

            // Query lấy tất cả dữ liệu đã sắp xếp
            string sqlData = $@"SELECT * FROM customer 
                               WHERE is_deleted = 0 
                               {orderByClause}";

            return dbConnection.Query<Customer>(sqlData).ToList();
        }

        #endregion

        #region Lọc nhanh

        /// <summary>
        /// Lọc nhanh khách hàng theo tên, email, số điện thoại
        /// </summary>
        /// <param name="filterRequest">Thông tin lọc nhanh</param>
        /// <returns>Danh sách khách hàng thỏa điều kiện lọc</returns>
        public List<Customer> QuickFilter(CustomerQuickFilterRequest filterRequest)
        {
            DynamicParameters parameters = new DynamicParameters();
            
            // Xây dựng WHERE clause
            string whereClause = BuildWhereClause(filterRequest, parameters);

            // Query lấy dữ liệu
            string sqlData = $@"SELECT * FROM customer 
                               {whereClause} 
                               ORDER BY customer_id DESC";

            return dbConnection.Query<Customer>(sqlData, parameters).ToList();
        }

        #endregion

        #region Xuất CSV

        /// <summary>
        /// Lấy danh sách khách hàng để xuất CSV (có lọc và sắp xếp)
        /// </summary>
        /// <param name="exportRequest">Thông tin lọc và sắp xếp</param>
        /// <returns>Danh sách khách hàng</returns>
        public List<Customer> GetForExport(CustomerExportRequest exportRequest)
        {
            DynamicParameters parameters = new DynamicParameters();
            
            // Xây dựng WHERE clause từ filter
            string whereClause = BuildWhereClauseForExport(exportRequest, parameters);
            
            // Xây dựng ORDER BY clause
            string orderByClause = BuildOrderByClause(exportRequest.SortColumn ?? "", exportRequest.SortDirection);

            // Query lấy tất cả dữ liệu (không phân trang)
            string sqlData = $@"SELECT * FROM customer 
                               {whereClause} 
                               {orderByClause}";

            return dbConnection.Query<Customer>(sqlData, parameters).ToList();
        }

        #endregion

        #region Nhập CSV

        /// <summary>
        /// Thêm nhiều khách hàng cùng lúc (batch insert)
        /// </summary>
        /// <param name="customers">Danh sách khách hàng cần thêm</param>
        /// <returns>Số bản ghi đã thêm thành công</returns>
        public int InsertMany(List<Customer> customers)
        {
            if (customers == null || customers.Count == 0)
            {
                return 0;
            }

            string sqlCommand = @"INSERT INTO customer 
                (customer_id, customer_type, customer_code, customer_name, customer_phone_number, 
                 customer_email, customer_address, customer_shipping_address, customer_tax_code, 
                 last_purchase_date, purchased_item_code, purchased_item_name, is_deleted)
                VALUES 
                (@CustomerId, @CustomerType, @CustomerCode, @CustomerName, @CustomerPhoneNumber, 
                 @CustomerEmail, @CustomerAddress, @CustomerShippingAddress, @CustomerTaxCode, 
                 @LastPurchaseDate, @PurchasedItemCode, @PurchasedItemName, @IsDeleted)";

            return dbConnection.Execute(sqlCommand, customers);
        }

        #endregion

        #region Private Method - Build Clause

        /// <summary>
        /// Xây dựng điều kiện WHERE cho lọc nhanh
        /// </summary>
        /// <param name="filterRequest">Thông tin lọc nhanh</param>
        /// <param name="parameters">DynamicParameters để thêm tham số</param>
        /// <returns>Chuỗi WHERE clause</returns>
        private string BuildWhereClause(CustomerQuickFilterRequest filterRequest, DynamicParameters parameters)
        {
            StringBuilder whereClause = new StringBuilder("WHERE is_deleted = 0");

            // Lọc theo tên khách hàng
            if (!string.IsNullOrWhiteSpace(filterRequest.CustomerName))
            {
                whereClause.Append(" AND customer_name LIKE @CustomerName");
                parameters.Add("CustomerName", $"%{filterRequest.CustomerName}%");
            }

            // Lọc theo email
            if (!string.IsNullOrWhiteSpace(filterRequest.CustomerEmail))
            {
                whereClause.Append(" AND customer_email LIKE @CustomerEmail");
                parameters.Add("CustomerEmail", $"%{filterRequest.CustomerEmail}%");
            }

            // Lọc theo số điện thoại
            if (!string.IsNullOrWhiteSpace(filterRequest.CustomerPhoneNumber))
            {
                whereClause.Append(" AND customer_phone_number LIKE @CustomerPhoneNumber");
                parameters.Add("CustomerPhoneNumber", $"%{filterRequest.CustomerPhoneNumber}%");
            }

            // Lọc theo keyword chung (tìm trên nhiều trường)
            if (!string.IsNullOrWhiteSpace(filterRequest.Keyword))
            {
                whereClause.Append(" AND (customer_name LIKE @Keyword OR customer_email LIKE @Keyword OR customer_phone_number LIKE @Keyword OR customer_code LIKE @Keyword)");
                parameters.Add("Keyword", $"%{filterRequest.Keyword}%");
            }

            return whereClause.ToString();
        }

        /// <summary>
        /// Xây dựng điều kiện WHERE cho xuất CSV
        /// </summary>
        /// <param name="exportRequest">Thông tin xuất</param>
        /// <param name="parameters">DynamicParameters để thêm tham số</param>
        /// <returns>Chuỗi WHERE clause</returns>
        private string BuildWhereClauseForExport(CustomerExportRequest exportRequest, DynamicParameters parameters)
        {
            StringBuilder whereClause = new StringBuilder("WHERE is_deleted = 0");

            // Lọc theo tên khách hàng
            if (!string.IsNullOrWhiteSpace(exportRequest.CustomerName))
            {
                whereClause.Append(" AND customer_name LIKE @CustomerName");
                parameters.Add("CustomerName", $"%{exportRequest.CustomerName}%");
            }

            // Lọc theo email
            if (!string.IsNullOrWhiteSpace(exportRequest.CustomerEmail))
            {
                whereClause.Append(" AND customer_email LIKE @CustomerEmail");
                parameters.Add("CustomerEmail", $"%{exportRequest.CustomerEmail}%");
            }

            // Lọc theo số điện thoại
            if (!string.IsNullOrWhiteSpace(exportRequest.CustomerPhoneNumber))
            {
                whereClause.Append(" AND customer_phone_number LIKE @CustomerPhoneNumber");
                parameters.Add("CustomerPhoneNumber", $"%{exportRequest.CustomerPhoneNumber}%");
            }

            // Lọc theo keyword chung (tìm trên nhiều trường)
            if (!string.IsNullOrWhiteSpace(exportRequest.Keyword))
            {
                whereClause.Append(" AND (customer_name LIKE @Keyword OR customer_email LIKE @Keyword OR customer_phone_number LIKE @Keyword OR customer_code LIKE @Keyword)");
                parameters.Add("Keyword", $"%{exportRequest.Keyword}%");
            }

            return whereClause.ToString();
        }

        /// <summary>
        /// Xây dựng ORDER BY clause cho sắp xếp
        /// </summary>
        /// <param name="sortColumn">Cột cần sắp xếp</param>
        /// <param name="sortDirection">Hướng sắp xếp (ASC/DESC)</param>
        /// <returns>Chuỗi ORDER BY clause</returns>
        private string BuildOrderByClause(string sortColumn, string sortDirection)
        {
            // Mặc định sắp xếp theo ID giảm dần
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                return "ORDER BY customer_id DESC";
            }

            string columnName = ToSnakeCase(sortColumn);
            
            // Danh sách các cột được phép sắp xếp
            string[] validColumns = { 
                "customer_id", 
                "customer_code", 
                "customer_name", 
                "customer_email", 
                "customer_phone_number", 
                "customer_type", 
                "last_purchase_date" 
            };

            // Validate cột sắp xếp để tránh SQL injection
            if (!validColumns.Contains(columnName))
            {
                return "ORDER BY customer_id DESC";
            }

            string direction = sortDirection?.ToUpper() == "DESC" ? "DESC" : "ASC";
            return $"ORDER BY {columnName} {direction}";
        }

        #endregion
    }
}
