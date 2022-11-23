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
    public class GetInitialAmountFromEndShiftByBranchIdRequest : IRequest<GetInitialAmountFromEndShiftByBranchIdResponse>
    {
        public Guid BranchId { get; set; }
    }

    public class GetInitialAmountFromEndShiftByBranchIdResponse
    {
        public decimal InitialAmount { get; set; }
    }

    public class GetInitialAmountFromEndShiftByBranchIdRequestHandler : IRequestHandler<GetInitialAmountFromEndShiftByBranchIdRequest, GetInitialAmountFromEndShiftByBranchIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public GetInitialAmountFromEndShiftByBranchIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetInitialAmountFromEndShiftByBranchIdResponse> Handle(GetInitialAmountFromEndShiftByBranchIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var staff = await _unitOfWork.Staffs.GetStaffByAccountId(loggedUser.AccountId.Value).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(staff == null, "Cannot find staff information");

            var branch = await _unitOfWork.StoreBranches
                .Find(b => b.StoreId == loggedUser.StoreId && b.Id == request.BranchId && !b.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(branch == null, "Cannot find branch information");

            var shifts = await _unitOfWork.Shifts.Find(s => s.StoreId == loggedUser.StoreId && s.BranchId == request.BranchId).ToListAsync(cancellationToken: cancellationToken);
            var shift = shifts.OrderByDescending(s => s.CheckOutDateTime).FirstOrDefault();

            decimal initialAmount = 0;
            if (shift != null)
            {
                var listOrderbyShifId = _unitOfWork.Orders.Find(x => x.StoreId == loggedUser.StoreId && x.ShiftId == shift.Id);
                var revenue = listOrderbyShifId.Where(x => x.StatusId == EnumOrderStatus.Completed).Sum(x => x.OriginalPrice);
                var totalDiscountAmount = listOrderbyShifId.Where(x => x.StatusId == EnumOrderStatus.Completed).Sum(x => x.TotalDiscountAmount);

                initialAmount = (shift.InitialAmount + revenue - totalDiscountAmount - shift.WithdrawalAmount);
            }
            
            var response = new GetInitialAmountFromEndShiftByBranchIdResponse()
            {
                InitialAmount = initialAmount
            };

            return response;
        }
    }
}
