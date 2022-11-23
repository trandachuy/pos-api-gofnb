using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.VNPay.Enums
{
    public class VNPayRefundResponseCode
    {
        /// <summary>
        /// Tổng số tiền hoản trả lớn hơn số tiền gốc
        /// </summary>
        public const string WrongRefundAmount = "02";

        /// <summary>
        /// Dữ liệu gửi sang không đúng định dạng
        /// </summary>
        public const string ErrorDataFormat = "03";

        /// <summary>
        /// Không cho phép hoàn trả toàn phần sau khi hoàn trả một phần
        /// </summary>
        public const string ErrorRefundPartial = "04";

        /// <summary>
        /// Chỉ cho phép hoàn trả một phần
        /// </summary>
        public const string PartialRefundAllowed = "13";

        /// <summary>
        /// Không tìm thấy giao dịch yêu cầu hoàn trả
        /// </summary>
        public const string NotFoundTransaction = "91";

        /// <summary>
        /// Số tiền hoàn trả không hợp lệ. Số tiền hoàn trả phải nhỏ hơn hoặc bằng số tiền thanh toán.
        /// </summary>
        public const string InvalidAmount = "93";

        /// <summary>
        /// Yêu cầu bị trùng lặp trong thời gian giới hạn của API (Giới hạn trong 5 phút)
        /// </summary>
        public const string RequestDuplicated = "94";

        /// <summary>
        /// Giao dịch này không thành công bên VNPAY. VNPAY từ chối xử lý yêu cầu.
        /// </summary>
        public const string Rejected = "95";

        /// <summary>
        /// Chữ ký không hợp lệ
        /// </summary>
        public const string ErrorSignature = "97";

        /// <summary>
        /// Timeout Exception
        /// </summary>
        public const string TimeoutException = "98";

        /// <summary>
        /// Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)
        /// </summary>
        public const string Other = "99";
    }
}
