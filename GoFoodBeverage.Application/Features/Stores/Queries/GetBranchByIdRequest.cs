using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Store;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using System;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetBranchByIdRequest : IRequest<GetBranchByIdResponse>
    {
        public Guid BranchId { get; set; }
    }

    public class GetBranchByIdResponse
    {
        public EditBranchModel Branch { get; set; }
    }

    public class GetBranchByIdHandler : IRequestHandler<GetBranchByIdRequest, GetBranchByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetBranchByIdHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetBranchByIdResponse> Handle(GetBranchByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var branch = await _unitOfWork.StoreBranches
                .Find(x => x.StoreId == loggedUser.StoreId && x.Id == request.BranchId && !x.IsDeleted)
                .Include(x => x.Address).FirstOrDefaultAsync();

            var result = new EditBranchModel
            {
                Id = branch.Id,
                BranchName = branch.Name,
                PhoneNumber = branch.PhoneNumber,
                Email = branch.Email,
                CountryId = branch.Address.CountryId,
                StateId = branch.Address.StateId,
                CityTown = branch.Address.CityTown,
                CityId = branch.Address.CityId,
                Address1 = branch.Address.Address1,
                Address2 = branch.Address.Address2,
                PostalCode = branch.Address.PostalCode,
                DistrictId = branch.Address.DistrictId,
                WardId = branch.Address.WardId,
                Lat = branch?.Address?.Latitude,
                Lng = branch?.Address?.Longitude,
            };

            var response = new GetBranchByIdResponse()
            {
                Branch = result
            };

            return response;
        }
    }
}
