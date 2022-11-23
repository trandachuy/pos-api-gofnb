using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Models.DeliveryMethod;
using MoreLinq;
using DocumentFormat.OpenXml.Wordprocessing;
using static GoFoodBeverage.Models.DeliveryMethod.DeliveryMethodModel;
using static GoFoodBeverage.Models.DeliveryMethod.DeliveryMethodModel.DeliveryConfigDto;
using System.Diagnostics;

namespace GoFoodBeverage.Application.Features.DeliveryMethods.Queries
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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetDeliveryMethodsRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetDeliveryMethodsResponse> Handle(GetDeliveryMethodsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var deliveryMethods = await _unitOfWork.DeliveryMethods
                .GetAll()
                .Include(dvm => dvm.DeliveryConfigs)
                .ThenInclude(dvcp => dvcp.DeliveryConfigPricings)
                .AsNoTracking()
                .Select(dvm => new DeliveryMethodModel()
                {
                    Id = dvm.Id,
                    Name = dvm.Name,
                    EnumId = dvm.EnumId,
                    DeliveryConfig = dvm.DeliveryConfigs.Where(dvc => dvc.StoreId == loggedUser.StoreId).Select(dc => new DeliveryConfigDto
                    {
                        Id = dc.Id,
                        IsFixedFee = dc.IsFixedFee,
                        FeeValue = dc.FeeValue,
                        ApiKey = dc.ApiKey,
                        PhoneNumber = dc.PhoneNumber,
                        Name = dc.Name,
                        Address = dc.Address,
                        IsActivated = dc.IsActivated == null ? false : dc.IsActivated.Value,
                        DeliveryConfigPricings = dc.DeliveryConfigPricings.Select(dcp => new DeliveryConfigPricingDto
                        {
                            Id = dcp.Id,
                            Position = dcp.Position,
                            FromDistance = dcp.FromDistance,
                            ToDistance = dcp.ToDistance,
                            FeeValue = dcp.FeeValue
                        })
                    }).FirstOrDefault(),
                })
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetDeliveryMethodsResponse()
            {
                DeliveryMethods = deliveryMethods
            };

            return response;
        }

    }
}
