using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.POS.Application.Features.Shifts.Commands
{
    public class GetInfoEndShiftByBranchIdRequest : IRequest<GetInfoEndShiftByBranchIdResponse>
    {
    }

    public class GetInfoEndShiftByBranchIdResponse
    {
        public string Code { get; set; }

        public Guid ShiftId { get; set; }

        public string CodeStaff { get; set; }

        public string NameStaff { get; set; }

        public DateTime? CheckInDateTime { get; set; }

        public int NumberOrderSuccess { get; set; }

        public int NumberOrderCanceled { get; set; }

        public decimal InitialAmount { get; set; }

        public decimal Revenue { get; set; }

        public decimal Discount { get; set; }

        public decimal WithdrawalAmount { get; set; }

        public decimal Remain { get; set; }
    }

    public class GetInfoEndShiftByBranchIdRequestHandler : IRequestHandler<GetInfoEndShiftByBranchIdRequest, GetInfoEndShiftByBranchIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public GetInfoEndShiftByBranchIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetInfoEndShiftByBranchIdResponse> Handle(GetInfoEndShiftByBranchIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var staff = await _unitOfWork.Staffs.GetStaffByAccountId(loggedUser.AccountId.Value).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(staff == null, "Cannot find staff information");

            var branch = await _unitOfWork.StoreBranches
                .Find(b => b.StoreId == loggedUser.StoreId && b.Id == loggedUser.BranchId.Value && !b.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(branch == null, "Cannot find branch information");

            var shifts = await _unitOfWork.Shifts
                .Find(s => s.StoreId == loggedUser.StoreId && s.BranchId == loggedUser.BranchId.Value && s.StaffId == staff.Id)
                .ToListAsync(cancellationToken: cancellationToken);
            var shiftNeedCheckOut = shifts.Where(s => s.CheckOutDateTime == null).OrderByDescending(s => s.CheckOutDateTime).FirstOrDefault();
            var listOrderbyShifId = _unitOfWork.Orders.Find(x => x.StoreId == loggedUser.StoreId && x.ShiftId == shiftNeedCheckOut.Id);
            var numberOrderSuccess = listOrderbyShifId.Count(x => x.StatusId == EnumOrderStatus.Completed);
            var numberOrderCanceled = listOrderbyShifId.Count(x => x.StatusId == EnumOrderStatus.Canceled);
            var revenue = listOrderbyShifId.Where(x => x.StatusId == EnumOrderStatus.Completed).Sum(x => x.OriginalPrice + x.TotalFee + x.TotalTax);
            var discount = listOrderbyShifId.Where(x => x.StatusId == EnumOrderStatus.Completed).Sum(x => x.TotalDiscountAmount);
            var response = new GetInfoEndShiftByBranchIdResponse()
            {
                ShiftId = shiftNeedCheckOut.Id,
                Code = shiftNeedCheckOut.Code,
                CodeStaff = staff.Code,
                NameStaff = staff.FullName,
                NumberOrderSuccess = numberOrderSuccess,
                NumberOrderCanceled = numberOrderCanceled,
                CheckInDateTime = shiftNeedCheckOut.CheckInDateTime,
                InitialAmount = shiftNeedCheckOut.InitialAmount,
                Revenue = revenue,
                Discount = discount,
                WithdrawalAmount = shiftNeedCheckOut.WithdrawalAmount
            };
            response.Remain = shiftNeedCheckOut.InitialAmount + response.Revenue - response.Discount - shiftNeedCheckOut.WithdrawalAmount;

            return response;
        }
    }
}
