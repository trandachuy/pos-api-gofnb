using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Common.Helpers;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.Services;
using GoFoodBeverage.Models.OnlineStoreMenus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.OnlineStoreMenus.Queries
{
    public class GetAllMenuRequest : IRequest<GetAllMenuResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetAllMenuResponse
    {
        public IEnumerable<OnlineStoreMenuModel> MenuManagements { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }

    }

    public class GetAllMenuRequestHandler : IRequestHandler<GetAllMenuRequest, GetAllMenuResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllMenuRequestHandler(
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

        public async Task<GetAllMenuResponse> Handle(GetAllMenuRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(loggedUser.StoreId == null, "Cannot find store information");

            var listMenu = new PagingExtensions.Pager<OnlineStoreMenuModel>(new List<OnlineStoreMenuModel>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                listMenu = await _unitOfWork.OnlineStoreMenus
                .GetAllMenuInStore(loggedUser.StoreId.Value)
                .AsNoTracking()
                .ProjectTo<OnlineStoreMenuModel>(_mapperConfiguration)
                .OrderByDescending(p => p.CreatedTime)
                .ToPaginationAsync(request.PageNumber, request.PageSize);
            }
            else
            {
                string keySearch = StringHelpers.RemoveSign4VietnameseString(request.KeySearch.Trim().ToLower());
                listMenu = await _unitOfWork.OnlineStoreMenus
                .GetAllMenuInStore(loggedUser.StoreId.Value)
                .Where(qr => qr.Name.ToLower().Contains(keySearch))
                .AsNoTracking()
                .ProjectTo<OnlineStoreMenuModel>(_mapperConfiguration)
                .OrderByDescending(p => p.CreatedTime)
                .ToPaginationAsync(request.PageNumber, request.PageSize);
            }

            var listMenuModels = _mapper.Map<IEnumerable<OnlineStoreMenuModel>>(listMenu.Result);

            var response = new GetAllMenuResponse()
            {
                MenuManagements = listMenuModels,
                PageNumber = request.PageNumber,
                Total = listMenu.Total
            };

            return response;
        }
    }
}
