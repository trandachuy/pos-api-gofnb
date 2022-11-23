using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using System;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class GetMaterialCategoryByIdRequest : IRequest<GetMaterialCategoryByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetMaterialCategoryByIdResponse
    {
        public MaterialCategoryByIdModel MaterialCategory { get; set; }
    }

    public class GetMaterialCategoryByIdRequestHandler : IRequestHandler<GetMaterialCategoryByIdRequest, GetMaterialCategoryByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetMaterialCategoryByIdRequestHandler(
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

        public async Task<GetMaterialCategoryByIdResponse> Handle(GetMaterialCategoryByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var materialCategory = await _unitOfWork.MaterialCategories
                .Find(m => m.StoreId == loggedUser.StoreId && m.Id == request.Id)
                .Include(m => m.Materials).ThenInclude(m => m.Unit)
                .ProjectTo<MaterialCategoryByIdModel>(_mapperConfiguration)
                .FirstOrDefaultAsync();

            var response = new GetMaterialCategoryByIdResponse()
            {
                MaterialCategory = materialCategory
            };

            return response;
        }
    }
}
