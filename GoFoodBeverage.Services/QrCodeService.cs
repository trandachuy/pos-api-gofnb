using GoFoodBeverage.Common.AutoWire;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoFoodBeverage.Services
{
    [AutoService(typeof(IQrCodeService), Lifetime = ServiceLifetime.Scoped)]
    public class QrCodeService : IQrCodeService
    {
        private readonly IDateTimeService _dateTimeService;

        public QrCodeService(IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
        }

        public EnumQRCodeStatus GetQrCodeStatus(System.DateTime utcStartDate, System.DateTime? utcEndDate)
        {
            var nowUtcDate = _dateTimeService.NowUtc.StartOfDay().ToUniversalTime();

            if (utcStartDate > nowUtcDate)
            {
                return EnumQRCodeStatus.Scheduled;
            }
            else if (utcStartDate == nowUtcDate)
            {
                return EnumQRCodeStatus.Active;
            }
            else
            {
                if (utcEndDate.HasValue)
                {
                    return utcEndDate < nowUtcDate ? EnumQRCodeStatus.Finished : EnumQRCodeStatus.Active;
                }
                else
                {
                    return EnumQRCodeStatus.Active;
                }
            }
        }
    }
}
