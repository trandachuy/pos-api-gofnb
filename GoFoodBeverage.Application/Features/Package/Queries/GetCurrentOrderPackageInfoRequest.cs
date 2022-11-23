using AutoMapper;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Package.Queries
{
    public class GetCurrentOrderPackageInfoRequest : IRequest<GetCurrentOrderPackageInfoResponse>
    {
    }

    public class GetCurrentOrderPackageInfoResponse
    {
        public OrderPackageInfoDto OrderPackageInfo { get; set; }

        public LastOrderPackageDto LastOrderPackage { get; set; }

        public class OrderPackageInfoDto
        {
            public Guid OrderPackageId { get; set; }

            public string PackageName { get; set; }

            public DateTime ActivatedDate { get; set; }

            public int AvailableBranchNumber { get; set; }

            public int? CostPerMonth { get; set; }

            public DateTime? ExpiredDate { get; set; }

            public DateTime? StartDate { get; set; }

            public decimal Tax { get; set; }

            public decimal TotalAmount { get; set; }

            public int PackageDurationByMonth { get; set; }

            public decimal PackagePricePerDay { get; set; }
        }

        public class LastOrderPackageDto
        {
            public int BranchQuantity { get; set; }

            public decimal RemainAmount { get; set; }

            public DateTime? ExpiredDate { get; set; }
        }
    }

    public class GetCurrentOrderPackageInfoRequestHandler : IRequestHandler<GetCurrentOrderPackageInfoRequest, GetCurrentOrderPackageInfoResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IDateTimeService _dateTimeService;

        public GetCurrentOrderPackageInfoRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            MapperConfiguration mapperConfiguration,
            IDateTimeService dateTimeService
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
            _dateTimeService = dateTimeService;
        }

        public async Task<GetCurrentOrderPackageInfoResponse> Handle(GetCurrentOrderPackageInfoRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var today = _dateTimeService.NowUtc;
            var store = await _unitOfWork.Stores.GetStoreByIdWithoutTrackingAsync(loggedUser.StoreId);
            var orderPackage = await _unitOfWork.OrderPackages
              .GetAll()
              .AsNoTracking()
              .Where(op => op.StoreId == loggedUser.StoreId &&
                           op.Id == store.ActivatedByOrderPackageId &&
                           op.IsActivated == true)
              .Include(op => op.Package)
              .Select(op => new GetCurrentOrderPackageInfoResponse.OrderPackageInfoDto
              {
                  OrderPackageId = op.Id,
                  PackageName = "store.branch",
                  ActivatedDate = op.LastModifiedDate,
                  AvailableBranchNumber = op.Package.AvailableBranchNumber,
                  CostPerMonth = op.Package.CostPerMonth,
                  StartDate = op.LastModifiedDate,
                  ExpiredDate = op.ExpiredDate,
                  Tax = op.Package.Tax,
                  TotalAmount = op.TotalAmount,
                  PackageDurationByMonth = op.PackageDurationByMonth,
                  PackagePricePerDay = op.Package.CostPerMonth.Value * DefaultConstants.MONTHS_OF_YEAR / DefaultConstants.DAYS_OF_YEAR
              })
              .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var lastOrderPackage = await _unitOfWork.OrderPackages
                 .GetAll()
                 .AsNoTracking()
                 .Where(op => op.StoreId == store.Id &&
                              op.OrderPackageType == EnumOrderPackageType.BranchPurchase &&
                              op.Status == EnumOrderPackageStatus.APPROVED.GetName() &&
                              op.IsActivated == true &&
                              op.ExpiredDate >= today)
                 .OrderByDescending(op => op.LastModifiedDate)
                 .Select(op => new { 
                    Quantity = op.BranchQuantity,
                    StartDate = op.LastModifiedDate,
                    op.BranchUnitPricePerYear,
                    op.ExpiredDate
                 })
                 .FirstOrDefaultAsync();

            var timeSpan = orderPackage.ExpiredDate.Value - today;
            var branchUnitPricePerDay = (orderPackage.CostPerMonth.Value * DefaultConstants.MONTHS_OF_YEAR) / DefaultConstants.DAYS_OF_YEAR;
            var response = new GetCurrentOrderPackageInfoResponse()
            {
                OrderPackageInfo = orderPackage,
                LastOrderPackage = new GetCurrentOrderPackageInfoResponse.LastOrderPackageDto()
                {
                    BranchQuantity = orderPackage.AvailableBranchNumber,
                    ExpiredDate = orderPackage.ExpiredDate,
                    RemainAmount = timeSpan.Days * branchUnitPricePerDay * lastOrderPackage?.Quantity ?? 0
                }
            };

            if (lastOrderPackage != null)
            {
                timeSpan = lastOrderPackage.ExpiredDate.Value - today;

                response.LastOrderPackage.RemainAmount = timeSpan.Days * branchUnitPricePerDay * lastOrderPackage.Quantity;
                response.LastOrderPackage.BranchQuantity = lastOrderPackage.Quantity + orderPackage.AvailableBranchNumber;
                response.LastOrderPackage.ExpiredDate = lastOrderPackage.ExpiredDate;
            }

            return response;
        }
    }
}
