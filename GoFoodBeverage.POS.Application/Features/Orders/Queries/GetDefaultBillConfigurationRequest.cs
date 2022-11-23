using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Bill;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Orders.Queries
{
    public class GetDefaultBillConfigurationRequest : IRequest<GetDefaultBillConfigurationResponse>
    {
    }

    public class GetDefaultBillConfigurationResponse
    {
        public BillModel BillConfiguration { get; set; }
    }

    public class GetDefaultBillConfigurationRequestHandler : IRequestHandler<GetDefaultBillConfigurationRequest, GetDefaultBillConfigurationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetDefaultBillConfigurationRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetDefaultBillConfigurationResponse> Handle(GetDefaultBillConfigurationRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var billConfiguration = await _unitOfWork.Bills.GetDefaultBillConfigurationByStore(loggedUser.StoreId.Value)
                .AsNoTracking()
                .ProjectTo<BillModel>(_mapperConfiguration)
                .FirstOrDefaultAsync(cancellationToken);

            if (billConfiguration == null)
            {
                ThrowError.Against(billConfiguration==null, "Please set up receipt configuration first");
            }
            return new GetDefaultBillConfigurationResponse
            {
                BillConfiguration = billConfiguration
            };
        }
    }
}
