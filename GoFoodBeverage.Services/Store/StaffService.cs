using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;

namespace GoFoodBeverage.Services.Store
{
    public class StaffService : IStaffService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IDateTimeService _dateTimeService;

        public StaffService(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _dateTimeService = dateTimeService;
        }

        public async Task EndShiftAsync(string token)
        {
            var loggedUser = _userProvider.GetLoggedUserModelFromJwt(token);

            var shift = _unitOfWork.Shifts.Find(b => b.Id == loggedUser.ShiftId).FirstOrDefault();

            if (shift != null)
            {
                shift.WithdrawalAmount = 0;
                shift.CheckOutDateTime = shift.CheckInDateTime.Value.Date.AddDays(1).AddSeconds(-1);

                _unitOfWork.Shifts.Update(shift);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task EndShiftAsync(Guid shiftId)
        {
            var shift = _unitOfWork.Shifts.Find(b => b.Id == shiftId).FirstOrDefault();
            
            if (shift != null)
            {
                shift.WithdrawalAmount = 0;
                shift.CheckOutDateTime = _dateTimeService.NowUtc;

                _unitOfWork.Shifts.Update(shift);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
