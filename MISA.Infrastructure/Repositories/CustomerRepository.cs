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
                                  ORDER BY CAST(SUBSTRING(customer_code, @PrefixLength + 1) AS UNSIGNED) DESC 
                                  LIMIT 1";
            
            return dbConnection.QueryFirstOrDefault<string?>(sqlCommand, new { Prefix = prefix + "%", PrefixLength = prefix.Length });
        }

        #endregion

        #region Xuất CSV

        /// <summary>
        /// Lấy danh sách khách hàng để xuất CSV (có lọc và sắp xếp)
        /// </summary>
        /// <param name="exportRequest">Thông tin lọc và sắp xếp</param>
        /// <returns>Danh sách khách hàng</returns>
        public List<Customer> GetForExport(PagingRequest exportRequest)
        {
            DynamicParameters parameters = new DynamicParameters();
            
            // Xây dựng WHERE clause từ filter
            string whereClause = BuildWhereClause(exportRequest, parameters);
            
            // Xây dựng ORDER BY clause
            string orderByClause = "";
            if (!string.IsNullOrWhiteSpace(exportRequest.SortColumn))
            {
                string sortColumn = ToSnakeCase(exportRequest.SortColumn);
                var validColumns = typeof(Customer).GetProperties().Select(p => ToSnakeCase(p.Name));
                
                if (validColumns.Contains(sortColumn))
                {
                    string sortDirection = exportRequest.SortDirection?.ToUpper() == "DESC" ? "DESC" : "ASC";
                    orderByClause = $"ORDER BY {sortColumn} {sortDirection}";
                }
            }
            
            // Nếu không có order by, sắp xếp theo ID mặc định
            if (string.IsNullOrEmpty(orderByClause))
            {
                orderByClause = "ORDER BY customer_id DESC";
            }

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
                 customer_email, customer_shipping_address, customer_tax_code, 
                 last_purchase_date, purchased_item_code, purchased_item_name, is_deleted, customer_avatar_url)
                VALUES 
                (@CustomerId, @CustomerType, @CustomerCode, @CustomerName, @CustomerPhoneNumber, 
                 @CustomerEmail, @CustomerShippingAddress, @CustomerTaxCode, 
                 @LastPurchaseDate, @PurchasedItemCode, @PurchasedItemName, @IsDeleted, @CustomerAvatarUrl)";

            return dbConnection.Execute(sqlCommand, customers);
        }

        #endregion

        #region Private Method - Build Clause

        /// <summary>
        /// Xây dựng điều kiện WHERE cho xuất CSV
        /// </summary>
        /// <param name="filterRequest">Thông tin lọc</param>
        /// <param name="parameters">DynamicParameters để thêm tham số</param>
        /// <returns>Chuỗi WHERE clause</returns>
        private string BuildWhereClause(PagingRequest filterRequest, DynamicParameters parameters)
        {
            StringBuilder whereClause = new StringBuilder("WHERE is_deleted = 0");

            // Lọc theo tên khách hàng
            if (!string.IsNullOrWhiteSpace(filterRequest.CustomerName))
            {
                whereClause.Append(" AND " + ToSnakeCase(nameof(Customer.CustomerName)) + " LIKE @CustomerName");
                parameters.Add("CustomerName", $"%{filterRequest.CustomerName}%");
            }

            // Lọc theo email
            if (!string.IsNullOrWhiteSpace(filterRequest.CustomerEmail))
            {
                whereClause.Append(" AND " + ToSnakeCase(nameof(Customer.CustomerEmail)) + " LIKE @CustomerEmail");
                parameters.Add("CustomerEmail", $"%{filterRequest.CustomerEmail}%");
            }

            // Lọc theo số điện thoại
            if (!string.IsNullOrWhiteSpace(filterRequest.CustomerPhoneNumber))
            {
                whereClause.Append(" AND " + ToSnakeCase(nameof(Customer.CustomerPhoneNumber)) + " LIKE @CustomerPhoneNumber");
                parameters.Add("CustomerPhoneNumber", $"%{filterRequest.CustomerPhoneNumber}%");
            }

            // Lọc theo keyword chung (tìm trên nhiều trường)
            if (!string.IsNullOrWhiteSpace(filterRequest.Keyword))
            {
                var searchColumns = new string[]
                {
                    ToSnakeCase(nameof(Customer.CustomerName)),
                    ToSnakeCase(nameof(Customer.CustomerEmail)),
                    ToSnakeCase(nameof(Customer.CustomerPhoneNumber)),
                    ToSnakeCase(nameof(Customer.CustomerCode))
                };
                
                var searchConditions = searchColumns.Select(col => $"{col} LIKE @Keyword");
                whereClause.Append(" AND (" + string.Join(" OR ", searchConditions) + ")");
                parameters.Add("Keyword", $"%{filterRequest.Keyword}%");
            }

            return whereClause.ToString();
        }

        #endregion
    }
}
