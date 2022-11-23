using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Shifts.Queries
{
    public class GetInfoShiftByIdRequest : IRequest<GetInfoShiftByIdResponse>
    {
        public Guid ShiftId { get; set; }
    }

    public class GetInfoShiftByIdResponse
    {
        public Guid ShiftId { get; set; }

        public string CodeStaff { get; set; }

        public string NameStaff { get; set; }

        public DateTime? CheckInDateTime { get; set; }

        public DateTime? CheckOutDateTime { get; set; }

        public int NumberOrderSuccess { get; set; }

        public int NumberOrderCanceled { get; set; }

        public decimal InitialAmount { get; set; }

        public decimal Revenue { get; set; }

        public decimal Discount { get; set; }

        public decimal WithdrawalAmount { get; set; }

        public decimal Remain { get; set; }
    }

    public class GetInfoShiftByIdRequestHandler : IRequestHandler<GetInfoShiftByIdRequest, GetInfoShiftByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public GetInfoShiftByIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetInfoShiftByIdResponse> Handle(GetInfoShiftByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var shift = await _unitOfWork.Shifts.Find(s => s.StoreId == loggedUser.StoreId && s.Id == request.ShiftId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            var staff = await _unitOfWork.Staffs.GetStaffByShift(shift, loggedUser.StoreId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            var listOrderbyShifId = _unitOfWork.Orders.Find(x => x.StoreId == loggedUser.StoreId && x.ShiftId == shift.Id);

            var response = new GetInfoShiftByIdResponse()
            {
                ShiftId = shift.Id,
                CodeStaff = staff?.Code,
                NameStaff = staff?.FullName,
                NumberOrderSuccess = listOrderbyShifId.Count(x => x.StatusId == EnumOrderStatus.Completed),
                NumberOrderCanceled = listOrderbyShifId.Count(x => x.StatusId == EnumOrderStatus.Canceled),
                CheckInDateTime = shift.CheckInDateTime,
                CheckOutDateTime = shift.CheckOutDateTime,
                InitialAmount = shift.InitialAmount,
                Revenue = listOrderbyShifId.Where(x => x.StatusId == EnumOrderStatus.Completed).Sum(x=>x.OriginalPrice),
                Discount = listOrderbyShifId.Where(x => x.StatusId == EnumOrderStatus.Completed).Sum(x => x.TotalDiscountAmount),
                WithdrawalAmount = shift.WithdrawalAmount
            };
            response.Remain = shift.InitialAmount + response.Revenue - response.Discount - shift.WithdrawalAmount;
            return response;
        }
    }
}
