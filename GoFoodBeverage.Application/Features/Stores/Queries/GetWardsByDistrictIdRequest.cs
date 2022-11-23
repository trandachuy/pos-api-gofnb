using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Address;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetWardsByDistrictIdRequest : IRequest<GetWardsByDistrictIdResponse>
    {
        public Guid? DistrictId { get; set; }
    }

    public class GetWardsByDistrictIdResponse
    {
        public IList<WardModel> Wards { get; set; }
    }

    public class GetWardsByDistrictIdRequestHandler : IRequestHandler<GetWardsByDistrictIdRequest, GetWardsByDistrictIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetWardsByDistrictIdRequestHandler(
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetWardsByDistrictIdResponse> Handle(GetWardsByDistrictIdRequest request, CancellationToken cancellationToken)
        {
            ThrowError.BadRequestAgainstNull(request.DistrictId, "Please enter the DistrictId");

            var wards = await _unitOfWork.Wards
                .GetWardsByDistrictId(request.DistrictId.Value)
                .ProjectTo<WardModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetWardsByDistrictIdResponse()
            {
                Wards = wards
            };

            return response;
        }
    }
}
