using System;

namespace GoFoodBeverage.Interfaces
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
    }
}
