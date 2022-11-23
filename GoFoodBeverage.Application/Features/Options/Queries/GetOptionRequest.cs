using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoFoodBeverage.Models.Option;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Options.Queries
{
    public class GetOptionRequest : IRequest<GetOptionsResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetOptionsResponse
    {
        public IEnumerable<OptionModel> Options { get; set; }

        public int Total { get; set; }
    }

    public class GetOptionRequestHandler : IRequestHandler<GetOptionRequest, GetOptionsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IUserProvider _userProvider;

        public GetOptionRequestHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration,
            IUserProvider userProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
            _userProvider = userProvider;
        }

        public async Task<GetOptionsResponse> Handle(GetOptionRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var options = _unitOfWork.Options.GetAllOptionsInStore(loggedUser.StoreId);
            if (!string.IsNullOrEmpty(request.KeySearch) && options != null)
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                options = options.Where(g => g.Name.ToLower().Contains(keySearch));
            }

            var optionsByPaging = await options.OrderByDescending(p => p.CreatedTime).ToPaginationAsync(request.PageNumber, request.PageSize);
            var optionModels = _mapper.Map<IEnumerable<OptionModel>>(optionsByPaging.Result);

            var response = new GetOptionsResponse()
            {
                Options = optionModels,
                Total = optionsByPaging.Total
            };

            return response;
        }
    }
}
