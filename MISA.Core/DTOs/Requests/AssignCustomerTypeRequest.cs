using System;
using System.Collections.Generic;

namespace MISA.Core.DTOs.Requests
{
    /// <summary>
    /// DTO cho request gán loại khách hàng hàng loạt
    /// </summary>
    public class AssignCustomerTypeRequest
    {
        public List<Guid> CustomerIds { get; set; }
        public string CustomerType { get; set; }
    }
}