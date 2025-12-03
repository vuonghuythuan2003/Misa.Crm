using MISA.Core.DTOs.Requests;
using MISA.Core.DTOs.Responses;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repositories;
using MISA.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.Core.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ Customer
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class CustomerService : BaseService<Customer, CustomerCreateRequest, CustomerUpdateRequest, CustomerResponse>, ICustomerService
    {
        #region Declaration

        /// <summary>
        /// Customer Repository
        /// </summary>
        private readonly ICustomerRepository _customerRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo CustomerService
        /// </summary>
        /// <param name="customerRepository">Customer Repository</param>
        public CustomerService(ICustomerRepository customerRepository) : base(customerRepository)
        {
            _customerRepository = customerRepository;
        }

        #endregion

        #region Sinh mã khách hàng

        /// <summary>
        /// Sinh mã khách hàng tự động theo định dạng KH + yyyyMM + 6 số tăng dần
        /// Ví dụ: KH202512000001
        /// </summary>
        /// <returns>Mã khách hàng mới</returns>
        public string GenerateCustomerCode()
        {
            string yearMonth = DateTime.Now.ToString("yyyyMM");
            string prefix = "KH" + yearMonth;

            string maxCode = _customerRepository.GetMaxCustomerCode(prefix);

            int nextSequence = 1;
            if (!string.IsNullOrEmpty(maxCode))
            {
                // Lấy 6 số cuối và tăng lên 1
                string sequenceStr = maxCode.Substring(8);
                nextSequence = int.Parse(sequenceStr) + 1;
            }
            // Decimal độ dài mong muốn là 6
            return prefix + nextSequence.ToString("D6");
        }

        #endregion

        #region Phân trang

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang</param>
        /// <returns>Response DTO chứa danh sách và thông tin phân trang</returns>
        public PagingResponse<CustomerResponse> GetPaging(CustomerPagingRequest pagingRequest)
        {
            // Lấy dữ liệu từ repository
            var (customers, totalRecords) = _customerRepository.GetPaging(pagingRequest);

            // Map từ Entity sang Response DTO
            List<CustomerResponse> responses = new List<CustomerResponse>();
            foreach (Customer customer in customers)
            {
                responses.Add(MapEntityToResponse(customer));
            }

            // Tạo response phân trang
            return new PagingResponse<CustomerResponse>(responses, totalRecords, pagingRequest.PageNumber, pagingRequest.PageSize);
        }

        #endregion

        #region Sắp xếp

        /// <summary>
        /// Lấy danh sách khách hàng có sắp xếp
        /// </summary>
        /// <param name="sortRequest">Thông tin sắp xếp</param>
        /// <returns>Danh sách khách hàng đã sắp xếp</returns>
        public List<CustomerResponse> GetSorted(CustomerSortRequest sortRequest)
        {
            // Lấy dữ liệu từ repository
            List<Customer> customers = _customerRepository.GetSorted(sortRequest);

            // Map từ Entity sang Response DTO
            List<CustomerResponse> responses = new List<CustomerResponse>();
            foreach (Customer customer in customers)
            {
                responses.Add(MapEntityToResponse(customer));
            }

            return responses;
        }

        #endregion

        #region Lọc nhanh

        /// <summary>
        /// Lọc nhanh khách hàng theo tên, email, số điện thoại
        /// </summary>
        /// <param name="filterRequest">Thông tin lọc nhanh</param>
        /// <returns>Danh sách khách hàng thỏa điều kiện lọc</returns>
        public List<CustomerResponse> QuickFilter(CustomerQuickFilterRequest filterRequest)
        {
            // Lấy dữ liệu từ repository
            List<Customer> customers = _customerRepository.QuickFilter(filterRequest);

            // Map từ Entity sang Response DTO
            List<CustomerResponse> responses = new List<CustomerResponse>();
            foreach (Customer customer in customers)
            {
                responses.Add(MapEntityToResponse(customer));
            }

            return responses;
        }

        #endregion

        #region Nhập CSV

        /// <summary>
        /// Nhập khách hàng từ file CSV
        /// Mapping bắt buộc: FullName, Phone, Email, Address, CustomerType
        /// </summary>
        /// <param name="csvStream">Stream của file CSV</param>
        /// <returns>Kết quả nhập dữ liệu</returns>
        public ImportResponse ImportFromCsv(Stream csvStream)
        {
            ImportResponse response = new ImportResponse();
            List<Customer> validCustomers = new List<Customer>();
            
            // Danh sách để kiểm tra trùng lặp trong file
            HashSet<string> emailsInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> phonesInFile = new HashSet<string>();

            // Dictionary để lưu vị trí các cột theo tên header
            Dictionary<string, int> columnMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            using (StreamReader reader = new StreamReader(csvStream, Encoding.UTF8))
            {
                // Đọc header và mapping cột
                string headerLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    throw new MISA.Core.Exception.BusinessException(
                        MISA.Core.Exception.ErrorCode.EmptyFile, 
                        "File CSV không có dữ liệu hoặc thiếu header.");
                }

                // Parse header để lấy vị trí các cột
                string[] headers = ParseCsvLine(headerLine);
                for (int i = 0; i < headers.Length; i++)
                {
                    string header = headers[i].Trim();
                    if (!string.IsNullOrWhiteSpace(header))
                    {
                        columnMapping[header] = i;
                    }
                }

                // Kiểm tra các cột bắt buộc
                string[] requiredColumns = { "FullName", "Phone", "Email", "Address", "CustomerType" };
                List<string> missingColumns = new List<string>();
                foreach (string col in requiredColumns)
                {
                    if (!columnMapping.ContainsKey(col))
                    {
                        missingColumns.Add(col);
                    }
                }

                if (missingColumns.Count > 0)
                {
                    throw new MISA.Core.Exception.BusinessException(
                        MISA.Core.Exception.ErrorCode.MissingRequiredColumns, 
                        $"File CSV thiếu các cột bắt buộc: {string.Join(", ", missingColumns)}");
                }

                int rowNumber = 0;
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    rowNumber++;
                    response.TotalRows++;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    List<string> errors = new List<string>();
                    string[] columns = ParseCsvLine(line);

                    // Lấy dữ liệu từ các cột theo mapping
                    string fullName = GetColumnValue(columns, columnMapping, "FullName");
                    string phone = GetColumnValue(columns, columnMapping, "Phone");
                    string email = GetColumnValue(columns, columnMapping, "Email");
                    string address = GetColumnValue(columns, columnMapping, "Address");
                    string customerType = GetColumnValue(columns, columnMapping, "CustomerType");

                    // === VALIDATE DỮ LIỆU ===

                    // 1. Tên khách hàng (FullName) - Bắt buộc, tối đa 255 ký tự
                    if (string.IsNullOrWhiteSpace(fullName))
                    {
                        errors.Add("Tên khách hàng (FullName) không được để trống.");
                    }
                    else if (fullName.Length > 255)
                    {
                        errors.Add("Tên khách hàng không được vượt quá 255 ký tự.");
                    }

                    // 2. Số điện thoại (Phone) - Bắt buộc, unique, 10-11 số
                    if (string.IsNullOrWhiteSpace(phone))
                    {
                        errors.Add("Số điện thoại (Phone) không được để trống.");
                    }
                    else if (!Regex.IsMatch(phone, @"^0(3|5|7|8|9)\d{8,9}$"))
                    {
                        errors.Add("Số điện thoại phải từ 10-11 số, bắt đầu bằng 03, 05, 07, 08, 09.");
                    }
                    else
                    {
                        // Kiểm tra trùng trong file
                        if (phonesInFile.Contains(phone))
                        {
                            errors.Add($"Số điện thoại '{phone}' bị trùng trong file.");
                        }
                        // Kiểm tra trùng trong database
                        else if (_customerRepository.IsPhoneNumberExist(phone))
                        {
                            errors.Add($"Số điện thoại '{phone}' đã tồn tại trong hệ thống.");
                        }
                        else
                        {
                            phonesInFile.Add(phone);
                        }
                    }

                    // 3. Email - Bắt buộc, unique, đúng định dạng, tối đa 100 ký tự
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        errors.Add("Email không được để trống.");
                    }
                    else if (!new EmailAddressAttribute().IsValid(email))
                    {
                        errors.Add("Email không đúng định dạng.");
                    }
                    else if (email.Length > 100)
                    {
                        errors.Add("Email không được vượt quá 100 ký tự.");
                    }
                    else
                    {
                        // Kiểm tra trùng trong file
                        if (emailsInFile.Contains(email))
                        {
                            errors.Add($"Email '{email}' bị trùng trong file.");
                        }
                        // Kiểm tra trùng trong database
                        else if (_customerRepository.IsEmailExist(email))
                        {
                            errors.Add($"Email '{email}' đã tồn tại trong hệ thống.");
                        }
                        else
                        {
                            emailsInFile.Add(email);
                        }
                    }

                    // 4. Địa chỉ (Address) - Bắt buộc, tối đa 255 ký tự
                    if (string.IsNullOrWhiteSpace(address))
                    {
                        errors.Add("Địa chỉ (Address) không được để trống.");
                    }
                    else if (address.Length > 255)
                    {
                        errors.Add("Địa chỉ không được vượt quá 255 ký tự.");
                    }

                    // 5. Loại khách hàng (CustomerType) - Bắt buộc, tối đa 20 ký tự
                    if (string.IsNullOrWhiteSpace(customerType))
                    {
                        errors.Add("Loại khách hàng (CustomerType) không được để trống.");
                    }
                    else if (customerType.Length > 20)
                    {
                        errors.Add("Loại khách hàng không được vượt quá 20 ký tự.");
                    }

                    // Nếu có lỗi, thêm vào danh sách lỗi
                    if (errors.Count > 0)
                    {
                        response.Errors.Add(new ImportErrorDetail
                        {
                            RowNumber = rowNumber,
                            RowData = line,
                            ErrorMessages = errors
                        });
                        response.ErrorCount++;
                        continue;
                    }

                    // Tự động sinh mã khách hàng
                    string customerCode = GenerateCustomerCode();

                    // Tạo entity Customer
                    Customer customer = new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        CustomerType = customerType,
                        CustomerCode = customerCode,
                        CustomerName = fullName,
                        CustomerPhoneNumber = phone,
                        CustomerEmail = email,
                        CustomerAddress = address,
                        CustomerShippingAddress = address, // Mặc định dùng địa chỉ liên hệ
                        CustomerTaxCode = "", // Để trống
                        LastPurchaseDate = null,
                        PurchasedItemCode = null,
                        PurchasedItemName = null,
                        IsDeleted = false
                    };

                    validCustomers.Add(customer);
                }
            }

            // Insert các bản ghi hợp lệ vào database
            if (validCustomers.Count > 0)
            {
                int insertedCount = _customerRepository.InsertMany(validCustomers);
                response.SuccessCount = insertedCount;
            }

            return response;
        }

        /// <summary>
        /// Lấy giá trị cột từ mảng dữ liệu theo tên cột
        /// </summary>
        /// <param name="columns">Mảng giá trị các cột</param>
        /// <param name="columnMapping">Dictionary mapping tên cột - vị trí</param>
        /// <param name="columnName">Tên cột cần lấy</param>
        /// <returns>Giá trị của cột hoặc chuỗi rỗng nếu không tìm thấy</returns>
        private string GetColumnValue(string[] columns, Dictionary<string, int> columnMapping, string columnName)
        {
            if (columnMapping.TryGetValue(columnName, out int index) && index < columns.Length)
            {
                return columns[index].Trim();
            }
            return "";
        }

        /// <summary>
        /// Parse một dòng CSV, xử lý cả trường hợp có dấu ngoặc kép
        /// </summary>
        /// <param name="line">Dòng CSV cần parse</param>
        /// <returns>Mảng các giá trị</returns>
        private string[] ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            StringBuilder currentValue = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Escaped quote
                        currentValue.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            result.Add(currentValue.ToString());
            return result.ToArray();
        }

        #endregion

        #region Xuất CSV

        /// <summary>
        /// Xuất danh sách khách hàng ra file CSV (có lọc và sắp xếp)
        /// </summary>
        /// <param name="exportRequest">Thông tin lọc và sắp xếp</param>
        /// <returns>Byte array chứa nội dung file CSV</returns>
        public byte[] ExportToCsv(CustomerExportRequest exportRequest)
        {
            // Lấy danh sách khách hàng theo điều kiện lọc (không phân trang)
            List<Customer> customers = _customerRepository.GetForExport(exportRequest);

            StringBuilder csvBuilder = new StringBuilder();

            // Thêm BOM cho UTF-8 để Excel đọc đúng tiếng Việt
            csvBuilder.Append('\uFEFF');

            // Header
            csvBuilder.AppendLine("STT,Mã khách hàng,Tên khách hàng,Loại khách hàng,Số điện thoại,Email,Địa chỉ,Địa chỉ giao hàng,Mã số thuế,Ngày mua gần nhất,Mã hàng đã mua,Tên hàng đã mua");

            // Data rows
            int stt = 1;
            foreach (Customer customer in customers)
            {
                csvBuilder.AppendLine(string.Join(",",
                    stt++,
                    EscapeCsvValue(customer.CustomerCode),
                    EscapeCsvValue(customer.CustomerName),
                    EscapeCsvValue(customer.CustomerType),
                    EscapeCsvValue(customer.CustomerPhoneNumber),
                    EscapeCsvValue(customer.CustomerEmail),
                    EscapeCsvValue(customer.CustomerAddress),
                    EscapeCsvValue(customer.CustomerShippingAddress),
                    EscapeCsvValue(customer.CustomerTaxCode),
                    customer.LastPurchaseDate?.ToString("dd/MM/yyyy") ?? "",
                    EscapeCsvValue(customer.PurchasedItemCode),
                    EscapeCsvValue(customer.PurchasedItemName)
                ));
            }

            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        /// <summary>
        /// Escape giá trị để đưa vào CSV (xử lý dấu phẩy, dấu ngoặc kép, xuống dòng)
        /// </summary>
        /// <param name="value">Giá trị cần escape</param>
        /// <returns>Giá trị đã escape</returns>
        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            // Nếu giá trị chứa dấu phẩy, dấu ngoặc kép hoặc xuống dòng thì bọc trong dấu ngoặc kép
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                // Escape dấu ngoặc kép bằng cách nhân đôi
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Map từ CustomerCreateRequest sang Customer Entity
        /// </summary>
        /// <param name="request">DTO tạo mới</param>
        /// <returns>Customer Entity</returns>
        protected override Customer MapCreateRequestToEntity(CustomerCreateRequest request)
        {
            Customer customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                CustomerType = request.CustomerType,
                CustomerCode = request.CustomerCode,
                CustomerName = request.CustomerName,
                CustomerPhoneNumber = request.CustomerPhoneNumber,
                CustomerEmail = request.CustomerEmail,
                CustomerAddress = request.CustomerAddress,
                CustomerShippingAddress = request.CustomerShippingAddress,
                CustomerTaxCode = request.CustomerTaxCode,
                LastPurchaseDate = request.LastPurchaseDate,
                PurchasedItemCode = request.PurchasedItemCode,
                PurchasedItemName = request.PurchasedItemName,
                IsDeleted = false
            };
            return customer;
        }

        /// <summary>
        /// Map từ CustomerUpdateRequest sang Customer Entity
        /// </summary>
        /// <param name="request">DTO cập nhật</param>
        /// <returns>Customer Entity</returns>
        protected override Customer MapUpdateRequestToEntity(CustomerUpdateRequest request)
        {
            Customer customer = new Customer
            {
                CustomerId = request.CustomerId,
                CustomerType = request.CustomerType,
                CustomerCode = request.CustomerCode,
                CustomerName = request.CustomerName,
                CustomerPhoneNumber = request.CustomerPhoneNumber,
                CustomerEmail = request.CustomerEmail,
                CustomerAddress = request.CustomerAddress,
                CustomerShippingAddress = request.CustomerShippingAddress,
                CustomerTaxCode = request.CustomerTaxCode,
                LastPurchaseDate = request.LastPurchaseDate,
                PurchasedItemCode = request.PurchasedItemCode,
                PurchasedItemName = request.PurchasedItemName,
                IsDeleted = false
            };
            return customer;
        }

        /// <summary>
        /// Map từ Customer Entity sang CustomerResponse DTO
        /// </summary>
        /// <param name="entity">Customer Entity</param>
        /// <returns>CustomerResponse DTO</returns>
        protected override CustomerResponse MapEntityToResponse(Customer entity)
        {
            CustomerResponse response = new CustomerResponse
            {
                CustomerId = entity.CustomerId,
                CustomerType = entity.CustomerType,
                CustomerCode = entity.CustomerCode,
                CustomerName = entity.CustomerName,
                CustomerPhoneNumber = entity.CustomerPhoneNumber,
                CustomerEmail = entity.CustomerEmail,
                CustomerAddress = entity.CustomerAddress,
                CustomerShippingAddress = entity.CustomerShippingAddress,
                CustomerTaxCode = entity.CustomerTaxCode,
                LastPurchaseDate = entity.LastPurchaseDate,
                PurchasedItemCode = entity.PurchasedItemCode,
                PurchasedItemName = entity.PurchasedItemName
            };
            return response;
        }

        /// <summary>
        /// Validate dữ liệu trước khi thêm mới khách hàng
        /// </summary>
        /// <param name="request">DTO tạo mới</param>
        protected override void ValidateInsert(CustomerCreateRequest request)
        {
            // Kiểm tra số điện thoại đã tồn tại chưa
            if (_customerRepository.IsPhoneNumberExist(request.CustomerPhoneNumber))
            {
                throw new MISA.Core.Exception.DuplicateException("Số điện thoại", request.CustomerPhoneNumber);
            }

            // Kiểm tra email đã tồn tại chưa
            if (_customerRepository.IsEmailExist(request.CustomerEmail))
            {
                throw new MISA.Core.Exception.DuplicateException("Email", request.CustomerEmail);
            }
        }

        /// <summary>
        /// Validate dữ liệu trước khi cập nhật khách hàng
        /// </summary>
        /// <param name="request">DTO cập nhật</param>
        protected override void ValidateUpdate(CustomerUpdateRequest request)
        {
            // Kiểm tra khách hàng có tồn tại không
            Customer existingCustomer = _customerRepository.GetById(request.CustomerId);
            if (existingCustomer == null)
            {
                throw new MISA.Core.Exception.NotFoundException("Khách hàng", request.CustomerId);
            }

            // Kiểm tra số điện thoại đã tồn tại chưa (loại trừ chính khách hàng đang cập nhật)
            if (_customerRepository.IsPhoneNumberExist(request.CustomerPhoneNumber, request.CustomerId))
            {
                throw new MISA.Core.Exception.DuplicateException("Số điện thoại", request.CustomerPhoneNumber);
            }

            // Kiểm tra email đã tồn tại chưa (loại trừ chính khách hàng đang cập nhật)
            if (_customerRepository.IsEmailExist(request.CustomerEmail, request.CustomerId))
            {
                throw new MISA.Core.Exception.DuplicateException("Email", request.CustomerEmail);
            }
        }

        #endregion
    }
}
