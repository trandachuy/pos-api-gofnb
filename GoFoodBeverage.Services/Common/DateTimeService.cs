using GoFoodBeverage.Interfaces;
using System;

namespace GoFoodBeverage.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}
