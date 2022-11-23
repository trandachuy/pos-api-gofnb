using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.DeliveryMethod;

namespace GoFoodBeverage.Application.Features.DeliveryMethods.Queries
{
    public class GetDeliveryMethodsByStoreIdRequest : IRequest<GetDeliveryMethodsByStoreIdResponse>
    {
        public Guid StoreId { get; set; }
    }

    public class GetDeliveryMethodsByStoreIdResponse
    {
        public IEnumerable<DeliveryMethodByStoreIdModel> DeliveryMethods { get; set; }
    }

    public class GetDeliveryMethodsByStoreIdRequestHandler : IRequestHandler<GetDeliveryMethodsByStoreIdRequest, GetDeliveryMethodsByStoreIdResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetDeliveryMethodsByStoreIdRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetDeliveryMethodsByStoreIdResponse> Handle(GetDeliveryMethodsByStoreIdRequest request, CancellationToken cancellationToken)
        {
            var deliveryMethods = await _unitOfWork.DeliveryMethods
                .GetAll()
                .Include(dvm => dvm.DeliveryConfigs.Where(dvc => dvc.StoreId == request.StoreId)).ThenInclude(dvcp => dvcp.DeliveryConfigPricings)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            var deliveryMethodsActive = deliveryMethods.Where(dvm => dvm.EnumId == EnumDeliveryMethod.AhaMove || (dvm.EnumId == EnumDeliveryMethod.SelfDelivery && dvm.DeliveryConfigs.Count() > 0));

            var deliveryMethodsResponse = _mapper.Map<List<DeliveryMethodByStoreIdModel>>(deliveryMethodsActive);

            var response = new GetDeliveryMethodsByStoreIdResponse()
            {
                DeliveryMethods = deliveryMethodsResponse
            };

            return response;
        }
    }
}
