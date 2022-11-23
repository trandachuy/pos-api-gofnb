using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Package;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Package.Commands
{
    public class CreateBranchPurchaseOrderPackageRequest : IRequest<CreateBranchPurchaseOrderPackageResponse>
    {
        public EnumPackagePaymentMethod PaymentMethod { get; set; }

        public Guid? ActivateStorePackageId { get; set; }

        public string PackageName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime? BranchExpiredDate { get; set; }

        public decimal BranchPurchaseTotalPrice { get; set; }

        public decimal RemainAmount { get; set; }

        public decimal TaxAmount { get; set; }
    }

    public class CreateBranchPurchaseOrderPackageResponse
    {
        public AccountTransferModel AccountBankTransfer { get; set; }

        public PackageInfoResponseModel PackageInfo { get; set; }

        public class PackageInfoResponseModel
        {
            public int PackageCode { get; set; }

            public string PackageName { get; set; }

            public decimal TotalAmount { get; set; }

            public int Duration { get;set; }

            public EnumPackagePaymentMethod PackagePaymentMethod { get; set; }

            public string PaymentMethodName { get; set; }
        }
    }

    public class CreateBranchPurchaseOrderPackageRequestHandler : IRequestHandler<CreateBranchPurchaseOrderPackageRequest, CreateBranchPurchaseOrderPackageResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IDateTimeService _dateTimeService;

        public CreateBranchPurchaseOrderPackageRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration,
            IDateTimeService dateTimeService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
            _dateTimeService = dateTimeService;
        }

        public async Task<CreateBranchPurchaseOrderPackageResponse> Handle(CreateBranchPurchaseOrderPackageRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdWithoutTrackingAsync(loggedUser.StoreId);
            var activatePackage = await _unitOfWork.OrderPackages
                .Find(x => x.StoreId == loggedUser.StoreId && x.Id == request.ActivateStorePackageId && x.IsActivated == true)
                .AsNoTracking()
                .Include(op => op.Package)
                .Select(op => new {
                   op.Package.Id,
                   op.Package.Name,
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var accountCode = await _unitOfWork.Accounts
                .Find(a => a.Id == loggedUser.AccountId)
                .AsNoTracking()
                .Select(a => a.Code)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var response = new CreateBranchPurchaseOrderPackageResponse();
            switch (request.PaymentMethod)
            {
                case EnumPackagePaymentMethod.BankTransfer:
                    response.AccountBankTransfer = await _unitOfWork.AccountTransfers
                                        .GetAll()
                                        .AsNoTracking()
                                        .ProjectTo<AccountTransferModel>(_mapperConfiguration)
                                        .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                    var totalAmount = request.BranchPurchaseTotalPrice + request.TaxAmount - request.RemainAmount;
                    var branchExpiredDateUtc = TimeZoneInfo.ConvertTimeToUtc(request.BranchExpiredDate.Value);
                    var today = _dateTimeService.NowUtc;
                    decimal packageDurationByMonth = branchExpiredDateUtc.Subtract(today).Days / (DefaultConstants.DAYS_OF_YEAR / DefaultConstants.MONTHS_OF_YEAR);

                    using (var orderPackageTransaction = await _unitOfWork.BeginTransactionAsync())
                    {
                        try
                        {
                            /// Create branch purchase order package
                            var packageOrder = new OrderPackage
                            {
                                Title = $"{activatePackage.Name} - {request.PackageName}",
                                PackageId = activatePackage.Id,
                                StoreCode = store.Code,
                                AccountCode = accountCode,
                                PackageDurationByMonth = (int)Math.Round(packageDurationByMonth),
                                TotalAmount = totalAmount,
                                PackageOderPaymentStatus = EnumOrderPaymentStatus.Unpaid,
                                PackagePaymentMethod = EnumPackagePaymentMethod.BankTransfer,
                                Status = EnumOrderPackageStatus.PENDING.GetName(),
                                StoreId = store.Id,
                                Email = loggedUser.Email,
                                ShopPhoneNumber = loggedUser.PhoneNumber,
                                SellerName = loggedUser.FullName,
                                ShopName = store.Title,
                                OrderPackageType = EnumOrderPackageType.BranchPurchase,
                                ActivateStorePackageId = request.ActivateStorePackageId,
                                BranchQuantity = request.Quantity,
                                BranchUnitPricePerYear = request.UnitPrice * DefaultConstants.MONTHS_OF_YEAR,
                                BranchPurchaseTotalPrice = request.BranchPurchaseTotalPrice,
                                BranchPurchaseRemainAmount = request.RemainAmount,
                                TaxAmount = request.TaxAmount,
                                IsActivated = false
                            };

                            var orderPackage = await _unitOfWork.OrderPackages.AddAsync(packageOrder);
                            response.PackageInfo = new CreateBranchPurchaseOrderPackageResponse.PackageInfoResponseModel()
                            {
                                PackageCode = orderPackage.Code,
                                PackageName = orderPackage.Title,
                                TotalAmount = orderPackage.TotalAmount,
                                PackagePaymentMethod = request.PaymentMethod,
                                Duration = orderPackage.PackageDurationByMonth,
                                PaymentMethodName = EnumPackagePaymentMethod.BankTransfer.GetName()
                            };

                            await orderPackageTransaction.CommitAsync(cancellationToken);
                        }
                        catch
                        {
                            await orderPackageTransaction.RollbackAsync(cancellationToken);
                        }
                    }

                    break;

                case EnumPackagePaymentMethod.ATM:
                    /// Handle create branch purchase order package with payment method ATM
                    break;
                case EnumPackagePaymentMethod.Visa:
                    /// Handle create branch purchase order package with payment method Visa
                    break;
            }

            return response;
        }
    }
}
