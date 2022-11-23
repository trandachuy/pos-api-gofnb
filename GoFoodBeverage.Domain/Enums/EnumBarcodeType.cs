using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumBarcodeType
    {
        /// <summary>
        /// Barcode
        /// </summary>
        barcode = 0,

        /// <summary>
        /// QRcode
        /// </summary>
        qrCode = 1,
    }

    public static class EnumBarcodeTypeExtensions
    {
        public static string GetName(this EnumBarcodeType enums) => enums switch
        {
            EnumBarcodeType.barcode => "Barcode",
            EnumBarcodeType.qrCode => "QR code",
            _ => string.Empty
        };

        public static List<BarcodeType> GetList(this EnumBarcodeType enums)
        {
            return new List<BarcodeType>()
           {
               new BarcodeType
               {
                   Id = EnumBarcodeType.barcode,
                   Name = EnumBarcodeType.barcode.GetName()
               },
               new BarcodeType
               {
                   Id = EnumBarcodeType.qrCode,
                   Name = EnumBarcodeType.qrCode.GetName()
               }
           };
        }
    }

    public class BarcodeType
    {
        public EnumBarcodeType Id { get; set; }

        public string Name { get; set; }
    }
}
