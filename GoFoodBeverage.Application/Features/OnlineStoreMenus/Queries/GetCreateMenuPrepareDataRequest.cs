using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.OnlineStoreMenus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.OnlineStoreMenus.Queries
{
    public class GetCreateMenuPrepareDataRequest : IRequest<GetCreateMenuPrepareDataResponse>
    {
    }

    public class GetCreateMenuPrepareDataResponse
    {
        public IEnumerable<LevelDto> Levels { get; set; }

        public class LevelDto
        {
            public EnumLevelMenu LevelId { get; set; }

            public string LevelName { get; set; }
        }

        public IEnumerable<ProductCategoryDto> ProductCategories { get; set; }

        public IEnumerable<ProductCategoryDto> Products { get; set; }

        public class ProductCategoryDto
        {
            public Guid? Id { get; set; }

            public string Name { get; set; }
        }

        public IEnumerable<SubMenuDetailModel> SubMenus { get; set; }
    }

    public class GetCreateMenuPrepareDataRequestHandler : IRequestHandler<GetCreateMenuPrepareDataRequest, GetCreateMenuPrepareDataResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetCreateMenuPrepareDataRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetCreateMenuPrepareDataResponse> Handle(GetCreateMenuPrepareDataRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            ///Get all level
            var levels = Enum.GetValues(typeof(EnumLevelMenu))
                .Cast<EnumLevelMenu>()
                .Select(e => new GetCreateMenuPrepareDataResponse.LevelDto { LevelId = e, LevelName = e.GetName() })
                .ToList();

            ///Get list product categories
            var productCategories = await _unitOfWork.ProductCategories
                .GetAllProductCategoriesInStore(loggedUser.StoreId.Value)
                .Select(x => new GetCreateMenuPrepareDataResponse.ProductCategoryDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);

            ///Get list products
            var products = await _unitOfWork.Products
                .GetAllProductInStoreActive(loggedUser.StoreId.Value)
                .Where(x => !x.IsTopping)
                .Select(x => new GetCreateMenuPrepareDataResponse.ProductCategoryDto { Id = x.Id, Name = x.Name })
                .ToListAsync(cancellationToken);

            var subMenus = await _unitOfWork.OnlineStoreMenus
                .GetAllSubMenuInStore(loggedUser.StoreId.Value)
                .ProjectTo<SubMenuDetailModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken);

            var response = new GetCreateMenuPrepareDataResponse()
            {
                Levels = levels,
                Products = products,
                ProductCategories = productCategories,
                SubMenus = subMenus,
            };

            return response;
        }
    }
}
