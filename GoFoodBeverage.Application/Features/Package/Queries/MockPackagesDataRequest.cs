using AutoMapper;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.MemoryCaching;

namespace GoFoodBeverage.Application.Features.Package.Queries
{
    public class MockPackagesDataRequest : IRequest<List<PackageOrderDto>>
    {
    }

    public class PackageOrderDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int TotalAmount { get; set; }

        public bool Paid { get; set; }

        public int StoreId { get; set; }

        public int UserId { get; set; }

        public string PaymentMethod { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public int SetupFee { get; set; }

        /// <summary>
        /// The bought package id
        /// </summary>
        public int BoughtPackage { get; set; }

        public int NumberMonth { get; set; }

        public string Currency { get; set; }

        public string ContractId { get; set; }

        public string Note { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }

    public class MockPackagesDataRequestHandler : IRequestHandler<MockPackagesDataRequest, List<PackageOrderDto>>
    {
        private readonly IMemoryCachingService _memoryCachingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IDateTimeService _dateTimeService;

        public MockPackagesDataRequestHandler(
            IMemoryCachingService memoryCachingService,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration,
            IDateTimeService dateTimeService
            )
        {
            _memoryCachingService = memoryCachingService;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
            _dateTimeService = dateTimeService;
        }

        public async Task<List<PackageOrderDto>> Handle(MockPackagesDataRequest request, CancellationToken cancellationToken)
        {
            var orderPackagesCached = _memoryCachingService.GetCache<List<PackageOrderDto>>("MOCKUP_ORDERPACKAGES");
            if(orderPackagesCached != null && orderPackagesCached.Count > 0)
            {
                return orderPackagesCached;
            }

            var response = new List<PackageOrderDto>();
            var random = new Random();
            int maximum = 9999999;
            int minimum = 100000;
            int minPaymentMethod = 0;
            var paymentMethodNames = new List<string>(new string[] {
                EnumPaymentMethod.MoMo.GetName(),
                EnumPaymentMethod.ZaloPay.GetName(),
                EnumPaymentMethod.CreditDebitCard.GetName(),
                EnumPaymentMethod.Cash.GetName(),
                EnumPaymentMethod.VNPay.GetName(),
            });

            var orderPackageTypes = new List<string>(new string[] {
               "ADD",
               "UPGRADE"
            });

            var orderPakageStatus = new List<string>(new string[] {
               "PENDING",
               "APPROVED"
            });

            var packages = new List<string>(new string[] {
               "POS",
               "WEB ADMIN",
               "FULL"
            });

            for (var i = 1; i < 301; i++)
            {
                var packagePrice = random.Next(minimum, maximum);
                int randomUserId = random.Next(10, 100);
                int randomPaymentMethod = random.Next(minPaymentMethod, paymentMethodNames.Count);
                int randomOrderPackageType = random.Next(0, orderPackageTypes.Count);

                var randomFee = random.Next(50000, 200000);
                var randomPackage = random.Next(0, packages.Count);
                var newPackageOrderDto = new PackageOrderDto()
                {
                    Id = i,
                    Title = $"{packages[randomPackage]}",
                    TotalAmount = packagePrice + randomFee,
                    Paid = false,
                    StoreId = 100,
                    UserId = randomUserId,
                    PaymentMethod = paymentMethodNames[randomPaymentMethod],
                    Type = orderPackageTypes[randomOrderPackageType],
                    Status = orderPakageStatus[0], // Default is PENDING
                    SetupFee = randomFee,
                    BoughtPackage = randomOrderPackageType,
                    NumberMonth = (randomPackage == 0 ? 1 : randomPackage) * 6,
                    Currency = "VND",
                    CreatedBy = "dev.gofnb@gmail.com",
                    CreatedDate = _dateTimeService.NowUtc,
                    LastModifiedBy = "dev.gofnb@gmail.com",
                    LastModifiedDate = _dateTimeService.NowUtc,
                };

                response.Add(newPackageOrderDto);
            }

            _memoryCachingService.SetCache("MOCKUP_ORDERPACKAGES", response, 30);

            return await Task.FromResult(response);
        }
    }
}
