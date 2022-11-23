using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Models.Supplier;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Suppliers.Queries
{
    public class GetAllSuppilerRequest : IRequest<GetAllSuppilerResponse>
    {
        
    }

    public class GetAllSuppilerResponse
    {
        public IEnumerable<SupplierModel> Suppliers { get; set; }
    }

    public class GetAllSuppilerRequestHandler : IRequestHandler<GetAllSuppilerRequest, GetAllSuppilerResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllSuppilerRequestHandler(
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

        public async Task<GetAllSuppilerResponse> Handle(GetAllSuppilerRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var suppliers = await _unitOfWork.Suppliers
                .GetAllSuppliersInStore(loggedUser.StoreId)
                .AsNoTracking()
                .ProjectTo<SupplierModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllSuppilerResponse()
            {
                Suppliers = suppliers
            };

            return response;
        }
    }
}
