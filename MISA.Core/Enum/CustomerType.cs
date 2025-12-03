using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Enum
{
    /// <summary>
    /// Loại khách hàng
    /// </summary>
    /// Created by: vuonghuythuan2003 - 02/12/2024
    public enum CustomerType
    {
        /// <summary>
        /// Khách hàng mua buôn (NBH01)
        /// </summary>
        [Description("NBH01")]
        Nbh01 = 0,

        /// <summary>
        /// Khách hàng vãng lai (LKHA)
        /// </summary>
        [Description("LKHA")]
        Lkha = 1,

        /// <summary>
        /// Khách hàng VIP
        /// </summary>
        [Description("VIP")]
        Vip = 2
    }
}
