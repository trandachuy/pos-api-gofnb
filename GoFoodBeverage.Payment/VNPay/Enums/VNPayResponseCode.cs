using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.VNPay.Enums
{
    /// <summary>
    /// Mã lỗi trả về từ IPN và return Url
    /// Respone from IPN and Return URL:
    /// </summary>
    public class VNPayResponseCode
    {
        /// <summary>
        /// Giao dịch thành công
        /// </summary>
        public const string Success = "00";

        /// <summary>
        /// Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).
        /// Successful subtraction. Suspected transactions (related to fraud, unusual transactions).
        /// </summary>
        public const string SuccessfulDeduction = "07";

        /// <summary>
        /// Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.
        /// </summary>
        public const string NotRegisteredInternetBanking = "09";

        /// <summary>
        /// Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần
        /// </summary>
        public const string NotVerifyCard = "10";

        /// <summary>
        /// Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.
        /// </summary>
        public const string PaymentExpired = "11";

        /// <summary>
        /// Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.
        /// </summary>
        public const string Locked = "12";

        /// <summary>
        /// Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch.
        /// </summary>
        public const string ErrorOTPPassword = "13";

        /// <summary>
        /// Giao dịch không thành công do: Khách hàng hủy giao dịch
        /// </summary>
        public const string Canceled = "24";

        /// <summary>
        /// Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.
        /// </summary>
        public const string NotHaveEnoughBalance = "51";

        /// <summary>
        /// Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.
        /// </summary>
        public const string Limited = "65";

        /// <summary>
        /// Ngân hàng thanh toán đang bảo trì.
        /// </summary>
        public const string BankingMaintenance = "75";

        /// <summary>
        /// Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch
        /// </summary>
        public const string ErrorPassword = "79";

        /// <summary>
        /// Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)
        /// </summary>
        public const string Other = "99";

    }
}
