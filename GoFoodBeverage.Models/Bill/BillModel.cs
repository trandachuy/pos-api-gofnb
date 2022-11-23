using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Bill
{
    public class BillModel
    {
        public EnumBillFrameSize BillFrameSize { get; set; }

        public bool IsShowLogo { get; set; }

        public string LogoData { get; set; }

        public bool IsShowAddress { get; set; }

        public bool IsShowOrderTime { get; set; }

        public bool IsShowCashierName { get; set; }

        public bool IsShowCustomerName { get; set; }

        public bool IsShowToping { get; set; }

        public bool IsShowOption { get; set; }

        public bool IsShowThanksMessage { get; set; }

        public string ThanksMessageData { get; set; }

        public bool IsShowWifiAndPassword { get; set; }

        public string WifiData { get; set; }

        public string PasswordData { get; set; }

        public bool IsShowQRCode { get; set; }

        public string QRCodeData { get; set; }

        public string QRCodeThumbnail { get; set; }

        public bool IsDefault { get; set; }

        public string Logo { get; set; }
    }
}
