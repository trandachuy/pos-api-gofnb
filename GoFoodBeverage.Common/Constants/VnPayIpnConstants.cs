using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Common.Constants
{
    public sealed class VnPayIpnConstants
    {
        public class Code
        {
            public const string SUCCESS = "00";
            public const string ORDER_NOT_FOUND = "01";
            public const string ORDER_ALREADY_CONFIRMED = "02";
            public const string INVALID_AMOUNT = "04";
            public const string INVALID_SIGNATURE = "97";
            public const string INVALID_DATA = "99";
        };

        public class Message
        {
            public const string SUCCESS = "The order has been paid.";
            public const string ORDER_NOT_FOUND = "The order not found.";
            public const string ORDER_ALREADY_CONFIRMED = "Order already confirmed";
            public const string INVALID_AMOUNT = "Invalid amount";
            public const string INVALID_SIGNATURE = "Invalid signature";
            public const string INVALID_DATA = "Invalid data.";
        };
    }
}
