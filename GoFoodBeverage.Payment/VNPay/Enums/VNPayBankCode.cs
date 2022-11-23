using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.VNPay.Enums
{
    public class VNPayBankCode
    {
        /// <summary>
        /// VNPAYQR
        /// </summary>
        public const string VNPAYQR = "VNPAYQR";

        /// <summary>
        /// ATM card - Domestic bank account
        /// </summary>
        public const string VNBANK = "VNBANK";

        /// <summary>
        /// International payment cards
        /// </summary>
        public const string INTCARD = "INTCARD";
    }
}
