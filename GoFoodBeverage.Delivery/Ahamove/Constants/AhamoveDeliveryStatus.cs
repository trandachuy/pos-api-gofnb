using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Delivery.Ahamove.Constants
{
    /// <summary>
    /// Reference: https://developers.ahamove.com/#webhook Delivery Status
    /// </summary>
    public class AhamoveDeliveryStatus
    {
        /// <summary>
        /// Completed
        /// </summary>
        public const string COMPLETED = "COMPLETED";

        /// <summary>
        /// Failed
        /// </summary>
        public const string FAILED = "FAILED";
    }
}
