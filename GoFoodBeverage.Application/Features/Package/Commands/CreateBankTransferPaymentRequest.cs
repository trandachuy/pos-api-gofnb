using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Package;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Package.Commands
{
    public class CreateBankTransferPaymentRequest : IRequest<CreateBankTransferPaymentResponse>
    {
        public Guid PackageId { get; set; }

        public int PackageDurationByMonth { get; set; }
    }

    public class CreateBankTransferPaymentResponse
    {
        public CreateOrderPackagePaymentResponseModel PackageBankTransfer { get; set; }
    }

    public class CreateBankTransferPaymentHandler : IRequestHandler<CreateBankTransferPaymentRequest, CreateBankTransferPaymentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public CreateBankTransferPaymentHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<CreateBankTransferPaymentResponse> Handle(CreateBankTransferPaymentRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var store = await _unitOfWork.Stores.GetStoreByIdWithoutTrackingAsync(loggedUser.StoreId);
            var package = await _unitOfWork.Packages
                .Find(x => x.Id == request.PackageId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            ThrowError.BadRequestAgainstNull(package, "Cannot find package");

            var account = await _unitOfWork.Accounts
                .Find(a => a.Id == loggedUser.AccountId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.BadRequestAgainstNull(account, "Cannot find user account information");

            var accountTransfer = await _unitOfWork.AccountTransfers
                .GetAll()
                .AsNoTracking()
                .ProjectTo<AccountTransferModel>(_mapperConfiguration)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.BadRequestAgainstNull(accountTransfer, "Cannot find account transfer information");

            var taxAmount = (package.CostPerMonth.Value * request.PackageDurationByMonth * package.Tax) / 100;
            var packageOrderModelAdd = new OrderPackage
            {
                Title = $"{package.Name}",
                PackageId = request.PackageId,
                StoreCode = store.Code,
                AccountCode = account.Code,
                PackageDurationByMonth = request.PackageDurationByMonth,
                TotalAmount = CalculateTotalAmount(package, request, taxAmount),
                PackageOderPaymentStatus = EnumOrderPaymentStatus.Unpaid,
                PackagePaymentMethod = EnumPackagePaymentMethod.BankTransfer,
                Status = EnumOrderPackageStatus.PENDING.GetName(),
                StoreId = store.Id,
                Email = loggedUser.Email,
                ShopPhoneNumber = loggedUser.PhoneNumber,
                SellerName = loggedUser.FullName,
                ShopName = store.Title,
                OrderPackageType = EnumOrderPackageType.StoreActivate,
                TaxAmount = taxAmount,
                IsActivated = false
            };

            var orderPackage = await _unitOfWork.OrderPackages.AddAsync(packageOrderModelAdd);
            
            var reponse = new CreateOrderPackagePaymentResponseModel
            {
                PackageCode = orderPackage.Code,
                PackageName = orderPackage.Title,
                Duration = $"{orderPackage.PackageDurationByMonth}",
                TotalAmount = orderPackage.TotalAmount,
                PaymentMethod = EnumPackagePaymentMethod.BankTransfer.GetName(),
                PackagePaymentMethodId = EnumPackagePaymentMethod.BankTransfer,
                AccountTransfer = accountTransfer
            };

            return new CreateBankTransferPaymentResponse() { PackageBankTransfer = reponse };
        }

        private static decimal CalculateTotalAmount(Domain.Entities.Package package, CreateBankTransferPaymentRequest request, decimal taxAmount)
        {
            var totalAmount = package.CostPerMonth.Value * request.PackageDurationByMonth + taxAmount;

            return totalAmount;
        }
    }
}
