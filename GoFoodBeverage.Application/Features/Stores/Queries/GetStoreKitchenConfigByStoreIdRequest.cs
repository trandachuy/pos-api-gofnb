using AutoMapper;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetStoreKitchenConfigByStoreIdRequest : IRequest<GetStoreKitchenConfigByStoreIdResponse>
    {
    }

    public class GetStoreKitchenConfigByStoreIdResponse
    {
        public bool IsStoreHasKitchen { get; set; }

        public bool IsAutoPrintStamp { get; set; }
    }

    public class GetStoreKitchenConfigByStoreIdRequestHandler : IRequestHandler<GetStoreKitchenConfigByStoreIdRequest, GetStoreKitchenConfigByStoreIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetStoreKitchenConfigByStoreIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetStoreKitchenConfigByStoreIdResponse> Handle(GetStoreKitchenConfigByStoreIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var storeKitchenSetting = await _unitOfWork.Stores.GetStoreKitchenConfigAsync(loggedUser.StoreId.Value);

            return new GetStoreKitchenConfigByStoreIdResponse()
            {
                IsStoreHasKitchen = storeKitchenSetting.IsStoreHasKitchen,
                IsAutoPrintStamp = storeKitchenSetting.IsAutoPrintStamp,
            };
        }
    }
}
