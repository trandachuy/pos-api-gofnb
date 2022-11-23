using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.VNPay.Enums
{
    public class VNPayQueryResponseCode
    {
        /// <summary>
        /// Merchant không hợp lệ (kiểm tra lại vnp_TmnCode)
        /// </summary>
        public const string MerchantInvalid = "02";

        /// <summary>
        /// Dữ liệu gửi sang không đúng định dạng
        /// </summary>
        public const string ErrorDataFormat = "03";

        /// <summary>
        /// Không tìm thấy giao dịch yêu cầu
        /// </summary>
        public const string NotFoundTransaction = "91";

        /// <summary>
        /// Yêu cầu bị trùng lặp trong thời gian giới hạn của API (Giới hạn trong 5 phút)
        /// </summary>
        public const string RequestDuplicated = "94";

        /// <summary>
        /// Chữ ký không hợp lệ
        /// </summary>
        public const string ErrorSignature = "97";

        /// <summary>
        /// Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)
        /// </summary>
        public const string Other = "99";
    }
}
