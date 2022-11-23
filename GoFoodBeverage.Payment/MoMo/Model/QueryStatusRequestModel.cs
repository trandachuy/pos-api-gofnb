using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.MoMo.Model
{
    public class QueryStatusRequestModel
    {
        /// <summary>
        /// Request ID, unique for each request, MoMo's partner uses the requestId field for idempotency control
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Order amount in VND (0 VNĐ or greater)
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Partner Transaction ID
        /// Regex: ^[0-9a-zA-Z] ([-_.]*[0 - 9a - zA - Z]+)*$
        /// </summary>
        public string OrderId { get; set; }
    }
}
