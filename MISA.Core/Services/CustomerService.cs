using MISA.Core.DTOs.Requests;
using MISA.Core.DTOs.Responses;
using MISA.Core.Entities;
using MISA.Core.Interfaces.Repositories;
using MISA.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MISA.Core.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ Customer
    /// </summary>
    /// Created by: vuonghuythuan2003 - 03/12/2024
    public class CustomerService : BaseService<Customer, CustomerRequest, CustomerResponse>, ICustomerService
    {
        #region Declaration

        /// <summary>
        /// Customer Repository
        /// </summary>
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Cloudinary Service để upload ảnh
        /// </summary>
        private readonly ICloudinaryService _cloudinaryService;

        #endregion

        #region Gán loại khách hàng hàng loạt
        /// <summary>
        /// Gán loại khách hàng cho nhiều bản ghi
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng</param>
        /// <param name="customerType">Loại khách hàng mới</param>
        /// <returns>Số bản ghi đã cập nhật</returns>
        public int AssignType(List<Guid> customerIds, string customerType)
        {
            if (customerIds == null || customerIds.Count == 0 || string.IsNullOrWhiteSpace(customerType))
                return 0;
            return _customerRepository.AssignType(customerIds, customerType);
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo CustomerService
        /// </summary>
        /// <param name="customerRepository">Customer Repository</param>
        /// <param name="cloudinaryService">Cloudinary Service</param>
        public CustomerService(ICustomerRepository customerRepository, ICloudinaryService cloudinaryService) : base(customerRepository)
        {
            _customerRepository = customerRepository;
            _cloudinaryService = cloudinaryService;
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
            string prefix = "KH" + yearMonth; // KH202512 (8 ký tự)

            string maxCode = _customerRepository.GetMaxCustomerCode(prefix);

            int nextSequence = 1;
            if (!string.IsNullOrEmpty(maxCode) && maxCode.Length > prefix.Length)
            {
                // Lấy 6 số cuối (sau phần KH + yyyyMM) và tăng lên 1
                string sequenceStr = maxCode.Substring(prefix.Length); // Lấy từ vị trí 8 trở đi

                // Kiểm tra xem phần lấy ra có phải toàn số không
                if (int.TryParse(sequenceStr, out int currentSequence))
                {
                    nextSequence = currentSequence + 1;
                }
            }

            // Định dạng: prefix + 6 số tăng dần (D6 = định dạng 6 chữ số, padding 0 nếu cần)
            return prefix + nextSequence.ToString("D6");
        }



        #endregion

        #region Nhập CSV

        public ImportResponse ImportFromCsv(Stream csvStream)
        {
            ImportResponse response = new ImportResponse();
            List<Customer> validCustomers = new List<Customer>();

            HashSet<string> emailsInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> phonesInFile = new HashSet<string>();
            // Dictionary dùng để ánh xạ (map) tên cột trong header file CSV sang vị trí (index) của cột đó.
            Dictionary<string, int> columnMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Khởi tạo counter sinh mã
            string yearMonth = DateTime.Now.ToString("yyyyMM");
            string prefix = "KH" + yearMonth;

            string maxCodeInDb = _customerRepository.GetMaxCustomerCode(prefix);
            int nextSequence = 1;
            if (!string.IsNullOrEmpty(maxCodeInDb) && maxCodeInDb.Length > prefix.Length)
            {
                if (int.TryParse(maxCodeInDb.Substring(prefix.Length), out int currentSequence))
                {
                    nextSequence = currentSequence + 1;
                }
            }
            // Khai báo StreamReader để đọc file CSV dữ liệu đầu vào và đọc đúng ký tự UTF-8
            using (StreamReader reader = new StreamReader(csvStream, Encoding.UTF8))
            {
                // Đọc dòng header để xác định vị trí các cột
                string headerLine = reader.ReadLine(); 
                
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    throw new MISA.Core.Exception.ValidationException(
                        MISA.Core.Exception.ErrorCode.EmptyFile,
                        "File CSV không có dữ liệu hoặc thiếu header.",
                        null);
                }
                // để tách dòng header thành mảng các tên cột.
                string[] headers = ParseCsvLine(headerLine);
                // Duyệt qua mảng tên cột và lưu vào dictionary ánh xạ tên cột - vị trí index cột
                for (int i = 0; i < headers.Length; i++)
                {
                    string header = headers[i].Trim();
                    if (!string.IsNullOrWhiteSpace(header))
                    {
                        columnMapping[header] = i;
                    }
                }
                // Định nghĩa các tên trường chuẩn (FullName, Phone, Email...) và các tên thay thế có thể xuất hiện trong file CSV.
                Dictionary<string, string[]> requiredColumnsMap = new Dictionary<string, string[]>
                {
                    { "FullName", new[] { "FullName", "CustomerName" } },
                    { "Phone", new[] { "Phone", "CustomerPhoneNumber", "PhoneNumber" } },
                    { "Email", new[] { "Email", "CustomerEmail" } },
                    { "Address", new[] { "Address", "CustomerShippingAddress", "ShippingAddress", "CustomerAddress", "CustomerAdress" } },
                    { "CustomerType", new[] { "CustomerType" } }
                };

                List<string> missingColumns = new List<string>();
                Dictionary<string, string> mappedColumns = new Dictionary<string, string>();
                //Lặp qua từng trường bắt buộc
                foreach (var columnGroup in requiredColumnsMap)
                {
                    bool found = false;
                    //Lặp qua các tên thay thế
                    foreach (string altName in columnGroup.Value)
                    {
                        //Kiểm tra xem tên thay thế có tồn tại trong header file không. Nếu có, lưu tên cột thực tế tìm được vào mappedColumns.
                        if (columnMapping.ContainsKey(altName))
                        {
                            mappedColumns[columnGroup.Key] = altName;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        missingColumns.Add(columnGroup.Key);
                    }
                }
                // Nếu không tìm thấy đủ các cột bắt buộc, ném ra ngoại lệ và báo cho người dùng biết các cột bị thiếu.
                if (missingColumns.Count > 0)
                {
                    throw new MISA.Core.Exception.ValidationException(
                        MISA.Core.Exception.ErrorCode.MissingRequiredColumns,
                        $"File CSV thiếu các cột bắt buộc: {string.Join(", ", missingColumns)}",
                        null);
                }

                int rowNumber = 0;
                string line;
                // đọc từng dòng còn lại của file CSV
                while ((line = reader.ReadLine()) != null)
                {
                    // Tăng số thứ tự dòng và tổng số dòng được xử lý.
                    rowNumber++;
                    response.TotalRows++;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] columns = ParseCsvLine(line);
                    // Lấy giá trị từng cột theo ánh xạ đã xác định
                    string fullName = GetColumnValue(columns, columnMapping, mappedColumns["FullName"]);
                    string phone = GetColumnValue(columns, columnMapping, mappedColumns["Phone"]);
                    string email = GetColumnValue(columns, columnMapping, mappedColumns["Email"]);
                    string address = GetColumnValue(columns, columnMapping, mappedColumns["Address"]);
                    string taxCode = columnMapping.ContainsKey("TaxCode")
                        ? GetColumnValue(columns, columnMapping, mappedColumns["TaxCode"])
                        : string.Empty;
                    string customerType = GetColumnValue(columns, columnMapping, mappedColumns["CustomerType"]);
                    // Validate dữ liệu từng dòng
                    List<string> errors = ValidateCustomerData(fullName, phone, email, address, taxCode, customerType, phonesInFile, emailsInFile);
                    // Nếu có lỗi, ghi lại chi tiết lỗi vào response.Errors, tăng bộ đếm lỗi và bỏ qua dòng dữ liệu này, chuyển sang dòng tiếp theo (continue)
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

                    Customer customer = new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        CustomerType = customerType,
                        CustomerCode = prefix + nextSequence.ToString("D6"),
                        CustomerName = fullName,
                        CustomerPhoneNumber = phone,
                        CustomerEmail = email,
                        CustomerShippingAddress = address,
                        CustomerTaxCode = string.IsNullOrWhiteSpace(taxCode) ? null : taxCode,
                        LastPurchaseDate = null,
                        PurchasedItemCode = null,
                        PurchasedItemName = null,
                        IsDeleted = false
                    };

                    validCustomers.Add(customer);
                    nextSequence++;
                }
            }
            // Sau khi đọc và validate toàn bộ file, nếu có khách hàng hợp lệ thì thực hiện chèn vào database
            if (validCustomers.Count > 0)
            {
                response.SuccessCount = _customerRepository.InsertMany(validCustomers);
            }

            return response;
        }

        /// <summary>
        /// Validate dữ liệu khách hàng từ CSV
        /// </summary>
        private List<string> ValidateCustomerData(string fullName, string phone, string email, string address, string taxCode,
            string customerType, HashSet<string> phonesInFile, HashSet<string> emailsInFile)
        {
            List<string> errors = new List<string>();

            // Validate FullName
            if (string.IsNullOrWhiteSpace(fullName))
                errors.Add("Tên khách hàng không được để trống.");
            else if (fullName.Length > 255)
                errors.Add("Tên khách hàng không được vượt quá 255 ký tự.");

            // Validate Phone
            if (string.IsNullOrWhiteSpace(phone))
                errors.Add("Số điện thoại không được để trống.");
            else if (!Regex.IsMatch(phone, @"^0(3|5|7|8|9)\d{8,9}$"))
                errors.Add("Số điện thoại phải từ 10-11 số, bắt đầu bằng 03, 05, 07, 08, 09.");
            else if (phonesInFile.Contains(phone))
                errors.Add($"Số điện thoại '{phone}' bị trùng trong file.");
            else if (_customerRepository.IsPhoneNumberExist(phone))
                errors.Add($"Số điện thoại '{phone}' đã tồn tại trong hệ thống.");
            else
                phonesInFile.Add(phone);

            // Validate Email
            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email không được để trống.");
            else if (!new EmailAddressAttribute().IsValid(email))
                errors.Add("Email không đúng định dạng.");
            else if (email.Length > 100)
                errors.Add("Email không được vượt quá 100 ký tự.");
            else if (emailsInFile.Contains(email))
                errors.Add($"Email '{email}' bị trùng trong file.");
            else if (_customerRepository.IsEmailExist(email))
                errors.Add($"Email '{email}' đã tồn tại trong hệ thống.");
            else
                emailsInFile.Add(email);

            // Validate Address (optional)
            if (!string.IsNullOrWhiteSpace(address) && address.Length > 255)
                errors.Add("Địa chỉ không được vượt quá 255 ký tự.");

            // Validate TaxCode (optional)
            if (!string.IsNullOrWhiteSpace(taxCode) && taxCode.Length > 20)
                errors.Add("Mã số thuế không được vượt quá 20 ký tự.");

            // Validate CustomerType (optional)
            if (!string.IsNullOrWhiteSpace(customerType) && customerType.Length > 20)
                errors.Add("Loại khách hàng không được vượt quá 20 ký tự.");

            return errors;
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
            // Kiểm tra xem vị trí tìm được (index) có nhỏ hơn tổng số cột trong dòng dữ liệu hay không.
            // Kiểm tra xem columnMapping có chứa columnName không và index có hợp lệ không
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
        /// chức năng chính phân biệt và phân tách các cột
        /// <returns>Mảng các giá trị</returns>
        private string[] ParseCsvLine(string line)
        {

            List<string> result = new List<string>();
            // Kiểm tra trạng thái đang trong dấu ngoặc kép hay không
            bool inQuotes = false;
            // Dùng để tập hợp các ký tự tạo nên giá trị của cột.
            StringBuilder currentValue = new StringBuilder();

            // Vòng lặp duyệt qua từng ký tự c từ đầu đến cuối file CSV
            for (int i = 0; i < line.Length; i++)
            {
                //Lấy ký tự đang xét trong vòng lặp.
                char c = line[i];
                // Bắt đầu bằng dấu ngoặc kép
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        //Thêm một dấu ngoặc kép vào giá trị cột hiện tại.
                        currentValue.Append('"');
                        // Tăng i thêm 1 để bỏ qua ký tự ngoặc kép thứ hai của cặp "" (vì nó đã được xử lý).
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                // Nếu ký tự hiện tại là dấu phẩy (,) VÀ đang không nằm trong ngoặc kép (!inQuotes). Điều này xác nhận dấu phẩy này là ranh giới giữa hai cột.
                else if (c == ',' && !inQuotes)
                {
                    //Kết thúc cột hiện tại. Chuyển nội dung từ currentValue thành string và thêm vào danh sách kết quả (result).
                    result.Add(currentValue.ToString());
                    //Bắt đầu cột mới. Xóa nội dung của currentValue để chuẩn bị tích lũy các ký tự cho cột tiếp theo.
                    currentValue.Clear();
                }
                else
                {
                    // Nếu ký tự hiện tại không phải là dấu ngoặc kép và không phải là dấu phẩy phân cách
                    // (bao gồm các chữ cái, số, ký tự đặc biệt, và cả dấu phẩy nếu đang ở trạng thái inQuotes = true).
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
        public byte[] ExportToCsv(PagingRequest exportRequest)
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
                    EscapeCsvValue(customer.CustomerShippingAddress),
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
        /// đảm bảo rằng một giá trị dữ liệu cụ thể có thể được ghi vào file CSV mà không làm hỏng cấu trúc cột của file đó.
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
        /// Map từ CustomerRequest sang Customer Entity
        /// </summary>
        /// <param name="request">DTO tạo mới</param>
        /// <returns>Customer Entity</returns>
        protected override Customer MapCreateRequestToEntity(CustomerRequest request)
        {
            Customer customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                CustomerType = request.CustomerType,
                CustomerCode = request.CustomerCode,
                CustomerName = request.CustomerName,
                CustomerPhoneNumber = request.CustomerPhoneNumber,
                CustomerEmail = request.CustomerEmail,
                CustomerShippingAddress = request.CustomerShippingAddress,
                CustomerTaxCode = request.CustomerTaxCode,
                LastPurchaseDate = request.LastPurchaseDate,
                PurchasedItemCode = request.PurchasedItemCode,
                PurchasedItemName = request.PurchasedItemName,
                CustomerAvatarUrl = request.CustomerAvatarUrl,
                IsDeleted = false
            };
            return customer;
        }

        /// <summary>
        /// Map từ CustomerRequest sang Customer Entity
        /// </summary>
        /// <param name="request">DTO cập nhật</param>
        /// <param name="id">ID khách hàng (tham số thêm để khớp signature)</param>
        /// <returns>Customer Entity</returns>
        protected override Customer MapUpdateRequestToEntity(CustomerRequest request, Guid id)
        {
            Customer customer = new Customer
            {
                CustomerId = id,
                CustomerType = request.CustomerType,
                CustomerCode = request.CustomerCode,
                CustomerName = request.CustomerName,
                CustomerPhoneNumber = request.CustomerPhoneNumber,
                CustomerEmail = request.CustomerEmail,
                CustomerShippingAddress = request.CustomerShippingAddress,
                CustomerTaxCode = request.CustomerTaxCode,
                LastPurchaseDate = request.LastPurchaseDate,
                PurchasedItemCode = request.PurchasedItemCode,
                PurchasedItemName = request.PurchasedItemName,
                CustomerAvatarUrl = request.CustomerAvatarUrl ?? _customerRepository.GetById(id)?.CustomerAvatarUrl,
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
                CustomerShippingAddress = entity.CustomerShippingAddress,
                CustomerTaxCode = entity.CustomerTaxCode,
                LastPurchaseDate = entity.LastPurchaseDate,
                PurchasedItemCode = entity.PurchasedItemCode,
                PurchasedItemName = entity.PurchasedItemName,
                CustomerAvatarUrl = entity.CustomerAvatarUrl
            };
            return response;
        }

        /// <summary>
        /// Validate dữ liệu trước khi thêm mới khách hàng
        /// </summary>
        /// <param name="request">DTO tạo mới</param>
        protected override void ValidateInsert(CustomerRequest request)
        {
            // Kiểm tra số điện thoại đã tồn tại chưa
            if (_customerRepository.IsPhoneNumberExist(request.CustomerPhoneNumber))
            {
                throw new MISA.Core.Exception.ValidationException(
                    MISA.Core.Exception.ErrorCode.DuplicatePhoneNumber,
                    $"Số điện thoại '{request.CustomerPhoneNumber}' đã tồn tại trong hệ thống",
                    null);
            }

            // Kiểm tra email đã tồn tại chưa
            if (_customerRepository.IsEmailExist(request.CustomerEmail))
            {
                throw new MISA.Core.Exception.ValidationException(
                    MISA.Core.Exception.ErrorCode.DuplicateEmail,
                    $"Email '{request.CustomerEmail}' đã tồn tại trong hệ thống",
                    null);
            }
        }

        /// <summary>
        /// Validate dữ liệu trước khi cập nhật khách hàng
        /// </summary>
        /// <param name="request">DTO cập nhật</param>
        protected void ValidateUpdate(CustomerRequest request, Guid customerId)
        {
            if (_customerRepository.IsPhoneNumberExist(request.CustomerPhoneNumber, customerId))
            {
                throw new MISA.Core.Exception.ValidationException(
                    MISA.Core.Exception.ErrorCode.DuplicatePhoneNumber,
                    $"Số điện thoại '{request.CustomerPhoneNumber}' đã tồn tại trong hệ thống",
                    null);
            }

            if (_customerRepository.IsEmailExist(request.CustomerEmail, customerId))
            {
                throw new MISA.Core.Exception.ValidationException(
                    MISA.Core.Exception.ErrorCode.DuplicateEmail,
                    $"Email '{request.CustomerEmail}' đã tồn tại trong hệ thống",
                    null);
            }
        }

        public override CustomerResponse Update(Guid id, CustomerRequest request)
        {
            // Kiểm tra tồn tại trước khi cập nhật
            var entity = _customerRepository.GetById(id);
            if (entity == null)
            {
                throw new MISA.Core.Exception.NotFoundException(typeof(Customer).Name, id);
            }
            // Validate dữ liệu trước khi cập nhật
            ValidateUpdate(request, id);
            // Map từ Request DTO sang Entity với ID từ parameter
            var updatedEntity = MapUpdateRequestToEntity(request, id);
            // Cập nhật vào database
            updatedEntity = _customerRepository.Update(updatedEntity);
            // Trả về Response DTO
            return MapEntityToResponse(updatedEntity);
        }

        #endregion
    }
}
