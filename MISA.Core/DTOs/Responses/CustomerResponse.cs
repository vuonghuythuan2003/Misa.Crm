using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.DTOs.Responses
{
    /// <summary>
    /// DTO trả về thông tin khách hàng
    /// </summary>
    /// Created by: vuonghuythuan2003 - 02/12/2024
    public class CustomerResponse
    {
        #region Property

        /// <summary>
        /// ID khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Loại khách hàng
        /// </summary>
        public string CustomerType { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string CustomerPhoneNumber { get; set; }

        /// <summary>
        /// Email khách hàng
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Địa chỉ liên hệ chính
        /// </summary>
        public string CustomerAddress { get; set; }

        /// <summary>
        /// Địa chỉ giao hàng
        /// </summary>
        public string CustomerShippingAddress { get; set; }

        /// <summary>
        /// Mã số thuế
        /// </summary>
        public string CustomerTaxCode { get; set; }

        /// <summary>
        /// Ngày mua hàng gần nhất (chỉ lấy ngày, không lấy giờ)
        /// </summary>
        public DateTime? LastPurchaseDate { get; set; }

        /// <summary>
        /// Hàng hóa đã mua (mã)
        /// </summary>
        public string PurchasedItemCode { get; set; }

        /// <summary>
        /// Tên hàng hóa đã mua
        /// </summary>
        public string PurchasedItemName { get; set; }

        /// <summary>
        /// Đường dẫn ảnh đại diện
        /// </summary>
        public string CustomerAvatarUrl { get; set; }

        #endregion
    }
}
