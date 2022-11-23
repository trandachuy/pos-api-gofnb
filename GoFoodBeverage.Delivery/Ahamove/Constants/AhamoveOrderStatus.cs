using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Delivery.Ahamove.Constants
{
    /// <summary>
    /// Reference: https://developers.ahamove.com/#webhook Order Status
    /// </summary>
    public class AhamoveOrderStatus
    {
        /// <summary>
        /// Idle
        /// </summary>
        public const string IDLE = "IDLE";

        /// <summary>
        /// Assigning
        /// </summary>
        public const string ASSIGNING = "ASSIGNING";

        /// <summary>
        /// Accepted
        /// </summary>
        public const string ACCEPTED = "ACCEPTED";

        /// <summary>
        /// In process
        /// </summary>
        public const string IN_PROCESS = "IN PROCESS";

        /// <summary>
        /// Completed
        /// </summary>
        public const string COMPLETED = "COMPLETED";

        /// <summary>
        /// Cancelled
        /// </summary>
        public const string CANCELLED = "CANCELLED";
    }
}
