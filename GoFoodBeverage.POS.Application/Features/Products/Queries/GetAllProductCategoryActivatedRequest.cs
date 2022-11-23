using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.POS.Models.Combo;
using GoFoodBeverage.POS.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Products.Queries
{
    public class GetAllProductCategoryActivatedRequest : IRequest<GetAllProductCategoryActivatedResponse>
    {
    }

    public class GetAllProductCategoryActivatedResponse
    {
        public IEnumerable<ProductCategoryActivatedModel> ProductCategories { get; set; }

        public IEnumerable<PosComboActivatedModel> Combos { get; set; }
    }

    public class GetActiveAreasByBranchIdRequestHandler : IRequestHandler<GetAllProductCategoryActivatedRequest, GetAllProductCategoryActivatedResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetActiveAreasByBranchIdRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetAllProductCategoryActivatedResponse> Handle(GetAllProductCategoryActivatedRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var productCategoriesAll = _unitOfWork.ProductCategories.GetAll().Where(p => p.StoreId == loggedUser.StoreId.Value && p.IsDisplayAllBranches == true);

            var productCategoriesStoreBranch = _unitOfWork.StoreBranchProductCategories.GetAll()
                 .Where(s => s.StoreId == loggedUser.StoreId && s.StoreBranchId == loggedUser.BranchId.Value)
                 .Include(sb => sb.ProductCategory).Select(i => i.ProductCategory);

            var productCategories = await productCategoriesAll
                .Union(productCategoriesStoreBranch)
                .OrderBy(x => x.Priority).ThenBy(x => x.Name)
                .ProjectTo<ProductCategoryActivatedModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var combos = await _unitOfWork.Combos
                .GetAllCombosInStore(loggedUser.StoreId.Value)
                .Include(cb => cb.ComboPricings)
                .AsNoTracking()
                .ProjectTo<PosComboActivatedModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllProductCategoryActivatedResponse()
            {
                ProductCategories = productCategories,
                Combos = combos
            };

            return response;
        }
    }
}
