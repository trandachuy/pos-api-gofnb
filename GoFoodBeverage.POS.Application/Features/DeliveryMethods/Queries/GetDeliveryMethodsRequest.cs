using System;
using System.Linq;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.POS.Models.DeliveryMethod;

namespace GoFoodBeverage.POS.Application.Features.DeliveryMethods.Queries
{
    public class GetDeliveryMethodsRequest : IRequest<GetDeliveryMethodsResponse>
    {
        public Guid? StoreId { get; set; }
    }

    public class GetDeliveryMethodsResponse
    {
        public IEnumerable<DeliveryMethodModel> DeliveryMethods { get; set; }
    }

    public class GetDeliveryMethodsRequestHandler : IRequestHandler<GetDeliveryMethodsRequest, GetDeliveryMethodsResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetDeliveryMethodsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetDeliveryMethodsResponse> Handle(GetDeliveryMethodsRequest request, CancellationToken cancellationToken)
        {
            Guid? storeId = request.StoreId;
            if (!storeId.HasValue)
            {
                var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
                ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");
            }

            var deliveryMethods = await _unitOfWork.DeliveryMethods
                .GetAll()
                .AsNoTracking()
                .OrderBy(x => x.EnumId)
                .ProjectTo<DeliveryMethodModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetDeliveryMethodsResponse()
            {
                DeliveryMethods = deliveryMethods
            };

            return response;
        }
    }
}
