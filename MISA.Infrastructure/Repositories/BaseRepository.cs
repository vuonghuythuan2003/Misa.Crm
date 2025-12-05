using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.Core.DTOs.Requests;
using MISA.Core.Interfaces.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.Infrastructure.Repositories
{
    /// <summary>
    /// Base repository chứa các phương thức CRUD cơ bản
    /// <list type="bullet">
    /// <item>Các hàm thực hiện thêm/sửa/xóa bản ghi trong Database</item>
    /// <item>Các hàm thực hiện load dữ liệu từ Database</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T">Kiểu entity</typeparam>
    /// Created by: vuonghuythuan2003 - 02/12/2024
    public class BaseRepository<T> : IBaseRepository<T>, IDisposable where T : class
    {
        #region Declaration

        /// <summary>
        /// Chứa toàn bộ thông tin cần thiết để tìm thấy và mở cửa Database.
        /// </summary>
        protected readonly string connectionString;

        /// <summary>
        /// Đối tượng kết nối database
        /// </summary>
        protected readonly IDbConnection dbConnection;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo BaseRepository với cấu hình kết nối
        /// Tìm đến lớp appsettings.json để lấy chuỗi kết nối
        /// Cây cầu kết nối đến database MySQL
        /// </summary>
        /// <param name="config">Cấu hình ứng dụng</param>
        public BaseRepository(IConfiguration config)
        {
            connectionString = config.GetConnectionString("StrongConnection") ?? throw new ArgumentNullException(nameof(config), "Connection string 'StrongConnection' not found");
            dbConnection = new MySqlConnection(connectionString);
        }

        #endregion

        #region Method

        /// <summary>
        /// Chuyển đổi PascalCase sang snake_case
        /// Ví dụ: CustomerId -> customer_id
        /// </summary>
        /// <param name="name">Tên cần chuyển đổi</param>
        /// <returns>Tên dạng snake_case</returns>
        protected string ToSnakeCase(string name)
        {
            return Regex.Replace(name, "([a-z])([A-Z])", "$1_$2").ToLower();
        }

        /// <summary>
        /// Giải phóng tài nguyên kết nối database
        /// </summary>
        public void Dispose()
        {
            dbConnection.Dispose();
        }

        /// <summary>
        /// Lấy tất cả bản ghi (không bao gồm bản ghi đã xóa mềm)
        /// </summary>
        /// <returns>Danh sách entity</returns>
        public List<T> GetAll()
        {
            string tableName = ToSnakeCase(typeof(T).Name);
            string sqlCommand = $"SELECT * FROM {tableName} WHERE is_deleted = 0";
            return dbConnection.Query<T>(sqlCommand).ToList();
        }

        /// <summary>
        /// Lấy bản ghi theo ID (không bao gồm bản ghi đã xóa mềm)
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Entity tìm được hoặc null</returns>
        public T? GetById(Guid id)
        {
            string tableName = ToSnakeCase(typeof(T).Name);
            string idColumnName = ToSnakeCase(typeof(T).Name + "Id");
            string sqlCommand = $"SELECT * FROM {tableName} WHERE {idColumnName} = @Id AND is_deleted = 0";
            return dbConnection.QueryFirstOrDefault<T>(sqlCommand, new { Id = id });
        }

        /// <summary>
        /// Thêm mới entity vào database
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Entity đã thêm</returns>
        public T Insert(T entity)
        {
            string tableName = ToSnakeCase(typeof(T).Name);
            var properties = typeof(T).GetProperties();
            string columnNames = string.Join(", ", properties.Select(p => ToSnakeCase(p.Name)));
            string parameterNames = string.Join(", ", properties.Select(p => "@" + p.Name));
            string sqlCommand = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";
            dbConnection.Execute(sqlCommand, entity);
            return entity;
        }

        /// <summary>
        /// Xóa mềm entity (đánh dấu is_deleted = 1)
        /// </summary>
        /// <param name="entityId">ID của entity cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public int SoftDelete(Guid entityId)
        {
            string tableName = ToSnakeCase(typeof(T).Name);
            string idColumnName = ToSnakeCase(typeof(T).Name + "Id");
            string sqlCommand = $"UPDATE {tableName} SET is_deleted = 1 WHERE {idColumnName} = @Id";
            return dbConnection.Execute(sqlCommand, new { Id = entityId });
        }

        /// <summary>
        /// Xóa mềm hàng loạt entities (đánh dấu is_deleted = 1 cho nhiều bản ghi)
        /// </summary>
        /// <param name="entityIds">Danh sách ID của entities cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        public int SoftDeleteMany(List<Guid> entityIds)
        {
            if (entityIds == null || entityIds.Count == 0)
                return 0;

            string tableName = ToSnakeCase(typeof(T).Name);
            string idColumnName = ToSnakeCase(typeof(T).Name + "Id");
            string placeholders = string.Join(",", entityIds.Select((_, i) => $"@Id{i}"));
            string sqlCommand = $"UPDATE {tableName} SET is_deleted = 1 WHERE {idColumnName} IN ({placeholders})";
            
            var parameters = new DynamicParameters();
            for (int i = 0; i < entityIds.Count; i++)
            {
                parameters.Add($"@Id{i}", entityIds[i]);
            }

            return dbConnection.Execute(sqlCommand, parameters);
        }

        /// <summary>
        /// Cập nhật entity trong database
        /// </summary>
        /// <param name="entity">Entity cần cập nhật</param>
        /// <returns>Entity đã cập nhật</returns>
        public T Update(T entity)
        {
            string tableName = ToSnakeCase(typeof(T).Name);
            var properties = typeof(T).GetProperties();
            string idPropertyName = typeof(T).Name + "Id";
            string idColumnName = ToSnakeCase(idPropertyName);
            
            string setClause = string.Join(", ", properties
                .Where(p => p.Name != idPropertyName)
                .Select(p => $"{ToSnakeCase(p.Name)} = @{p.Name}"));
            
            string sqlCommand = $"UPDATE {tableName} SET {setClause} WHERE {idColumnName} = @{idPropertyName}";
            dbConnection.Execute(sqlCommand, entity);
            return entity;
        }

        /// <summary>
        /// Lấy danh sách bản ghi với phân trang và sắp xếp
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang và sắp xếp</param>
        /// <returns>Tuple chứa danh sách entity và tổng số bản ghi</returns>
        public (List<T> Data, int TotalRecords) GetPaging(PagingRequest pagingRequest)
        {
            string tableName = ToSnakeCase(typeof(T).Name);
            
            // Xây dựng điều kiện WHERE
            string whereClause = "WHERE is_deleted = 0";
            
            // Thêm tìm kiếm theo keyword nếu có
            if (!string.IsNullOrWhiteSpace(pagingRequest.Keyword))
            {
                // Tìm kiếm trên tất cả các cột kiểu string
                var stringProperties = typeof(T).GetProperties()
                    .Where(p => p.PropertyType == typeof(string));
                
                if (stringProperties.Any())
                {
                    var searchConditions = stringProperties
                        .Select(p => $"{ToSnakeCase(p.Name)} LIKE @Keyword");
                    whereClause += $" AND ({string.Join(" OR ", searchConditions)})";
                }
            }
            
            // Xây dựng ORDER BY
            string orderByClause = "";
            if (!string.IsNullOrWhiteSpace(pagingRequest.SortColumn))
            {
                // Validate sort column để tránh SQL Injection
                string sortColumn = ToSnakeCase(pagingRequest.SortColumn);
                var validColumns = typeof(T).GetProperties().Select(p => ToSnakeCase(p.Name));
                
                if (validColumns.Contains(sortColumn))
                {
                    string sortDirection = pagingRequest.SortDirection?.ToUpper() == "DESC" ? "DESC" : "ASC";
                    orderByClause = $"ORDER BY {sortColumn} {sortDirection}";
                }
            }
            
            // Nếu không có order by, sắp xếp theo ID mặc định
            if (string.IsNullOrEmpty(orderByClause))
            {
                string idColumnName = ToSnakeCase(typeof(T).Name + "Id");
                orderByClause = $"ORDER BY {idColumnName} DESC";
            }
            
            // Tính offset
            int offset = (pagingRequest.PageNumber - 1) * pagingRequest.PageSize;
            
            // Query lấy dữ liệu phân trang
            string sqlData = $@"SELECT * FROM {tableName} 
                               {whereClause} 
                               {orderByClause} 
                               LIMIT @PageSize OFFSET @Offset";
            
            // Query đếm tổng số bản ghi
            string sqlCount = $"SELECT COUNT(*) FROM {tableName} {whereClause}";
            
            var parameters = new DynamicParameters();
            parameters.Add("PageSize", pagingRequest.PageSize);
            parameters.Add("Offset", offset);
            
            if (!string.IsNullOrWhiteSpace(pagingRequest.Keyword))
            {
                parameters.Add("Keyword", $"%{pagingRequest.Keyword}%");
            }
            
            List<T> data = dbConnection.Query<T>(sqlData, parameters).ToList();
            int totalRecords = dbConnection.ExecuteScalar<int>(sqlCount, parameters);
            
            return (data, totalRecords);
        }

        #endregion
    }
}
