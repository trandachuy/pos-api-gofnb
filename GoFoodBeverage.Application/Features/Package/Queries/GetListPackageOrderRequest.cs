using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Package;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Package.Queries
{
    public class GetListPackageOrderRequest : IRequest<GetListPackageOrderResponse>
    {
    }

    public class GetListPackageOrderResponse
    {
        public IEnumerable<PackageOrderModel> PackageOrders { get; set; }
    }

    public class GetListPackageOrderRequestHandler : IRequestHandler<GetListPackageOrderRequest, GetListPackageOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetListPackageOrderRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            MapperConfiguration mapperConfiguration
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetListPackageOrderResponse> Handle(GetListPackageOrderRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var packageOrders = await _unitOfWork.OrderPackages
                .GetAll()
                .Where(a => a.StoreId == loggedUser.StoreId)
                .Include(po => po.Package)
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedTime)
                .ToListAsync(cancellationToken: cancellationToken);

            List<PackageOrderModel> packageOrderResponse = _mapper.Map<List<PackageOrderModel>>(packageOrders);

            var response = new GetListPackageOrderResponse()
            {
                PackageOrders = packageOrderResponse
            };

            return response;
        }
    }
}
