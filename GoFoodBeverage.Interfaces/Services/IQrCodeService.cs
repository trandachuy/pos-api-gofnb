using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Interfaces.Services
{
    public interface IQrCodeService
    {
        /// <summary>
        /// Get QR code status by start date and end date.
        /// </summary>
        /// <param name="utcStartDate"></param>
        /// <param name="utcEndDate"></param>
        /// <returns>
        /// <list>
        /// <item>
        /// If the start date > today => 1: Scheduled
        /// </item>
        /// <item>
        /// If the start date = today => 2: Active
        /// </item>
        /// <item>
        /// If the end date not NULL and today > end date => 3: Finished else => 2: Active
        /// </item>
        /// </list>
        /// </returns>
        EnumQRCodeStatus GetQrCodeStatus(System.DateTime utcStartDate, System.DateTime? utcEndDate);
    }
}
