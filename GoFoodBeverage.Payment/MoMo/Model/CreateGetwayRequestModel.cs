using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.MoMo.Model
{
    public class CreateGetwayRequestModel
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
        /// Default value is empty ""
        /// Encode base64 follow Jsonformat: {"key": "value"}
        /// Example with data: {"username": "momo"}=> data of extraData: eyJ1c2VybmFtZSI6ICJtb21vIn0 =
        /// </summary>
        public string ExtraData { get; set; } = "";

        /// <summary>
        /// Partner Transaction ID
        /// Regex: ^[0-9a-zA-Z] ([-_.]*[0 - 9a - zA - Z]+)*$
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Order info from merchant
        /// </summary>
        public string OrderInfo { get; set; }

        /// <summary>
        /// Vendor’s unique identifier for each user (e.g.: user ID or email).
        /// This ID will be linked with end-user’s MoMo account.
        /// Regex: ^[0-9a-zA-Z] ([-_.@]*[0 - 9a - zA - Z]+)*$
        /// </summary>
        public string PartnerClientId { get; set; }

        /// <summary>
        /// MoMo Payment will redirect end-user back to Vendor using this URL, to notify result for end-user using HTTP GET request type
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// MoMo Payment will notify this URL. Vendor server needs to build this URL to receive results sent from MoMo, using HTTP POST request type with header application/json.
        /// </summary>
        public string IpnUrl { get; set; }
    }
}
