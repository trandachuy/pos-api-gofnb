using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Customer.Queries
{
    public class GetLoyaltyPointByStoreIdRequest: IRequest<GetLoyaltyPointByStoreIdResponse>
    {

    }
    public class GetLoyaltyPointByStoreIdResponse
    {
        public LoyaltyPointConfig Configuration { get; set; }

        public bool HasData { get; set; }

    }

    public class GetLoyaltyPointByStoreIdHandler : IRequestHandler<GetLoyaltyPointByStoreIdRequest, GetLoyaltyPointByStoreIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;

        public GetLoyaltyPointByStoreIdHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetLoyaltyPointByStoreIdResponse> Handle(GetLoyaltyPointByStoreIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var configuration = await _unitOfWork.LoyaltyPointsConfigs.GetLoyaltyPointConfigByStoreIdAsync(loggedUser.StoreId.Value).FirstOrDefaultAsync(cancellationToken);

            return new GetLoyaltyPointByStoreIdResponse()
            {
                Configuration = configuration,
                HasData = configuration != null
            };
        }
    }
}
