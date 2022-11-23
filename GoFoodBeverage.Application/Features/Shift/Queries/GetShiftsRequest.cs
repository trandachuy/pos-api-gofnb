using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using System.Linq;
using System;
using GoFoodBeverage.Models.Shift;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Shift.Queries
{
    public class GetShiftsRequest : IRequest<GetShiftsResponse>
    {
        public string Date { get; set; }

        public Guid? BranchId { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class GetShiftsResponse
    {
        public ShiftModel Shift { get; set; }

        public int Total { get; set; }
    }

    public class GetShiftsHandler : IRequestHandler<GetShiftsRequest, GetShiftsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _iDateTimeService;

        public GetShiftsHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            IDateTimeService iDateTimeService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _iDateTimeService = iDateTimeService;
        }

        public async Task<GetShiftsResponse> Handle(GetShiftsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            var result = new ShiftModel();
            var shiftsbyCurrentDate = new List<Domain.Entities.Shift>();
            var shiftsbyPrevDate = new List<Domain.Entities.Shift>();
            var paramDate = DateTime.Parse(request.Date);

            if (request.BranchId == null)
            {
                shiftsbyCurrentDate = await _unitOfWork.Shifts.Find(x => x.StoreId == loggedUser.StoreId && x.CheckInDateTime.Value.Date == paramDate.Date).ToListAsync(cancellationToken: cancellationToken);
                shiftsbyPrevDate = await _unitOfWork.Shifts.Find(x => x.StoreId == loggedUser.StoreId && x.CheckInDateTime.Value.Date == paramDate.Date.AddDays(-1)).ToListAsync(cancellationToken: cancellationToken);
            }
            else
            {
                shiftsbyCurrentDate = await _unitOfWork.Shifts.Find(x => x.StoreId == loggedUser.StoreId && x.CheckInDateTime.Value.Date == paramDate.Date && x.BranchId == request.BranchId).ToListAsync(cancellationToken: cancellationToken);
                shiftsbyPrevDate = await _unitOfWork.Shifts.Find(x => x.StoreId == loggedUser.StoreId && x.CheckInDateTime.Value.Date == paramDate.Date.AddDays(-1) && x.BranchId == request.BranchId).ToListAsync(cancellationToken: cancellationToken);
            }

            var listShiftIdsByCurrentDate = shiftsbyCurrentDate.Select(x => x.Id).ToList();
            var listOrderbyCurrentDate = _unitOfWork.Orders.Find(x => x.StoreId == loggedUser.StoreId && listShiftIdsByCurrentDate.Contains(x.ShiftId.Value) && x.StatusId == EnumOrderStatus.Completed).Include(x => x.OrderItems).ToList();
            var listShiftIdsByPrevDate = shiftsbyPrevDate.Select(x => x.Id).ToList();
            var listOrderbyPrevDate = _unitOfWork.Orders.Find(x => x.StoreId == loggedUser.StoreId && listShiftIdsByPrevDate.Contains(x.ShiftId.Value) && x.StatusId == EnumOrderStatus.Completed).Include(x => x.OrderItems).ToList();

            //Shift Info
            result.OrderValue = listOrderbyCurrentDate.Count;
            var countOrderPrev = listOrderbyPrevDate.Count;
            var (OrderPercent, OrderIncrease) = CalculateUpDown(countOrderPrev, result.OrderValue);
            result.OrderPercent = OrderPercent;
            result.OrderIncrease = OrderIncrease;

            //Sold Product Info
            var listProductPriceIdsCurrent = new List<Guid?>();
            foreach (var order in listOrderbyCurrentDate)
            {
                var productPriceIds = order.OrderItems.Select(x => x.ProductPriceId);
                listProductPriceIdsCurrent.AddRange(productPriceIds);
            }

            listProductPriceIdsCurrent = listProductPriceIdsCurrent.Distinct().ToList();
            var listProductPricesCurrent = _unitOfWork.ProductPrices.Find(x => x.StoreId == loggedUser.StoreId && listProductPriceIdsCurrent.Contains(x.Id));
            var listProductCurrent = listProductPricesCurrent.Select(x => x.ProductId).Distinct();
            result.SoldProductValue = listProductCurrent.Count();

            var listProductPriceIdsPrev = new List<Guid?>();
            foreach (var order in listOrderbyPrevDate)
            {
                var productPriceIds = order.OrderItems.Select(x => x.ProductPriceId);
                listProductPriceIdsPrev.AddRange(productPriceIds);
            }

            listProductPriceIdsPrev = listProductPriceIdsPrev.Distinct().ToList();
            var listProductPricesPrev = _unitOfWork.ProductPrices.Find(x => x.StoreId == loggedUser.StoreId && listProductPriceIdsPrev.Contains(x.Id));
            var listProductPrev = listProductPricesPrev.Select(x => x.ProductId).Distinct();
            var soldProductValuePrev = listProductPrev.Count();

            var (SoldProductPercent, SoldProductIncrease) = CalculateUpDown(soldProductValuePrev, result.SoldProductValue);
            result.SoldProductPercent = SoldProductPercent;
            result.SoldProductIncrease = SoldProductIncrease;

            //Total Discount Info
            result.TotalDiscountValue = listOrderbyCurrentDate.Sum(x => x.TotalDiscountAmount);
            var TotalDiscountPrev = listOrderbyPrevDate.Sum(x => x.TotalDiscountAmount);
            var (TotalDiscountPercent, TotalDiscountIncrease) = CalculateUpDown((double)TotalDiscountPrev, (double)result.TotalDiscountValue);
            result.TotalDiscountPercent = TotalDiscountPercent;
            result.TotalDiscountIncrease = TotalDiscountIncrease;

            //Revenue Info
            result.RevenueValue = listOrderbyCurrentDate.Sum(x => x.OriginalPrice);
            var TotalRevenuePrev = listOrderbyPrevDate.Sum(x => x.OriginalPrice);

            var (RevenuePercent, RevenueIncrease) = CalculateUpDown((double)TotalRevenuePrev, (double)result.RevenueValue);
            result.RevenuePercent = RevenuePercent;
            result.RevenueIncrease = RevenueIncrease;

            // List Shift Info
            result.ShiftTableModels = new List<ShiftTableModel>();
            var listStaffIds = shiftsbyCurrentDate.Select(sh => sh.StaffId.Value);
            var listStaff = await _unitOfWork.Staffs.GetAllStaffByListStaff(listStaffIds, loggedUser.StoreId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            foreach (var shift in shiftsbyCurrentDate)
            {
                var staff = listStaff.FirstOrDefault(x => x.Id == shift.StaffId);
                if (staff == null) continue;

                var shiftRecord = new ShiftTableModel
                {
                    ShiftId = shift.Id,
                    StaffName = staff?.FullName,
                    CheckIn = shift.CheckInDateTime == null ? string.Empty : string.Format("{0:00}:{1:00}", shift.CheckInDateTime.Value.Hour, shift.CheckInDateTime.Value.Minute),
                    CheckOut = shift.CheckOutDateTime == null ? string.Empty : string.Format("{0:00}:{1:00}", shift.CheckOutDateTime.Value.Hour, shift.CheckOutDateTime.Value.Minute),
                    InitialAmount = shift.InitialAmount,
                    WithdrawalAmount = shift.WithdrawalAmount
                };

                var listOrderbyShifId = listOrderbyCurrentDate.Where(x => x.ShiftId == shift.Id);
                shiftRecord.Revenue = listOrderbyShifId.Sum(x => x.OriginalPrice);
                shiftRecord.Discount = listOrderbyShifId.Sum(x => x.TotalDiscountAmount);
                shiftRecord.Remain = shiftRecord.InitialAmount + shiftRecord.Revenue - shiftRecord.Discount - shiftRecord.WithdrawalAmount;
                shiftRecord.MoMo = listOrderbyShifId.Where(x => x.PaymentMethodId == EnumPaymentMethod.MoMo).Sum(x => x.PriceAfterDiscount);
                shiftRecord.Cash = listOrderbyShifId.Where(x => x.PaymentMethodId == EnumPaymentMethod.Cash).Sum(x => x.PriceAfterDiscount);
                shiftRecord.ATM = listOrderbyShifId.Where(x => x.PaymentMethodId == EnumPaymentMethod.CreditDebitCard).Sum(x => x.PriceAfterDiscount);
                shiftRecord.CancleOrderAmount = listOrderbyCurrentDate.Count(x => x.StatusId == EnumOrderStatus.Canceled && x.ShiftId == shift.Id);
                result.ShiftTableModels.Add(shiftRecord);
            }

            var countShiftTableModels = result.ShiftTableModels.Count;
            var shifts = result.ShiftTableModels.OrderByDescending(p => p.CheckIn).ToList();
            var shiftTableModels = shifts.ToPagination(request.PageNumber, request.PageSize);
            result.ShiftTableModels = shiftTableModels.Result.ToList();
            result.ShiftTableModels.ForEach(c =>
            {
                c.No = result.ShiftTableModels.IndexOf(c) + ((request.PageNumber - 1) * request.PageSize) + 1;
            });

            var response = new GetShiftsResponse()
            {
                Shift = result,
                Total = countShiftTableModels
            };

            return response;
        }

        public (double, bool) CalculateUpDown(double prev, double current)
        {
            if (prev == 0)
            {
                if (current == 0)
                {
                    return (0, true);
                }
                else
                {
                    return (100, true);
                }
            }
            else
            {
                var percent = Math.Round((100 * current) / (double)prev) - 100;
                if (percent == 0)
                {
                    return (Math.Abs(percent), true);
                }
                else
                {
                    return (Math.Abs(percent), percent >= 0);
                }
            }
        }
    }
}
