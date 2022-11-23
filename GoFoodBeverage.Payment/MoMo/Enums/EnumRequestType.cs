using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.MoMo.Enums
{
    public class RequestTypes
    {
        /// <summary>
        /// One-Time Payments
        /// One-Time (Non-Recurring) Checkout Payments with MoMo E-Wallet are supported on various platforms:
        /// Desktop websites
        /// Mobile websites
        /// Mobile apps
        /// Devices that do not support internet browser
        /// </summary>
        public const string CaptureWallet = "captureWallet";

        /// <summary>
        /// Account Binding
        /// Recurring payments offers a one-click payment experience for your website or mobile app enabling payments via MoMo E-Wallet. Your customers need to once authorize and link their MoMo account with your application (binding) and later enjoy fast checkout every time
        /// </summary>
        public const string LinkWallet = "linkWallet";
    }
}
