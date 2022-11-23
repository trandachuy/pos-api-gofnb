using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Area;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Areas.Queries
{
    public class GetAreaByIdRequest : IRequest<GetAreasByIdResponse>
    {
        public Guid Id { get; set; }

        public Guid StoreBranchId { get; set; }
    }

    public class GetAreasByIdResponse
    {
        public AreaByIdModel Area { get; set; }
    }

    public class GetAreasByIdRequestHandler : IRequestHandler<GetAreaByIdRequest, GetAreasByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAreasByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetAreasByIdResponse> Handle(GetAreaByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var area = await _unitOfWork.Areas
                .GetAreaById(request.Id, loggedUser.StoreId, request.StoreBranchId)
                .AsNoTracking()
                .ProjectTo<AreaByIdModel>(_mapperConfiguration)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            ThrowError.Against(area == null, "Cannot find area information");

            var response = new GetAreasByIdResponse()
            {
                Area = area
            };
            return response;
        }
    }
}
