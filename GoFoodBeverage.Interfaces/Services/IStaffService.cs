using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces
{
    public interface IStaffService
    {
        Task EndShiftAsync(string token);

        Task EndShiftAsync(Guid shiftId);
    }
}
