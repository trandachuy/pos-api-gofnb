using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Models.Option;
using System.Linq;

namespace GoFoodBeverage.Application.Features.Options.Queries
{
    public class GetAllOptionRequest : IRequest<GetAllOptionResponse>
    {
        
    }

    public class GetAllOptionResponse
    {
        public IEnumerable<OptionModel> Options { get; set; }
    }

    public class GetAllOptionRequestHandler : IRequestHandler<GetAllOptionRequest, GetAllOptionResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllOptionRequestHandler(
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

        public async Task<GetAllOptionResponse> Handle(GetAllOptionRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var options = await _unitOfWork.Options
                .GetAllOptionsInStore(loggedUser.StoreId)
                .AsNoTracking()
                .ProjectTo<OptionModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllOptionResponse()
            {
                Options = options
            };

            return response;
        }
    }
}
