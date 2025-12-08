using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Requests
{
    /// <summary>
    /// DTO để tạo mới hoặc cập nhật khách hàng
    /// </summary>
    /// Created by: vuonghuythuan2003 - 04/12/2024
    public class CustomerRequest
    {
        /// <summary>
        /// ID khách hàng (dùng cho update)
        /// </summary>
        public Guid CustomerId { get; set; }
        #region Property

        /// <summary>
        /// Loại khách hàng
        /// </summary>
        [Required(ErrorMessage = "Loại khách hàng không được để trống")]
        [MaxLength(20, ErrorMessage = "Loại khách hàng không được vượt quá 20 ký tự")]
        public string CustomerType { get; set; }

        /// <summary>
        /// Mã khách hàng (Unique)
        /// </summary>
        [Required(ErrorMessage = "Mã khách hàng không được để trống")]
        [MaxLength(20, ErrorMessage = "Mã khách hàng không được vượt quá 20 ký tự")]
        public string CustomerCode { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        [MaxLength(255, ErrorMessage = "Tên khách hàng không được vượt quá 255 ký tự")]
        public string CustomerName { get; set; }

        /// <summary>
        /// Số điện thoại (Unique, 10-11 số)
        /// </summary>
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0(3|5|7|8|9)\d{8,9}$", ErrorMessage = "Số điện thoại phải từ 10-11 số")]
        public string CustomerPhoneNumber { get; set; }

        /// <summary>
        /// Email khách hàng (Unique)
        /// </summary>
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Địa chỉ giao hàng
        /// </summary>
        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        [MaxLength(255, ErrorMessage = "Địa chỉ giao hàng không được vượt quá 255 ký tự")]
        public string CustomerShippingAddress { get; set; }

        /// <summary>
        /// Mã số thuế (không bắt buộc)
        /// </summary>
        [MaxLength(20, ErrorMessage = "Mã số thuế không được vượt quá 20 ký tự")]
        public string? CustomerTaxCode { get; set; }

        /// <summary>
        /// Ngày mua hàng gần nhất (chỉ lấy ngày, không lấy giờ)
        /// </summary>
        public DateTime? LastPurchaseDate { get; set; }

        /// <summary>
        /// Hàng hóa đã mua (mã)
        /// </summary>
        [MaxLength(100, ErrorMessage = "Mã hàng hóa không được vượt quá 100 ký tự")]
        public string? PurchasedItemCode { get; set; }

        /// <summary>
        /// Tên hàng hóa đã mua
        /// </summary>
        [MaxLength(100, ErrorMessage = "Tên hàng hóa không được vượt quá 100 ký tự")]
        public string? PurchasedItemName { get; set; }

        /// <summary>
        /// Ảnh đại diện khách hàng (URL)
        /// </summary>
        [MaxLength(500, ErrorMessage = "URL ảnh không được vượt quá 500 ký tự")]
        public string? CustomerAvatarUrl { get; set; }
        #endregion
    }
}
